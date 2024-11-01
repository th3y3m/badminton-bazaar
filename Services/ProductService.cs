using BusinessObjects;
using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json;
using Polly.Retry;
using Polly;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Polly.Timeout;
using Polly.Wrap;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _policyWrap;

        public ProductService(IProductRepository productRepository, IProductVariantRepository productVariantRepository, IOrderDetailRepository orderDetailRepository, IConnectionMultiplexer redisConnection, IElasticsearchService elasticClient)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _orderDetailRepository = orderDetailRepository;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
            _elasticsearchService = elasticClient;
            _retryPolicy = Polly.Policy.Handle<SqlException>()
                                   .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                   (exception, timeSpan, retryCount, context) =>
                                   {
                                       Console.WriteLine($"Retry {retryCount} for {context.PolicyKey} at {timeSpan} due to: {exception}.");
                                   });
            _dbRetryPolicy = Polly.Policy.Handle<SqlException>()
                                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                (exception, timeSpan, retryCount, context) =>
                                {
                                    Console.WriteLine($"[Db Retry] Attempt {retryCount} after {timeSpan} due to: {exception.Message}");
                                });
            _dbTimeoutPolicy = Polly.Policy.TimeoutAsync(10, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
           {
               Console.WriteLine($"[Db Timeout] Operation timed out after {timeSpan}");
               return Task.CompletedTask;
           });

            _timeoutPolicy = Polly.Policy.TimeoutAsync(2, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
           {
               Console.WriteLine($"[Redis Timeout] Operation timed out after {timeSpan}");
               return Task.CompletedTask;
           });
            _dbPolicyWrap = Polly.Policy.WrapAsync(_dbRetryPolicy, _dbTimeoutPolicy);
            _policyWrap = Polly.Policy.WrapAsync(_retryPolicy, _timeoutPolicy);
        }

        public async Task<PaginatedList<Product>> GetPaginatedProducts(
            string searchQuery,
            decimal? start,
            decimal? end,
            string sortBy,
            bool? status,
            string supplierId,
            string categoryId,
            int pageIndex,
            int pageSize)
        {
            try
            {
                DbSet<Product> dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productRepository.GetDbSet()
                );

                var source = dbSet.AsNoTracking();

                if (start.HasValue && end.HasValue)
                {
                    source = source.Where(p => p.BasePrice >= start && p.BasePrice <= end);
                }

                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                if (!string.IsNullOrEmpty(supplierId))
                {
                    source = source.Where(p => p.SupplierId == supplierId);
                }

                if (!string.IsNullOrEmpty(categoryId))
                {
                    source = source.Where(p => p.CategoryId == categoryId);
                }

                source = sortBy switch
                {
                    "name_asc" => source.OrderBy(p => p.ProductName),
                    "name_desc" => source.OrderByDescending(p => p.ProductName),
                    "price_asc" => source.OrderBy(p => p.BasePrice),
                    "price_desc" => source.OrderByDescending(p => p.BasePrice),
                    "top_seller" => source.OrderByDescending(p => GetSelledProduct(p.ProductId)),
                    _ => source
                };

                List<Product> items = await source
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(searchQuery) && items.Any())
                {
                    if (!await _elasticsearchService.IsAvailableAsync())
                    {
                        items = items
                            .Where(p => p.ProductName.ToLower().Contains(searchQuery.ToLower()))
                            .ToList();
                    }
                    else
                    {
                        await _policyWrap.ExecuteAsync(async () =>
                        {
                            await _elasticsearchService.IndexProductsAsync(items);
                        });

                        items = await _policyWrap.ExecuteAsync(async () =>
                        {
                            return await _elasticsearchService.SearchProductsByNameAsync(searchQuery);
                        });
                    }
                }

                return new PaginatedList<Product>(items, items.Count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated products: {ex.Message}");
            }
        }

        public async Task<Product> AddProduct(ProductModel productModel)
        {
            try
            {
                var product = new Product
                {
                    ProductId = "P" + GenerateId.GenerateRandomId(4),
                    ProductName = productModel.ProductName,
                    CategoryId = productModel.CategoryId,
                    SupplierId = productModel.SupplierId,
                    ProductDescription = productModel.ProductDescription,
                    BasePrice = productModel.BasePrice,
                    ImageUrl = productModel.ImageUrl,
                    Status = productModel.Status
                };

                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productRepository.Add(product)
                );

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _policyWrap.ExecuteAsync(async () =>
                        {
                            await _redisDb.StringSetAsync($"product:{product.ProductId}", JsonConvert.SerializeObject(product), TimeSpan.FromHours(1));
                        });
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product: {ex.Message}");
            }
        }

        public async Task<Product> UpdateProduct(ProductModel productModel, string productId)
        {
            try
            {
                var product = await GetProductById(productId);

                product.ProductName = productModel.ProductName;
                product.CategoryId = productModel.CategoryId;
                product.SupplierId = productModel.SupplierId;
                product.ProductDescription = productModel.ProductDescription;
                product.BasePrice = productModel.BasePrice;
                product.ImageUrl = productModel.ImageUrl ?? string.Empty;
                product.Status = productModel.Status;

                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productRepository.Update(product)
                );

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _policyWrap.ExecuteAsync(async () =>
                        {
                            await _redisDb.StringSetAsync($"product:{product.ProductId}", JsonConvert.SerializeObject(product), TimeSpan.FromHours(1));
                        });
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}");
            }
        }

        public async Task DeleteProduct(string productId)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _productRepository.Delete(productId);
                });

                var product = await GetProductById(productId);

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _policyWrap.ExecuteAsync(async () =>
                        {
                            await _redisDb.StringSetAsync($"product:{product.ProductId}", JsonConvert.SerializeObject(product), TimeSpan.FromHours(1));
                        });
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<Product> GetProductById(string productId)
        {
            Product product = null;

            // Check Redis cache first
            if (_redisConnection != null && _redisConnection.IsConnected)
            {
                try
                {
                    RedisValue cachedProduct = RedisValue.Null;
                    await _policyWrap.ExecuteAsync(async () =>
                    {
                        cachedProduct = await _redisDb.StringGetAsync($"product:{productId}");
                    });

                    if (!cachedProduct.IsNullOrEmpty)
                    {
                        return JsonConvert.DeserializeObject<Product>(cachedProduct);
                    }
                }
                catch (RedisConnectionException ex)
                {
                    Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                }
            }

            // Fetch from database with retry policy
            await _dbPolicyWrap.ExecuteAsync(async () =>
            {
                product = await _productRepository.GetById(productId);
            });

            // Cache result in Redis if connected
            if (_redisConnection != null && _redisConnection.IsConnected)
            {
                try
                {
                    await _policyWrap.ExecuteAsync(async () =>
                    {
                        await _redisDb.StringSetAsync($"product:{productId}", JsonConvert.SerializeObject(product), TimeSpan.FromHours(1));
                    });
                }
                catch (RedisConnectionException ex)
                {
                    Console.WriteLine($"Failed to set Redis cache: {ex.Message}");
                }
            }

            return product;
        }

        public async Task<int> GetSelledProduct(string productId)
        {
            try
            {
                var orderDetails = await _orderDetailRepository.GetAll();
                var products = await _productVariantRepository.GetAll();
                return orderDetails.Where(od => products.Any(pv => pv.ProductId == productId && pv.ProductVariantId == od.ProductVariantId))
                                   .Sum(od => od.Quantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sold product count: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetTopSeller(int n)
        {
            try
            {
                var products = await _productRepository.GetAll();
                var productSales = new List<(Product Product, int TotalSales)>();

                foreach (var product in products)
                {
                    var totalSales = await GetSelledProduct(product.ProductId);
                    product.ProductVariants = null;
                    productSales.Add((product, totalSales));
                }

                var topSellers = productSales.OrderByDescending(ps => ps.TotalSales)
                                             .Take(n)
                                             .Select(ps => ps.Product)
                                             .ToList();

                return topSellers;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving top sellers: {ex.Message}");
            }
        }

        public async Task<int> ProductRemaining(string productId)
        {
            try
            {
                var allProduct = await _productVariantRepository.GetAll();
                var productVariants = allProduct.Where(p => p.ProductId == productId).ToList();
                int total = 0;
                foreach (var productVariant in productVariants)
                {
                    total += productVariant.StockQuantity;
                }
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product remaining: {ex.Message}");
            }
        }

        public async Task<Product> GetProductByProductVariantId(string productVariantId)
        {
            try
            {
                var productVariant = await _productVariantRepository.GetById(productVariantId);
                return await _productRepository.GetById(productVariant.ProductId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product by product variant ID: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetRelatedProduct(string productId)
        {
            try
            {
                var product = await _productRepository.GetById(productId);
                var products = await _productRepository.GetAll();
                return products.Where(p => p.CategoryId == product.CategoryId && p.ProductId != productId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving related products: {ex.Message}");
            }
        }
    }
}
