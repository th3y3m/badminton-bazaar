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
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public ColorService(IColorRepository colorRepository, IProductVariantRepository productVariantRepository, IConnectionMultiplexer redisConnection)
        {
            _colorRepository = colorRepository;
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

        public async Task<List<ColorModel>> GetColorsOfProduct(string productId)
        {
            try
            {
                var allProduct = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _productVariantRepository.GetAll()
                );
                var productVariants = allProduct.Where(p => p.ProductId == productId).ToList();
                var colorIds = productVariants.Select(p => p.ColorId).ToList();
                var allColors = await GetAll();
                var colors = allColors.Where(p => colorIds.Contains(p.ColorId)).ToList();
                var colorModels = new List<ColorModel>();
                foreach (var color in colors)
                {
                    var colorModel = new ColorModel
                    {
                        ColorId = color.ColorId,
                        ColorName = color.ColorName
                    };
                    colorModels.Add(colorModel);
                }
                return colorModels;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving colors of the product.", ex);
            }
        }

        public async Task<Color> Add(string colorName)
        {
            try
            {
                var color = new Color
                {
                    ColorId = "CL" + GenerateId.GenerateRandomId(5),
                    ColorName = colorName
                };

                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _colorRepository.Add(color)
                );

                if (_redisConnection != null && _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"color:{color.ColorId}", JsonConvert.SerializeObject(color), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
                return color;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the color.", ex);
            }
        }

        public async Task Update(Color color)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _colorRepository.Update(color)
                );

                if (_redisConnection != null && _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"color:{color.ColorId}", JsonConvert.SerializeObject(color), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the color.", ex);
            }
        }

        public async Task<Color> GetById(string id)
        {
            try
            {
                RedisValue cachedColor = RedisValue.Null;

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {

                    try
                    {
                        cachedColor = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringGetAsync($"color:{id}")
                        );
                        if (!cachedColor.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<Color>(cachedColor);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var color = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _colorRepository.GetById(id)
                );

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        cachedColor = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"color:{id}", JsonConvert.SerializeObject(color), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
                return color;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the color by ID.", ex);
            }
        }

        public async Task<List<Color>> GetAll()
        {
            try
            {
                List<Color> colors = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _colorRepository.GetAll()
                );

                return colors;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all colors.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _colorRepository.Delete(id)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.KeyDeleteAsync($"color:{id}")
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the color.", ex);
            }
        }

        public async Task<PaginatedList<Color>> GetPaginatedColors(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _colorRepository.GetDbSet()
                );
                var source = dbSet.AsNoTracking();
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.ColorName.ToLower().Contains(searchQuery.ToLower()));
                }

                source = sortBy switch
                {
                    "color_asc" => source.OrderBy(p => p.ColorName),
                    "color_desc" => source.OrderByDescending(p => p.ColorName),
                    _ => source
                };

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<Color>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving paginated colors.", ex);
            }
        }
    }
}
