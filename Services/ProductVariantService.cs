using BusinessObjects;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductService _productService;
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public ProductVariantService(IProductVariantRepository productVariantRepository, IHubContext<ProductHub> hubContext, IProductService productService, IConnectionMultiplexer redisConnection)
        {
            _productVariantRepository = productVariantRepository;
            _hubContext = hubContext;
            _productService = productService;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
            _redisRetryPolicy = Policy.Handle<SqlException>()
                                   .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                   (exception, timeSpan, retryCount, context) =>
                                   {
                                       Console.WriteLine($"Retry {retryCount} for {context.PolicyKey} at {timeSpan} due to: {exception}.");
                                   });
            _dbRetryPolicy = Policy.Handle<SqlException>()
                                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                (exception, timeSpan, retryCount, context) =>
                                {
                                    Console.WriteLine($"[Db Retry] Attempt {retryCount} after {timeSpan} due to: {exception.Message}");
                                });
            _dbTimeoutPolicy = Policy.TimeoutAsync(10, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Db Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _redisTimeoutPolicy = Policy.TimeoutAsync(2, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Redis Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _dbPolicyWrap = Policy.WrapAsync(_dbRetryPolicy, _dbTimeoutPolicy);
            _redisPolicyWrap = Policy.WrapAsync(_redisRetryPolicy, _redisTimeoutPolicy);

        }

        public async Task<ProductVariant> Add(ProductVariantModel productVariantModel)
        {
            try
            {
                var productVariant = new ProductVariant
                {
                    ProductVariantId = "PV" + GenerateId.GenerateRandomId(5),
                    ProductId = productVariantModel.ProductId,
                    SizeId = productVariantModel.SizeId,
                    ColorId = productVariantModel.ColorId,
                    Price = productVariantModel.Price,
                    StockQuantity = productVariantModel.StockQuantity,
                    Status = productVariantModel.Status,
                    VariantImageURL = productVariantModel.ProductImageUrl != null ? productVariantModel.ProductImageUrl[0].FileName : null
                };

                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _productVariantRepository.Add(productVariant);
                });

                await _redisDb.StringSetAsync($"productVariant:{productVariant.ProductVariantId}", JsonConvert.SerializeObject(productVariant), TimeSpan.FromHours(1));

                return productVariant;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product variant: {ex.Message}");
            }
        }

        public async Task Update(ProductVariantModel productVariantModel, string id)
        {
            try
            {
                var productVariant = await GetById(id);

                if (productVariant == null)
                {
                    throw new Exception("Product variant not found");
                }

                bool stockChanged = productVariant.StockQuantity != productVariantModel.StockQuantity;

                productVariant.ProductId = productVariantModel.ProductId;
                productVariant.SizeId = productVariantModel.SizeId;
                productVariant.ColorId = productVariantModel.ColorId;
                productVariant.Price = productVariantModel.Price;
                productVariant.StockQuantity = productVariantModel.StockQuantity;
                productVariant.Status = productVariantModel.Status;
                productVariant.VariantImageURL = productVariantModel.ProductImageUrl != null ? productVariantModel.ProductImageUrl[0].FileName : null;

                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _productVariantRepository.Update(productVariant);
                });
                await _redisDb.StringSetAsync($"productVariant:{id}", JsonConvert.SerializeObject(productVariant), TimeSpan.FromHours(1));

                if (stockChanged)
                {
                    await UpdateProductStock(productVariant.ProductVariantId, productVariant.StockQuantity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product variant: {ex.Message}");
            }
        }
        public async Task Update(ProductVariant productVariantModel)
        {
            try
            {
                await UpdateProductStock(productVariantModel.ProductVariantId, productVariantModel.StockQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product variant: {ex.Message}");
            }
        }


        public async Task<ProductVariant> GetById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        var cachedProduct = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringGetAsync($"productVariant:{id}")
                        );

                        if (!cachedProduct.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<ProductVariant>(cachedProduct);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var product = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productVariantRepository.GetById(id)
                );

                try
                {
                    await _redisPolicyWrap.ExecuteAsync(async () =>
                        await _redisDb.StringSetAsync($"productVariant:{id}", JsonConvert.SerializeObject(product), TimeSpan.FromHours(1))
                    );
                }
                catch (RedisConnectionException ex)
                {
                    Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product variant by ID: {ex.Message}");
            }
        }

        public async Task<List<ProductVariant>> GetAll()
        {
            try
            {
                var products = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productVariantRepository.GetAll()
                );

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all product variants: {ex.Message}");
            }
        }

        public async Task DeleteById(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productVariantRepository.Delete(id)
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product variant: {ex.Message}");
            }
        }

        public async Task<PaginatedList<ProductVariant>> GetPaginatedProductVariants(
            string sortBy,
            bool? status,
            string colorId,
            string sizeId,
            string productId,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productVariantRepository.GetDbSet()
                );
                var source = dbSet.AsNoTracking();

                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                if (!string.IsNullOrEmpty(colorId))
                {
                    source = source.Where(p => p.ColorId == colorId);
                }

                if (!string.IsNullOrEmpty(sizeId))
                {
                    source = source.Where(p => p.SizeId == sizeId);
                }
                if (!string.IsNullOrEmpty(productId))
                {
                    source = source.Where(p => p.ProductId == productId);
                }

                source = sortBy switch
                {
                    "price_asc" => source.OrderBy(p => p.Price),
                    "price_desc" => source.OrderByDescending(p => p.Price),
                    "stock_asc" => source.OrderBy(p => p.StockQuantity),
                    "stock_desc" => source.OrderByDescending(p => p.StockQuantity),
                    _ => source
                };

                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<ProductVariant>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated product variants: {ex.Message}");
            }
        }

        public async Task<bool> CheckStock(List<CartItem> productVariants)
        {
            try
            {
                foreach (var productVariant in productVariants)
                {
                    var variant = await GetById(productVariant.ItemId);
                    if (variant.StockQuantity < productVariant.Quantity)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking stock: {ex.Message}");
            }
        }
        public async Task<bool> CheckStock(CartItem cartItem)
        {
            try
            {

                var variant = await GetById(cartItem.ItemId);
                if (variant.StockQuantity < cartItem.Quantity)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking stock: {ex.Message}");
            }
        }

        public async Task UpdateProductStock(string productId, int newStockQuantity)
        {
            try
            {

                var product = await GetById(productId);
                if (product == null)
                {
                    return;
                }
                product.StockQuantity = newStockQuantity;

                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productVariantRepository.Update(product)
                );

                var newStock = await _productService.ProductRemaining(product.ProductId);

                await _hubContext.Clients.All.SendAsync("ReceiveProductStockUpdate", product.ProductId, newStock);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product stock: {ex.Message}");
            }


        }

        public async Task UpdateMultipleProductStocks(Dictionary<string, int> productStockUpdates)
        {
            foreach (var update in productStockUpdates)
            {
                var product = await GetById(update.Key);
                if (product != null)
                {
                    product.StockQuantity = update.Value;
                    await _dbPolicyWrap.ExecuteAsync(async () =>
                        await _productVariantRepository.Update(product)
                    );
                }
            }

            // Notify clients about the stock updates
            await _hubContext.Clients.All.SendAsync("ReceiveMultipleProductStockUpdates", productStockUpdates);
        }
    }
}
