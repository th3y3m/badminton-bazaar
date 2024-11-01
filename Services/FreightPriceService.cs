using BusinessObjects;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Repositories.Interfaces;
using Services.Interface;
using StackExchange.Redis;

namespace Services
{
    public class FreightPriceService : IFreightPriceService
    {
        private readonly IFreightPriceRepository _freightPriceRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public FreightPriceService(IFreightPriceRepository freightPriceRepository, IConnectionMultiplexer redisConnection)
        {
            _freightPriceRepository = freightPriceRepository;
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

        public async Task Add(FreightPrice freightPrice)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _freightPriceRepository.Add(freightPrice);
                });

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"freightPrice:{freightPrice.PriceId}", JsonConvert.SerializeObject(freightPrice), TimeSpan.FromHours(1))
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
                throw new Exception("An error occurred while adding the freight price.", ex);
            }
        }

        public async Task Update(FreightPrice freightPrice)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _freightPriceRepository.Update(freightPrice)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"freightPrice:{freightPrice.PriceId}", JsonConvert.SerializeObject(freightPrice), TimeSpan.FromHours(1))
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
                throw new Exception("An error occurred while updating the freight price.", ex);
            }
        }

        public async Task<FreightPrice> GetById(string id)
        {
            try
            {
                RedisValue cachedFreightPrice = RedisValue.Null;
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        cachedFreightPrice = await _redisPolicyWrap.ExecuteAsync(async () =>
                           await _redisDb.StringGetAsync($"freightPrice:{id}")
                       );
                        if (!cachedFreightPrice.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<FreightPrice>(cachedFreightPrice);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var freightPrice = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _freightPriceRepository.GetById(id)
                );

                try
                {
                    cachedFreightPrice = await _redisPolicyWrap.ExecuteAsync(async () =>
                       await _redisDb.StringSetAsync($"freightPrice:{id}", JsonConvert.SerializeObject(freightPrice), TimeSpan.FromHours(1))
                   );
                    if (!cachedFreightPrice.IsNullOrEmpty)
                        return JsonConvert.DeserializeObject<FreightPrice>(cachedFreightPrice);
                }
                catch (RedisConnectionException ex)
                {
                    Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                }

                return freightPrice;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the freight price by ID.", ex);
            }
        }

        public async Task<List<FreightPrice>> GetAll()
        {
            try
            {
                var freightPrices = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _freightPriceRepository.GetAll()
                );
                return freightPrices;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all freight prices.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _freightPriceRepository.Delete(id)
                );

                try
                {
                    await _redisPolicyWrap.ExecuteAsync(async () =>
                       await _redisDb.KeyDeleteAsync($"freightPrice:{id}")
                   );
                }
                catch (RedisConnectionException ex)
                {
                    Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                }


            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the freight price.", ex);
            }
        }

        public async Task<decimal> GetPriceByDistance(decimal km)
        {

            try
            {
                var freightPrices = await GetAll();
                var freightPrice = freightPrices.FirstOrDefault(x => x.MinDistance <= km && x.MaxDistance >= km);
                if (freightPrice == null)
                {
                    throw new Exception("Freight price not found for the specified distance.");
                }
                return freightPrice.PricePerKm;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the freight price by distance.", ex);
            }
        }
    }
}
