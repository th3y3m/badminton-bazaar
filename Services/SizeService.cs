using BusinessObjects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;

namespace Services
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public SizeService(ISizeRepository sizeRepository, IProductVariantRepository productVariantRepository, IConnectionMultiplexer redisConnection)
        {
            _sizeRepository = sizeRepository;
            _productVariantRepository = productVariantRepository;
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

        public async Task<List<SizeModel>> GetSizesOfProduct(string productId)
        {
            try
            {
                var allProduct = await _dbPolicyWrap.ExecuteAsync(async () => await _productVariantRepository.GetAll());

                var productVariants = allProduct.Where(p => p.ProductId == productId).ToList();
                var sizeIds = productVariants.Select(p => p.SizeId).ToList();
                var allSizes = await GetAll();
                var sizes = allSizes.Where(p => sizeIds.Contains(p.SizeId)).ToList();
                var sizeModels = new List<SizeModel>();
                foreach (var size in sizes)
                {
                    var sizeModel = new SizeModel
                    {
                        SizeId = size.SizeId,
                        SizeName = size.SizeName
                    };
                    sizeModels.Add(sizeModel);
                }
                return sizeModels;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sizes of product: {ex.Message}");
            }
        }

        public async Task<Size> Add(string sizeName)
        {
            try
            {
                var size = new Size
                {
                    SizeId = "SZ" + GenerateId.GenerateRandomId(5),
                    SizeName = sizeName
                };
                await _dbPolicyWrap.ExecuteAsync(async () => await _sizeRepository.Add(size));
                await _redisDb.StringSetAsync($"size:{size.SizeId}", JsonConvert.SerializeObject(size), TimeSpan.FromHours(1));
                return size;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding size: {ex.Message}");
            }
        }

        public async Task Update(Size size)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () => await _sizeRepository.Update(size));
                await _redisDb.StringSetAsync($"size:{size.SizeId}", JsonConvert.SerializeObject(size), TimeSpan.FromHours(1));

            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating size: {ex.Message}");
            }
        }

        public async Task<Size> GetById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        var cachedSize = await _redisPolicyWrap.ExecuteAsync(async () => await _redisDb.StringGetAsync($"size:{id}"));
                        if (!cachedSize.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<Size>(cachedSize);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var size = await _dbPolicyWrap.ExecuteAsync(async () => await _sizeRepository.GetById(id));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () => 
                            await _redisDb.StringSetAsync($"size:{id}", JsonConvert.SerializeObject(size), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
                return size;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving size by ID: {ex.Message}");
            }
        }

        public async Task<List<Size>> GetAll()
        {
            try
            {
                var sizes = await _dbPolicyWrap.ExecuteAsync(async () => await _sizeRepository.GetAll());
                return sizes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all sizes: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () => await _sizeRepository.Delete(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting size: {ex.Message}");
            }
        }

        public async Task<PaginatedList<Size>> GetPaginatedSizes(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () => await _sizeRepository.GetDbSet());
                
                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.SizeName.ToLower().Contains(searchQuery.ToLower()));
                }

                source = sortBy switch
                {
                    "size_asc" => source.OrderBy(p => p.SizeName),
                    "size_desc" => source.OrderByDescending(p => p.SizeName),
                    _ => source
                };

                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<Size>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated sizes: {ex.Message}");
            }
        }
    }
}
