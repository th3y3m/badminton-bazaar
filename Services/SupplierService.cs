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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public SupplierService(ISupplierRepository supplierRepository, IConnectionMultiplexer redisConnection)
        {
            _supplierRepository = supplierRepository;
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

        public async Task<PaginatedList<Supplier>> GetPaginatedSuppliers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
            {

                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () => await _supplierRepository.GetDbSet());
                
                var source = dbSet.AsNoTracking();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.CompanyName.ToLower().Contains(searchQuery.ToLower()));
                }

                // Apply status filter
                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                // Apply sorting
                source = sortBy switch
                {
                    "companyname_asc" => source.OrderBy(p => p.CompanyName),
                    "companyname_desc" => source.OrderByDescending(p => p.CompanyName),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<Supplier>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated suppliers: {ex.Message}");
            }
        }

        public async Task<Supplier> GetSupplierById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        var cachedSupplier = await _redisPolicyWrap.ExecuteAsync(async () => await _redisDb.StringGetAsync($"supplier:{id}"));
                        if (!cachedSupplier.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<Supplier>(cachedSupplier);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var supplier = await _dbPolicyWrap.ExecuteAsync(async () => await _supplierRepository.GetById(id));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () => 
                            await _redisDb.StringSetAsync($"supplier:{id}", JsonConvert.SerializeObject(supplier), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return supplier;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier by ID: {ex.Message}");
            }
        }

        public async Task<Supplier> AddSupplier(SupplierModel supplierModel)
        {
            try
            {
                var supplier = new Supplier
                {
                    SupplierId = "SUP" + GenerateId.GenerateRandomId(5),
                    CompanyName = supplierModel.CompanyName,
                    Address = supplierModel.Address,
                    Status = supplierModel.Status
                };
                await _dbPolicyWrap.ExecuteAsync(async () => await _supplierRepository.Add(supplier));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"supplier:{supplier.SupplierId}", JsonConvert.SerializeObject(supplier), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return supplier;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding supplier: {ex.Message}");
            }
        }

        public async Task<Supplier?> UpdateSupplier(SupplierModel supplierModel, string id)
        {
            try
            {
                var supplier = await _dbPolicyWrap.ExecuteAsync(async () => await _supplierRepository.GetById(id));

                if (supplier == null)
                {
                    return null;
                }
                supplier.CompanyName = supplierModel.CompanyName;
                supplier.Address = supplierModel.Address;
                supplier.Status = supplierModel.Status;

                await _dbPolicyWrap.ExecuteAsync(async () => await _supplierRepository.Update(supplier));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"supplier:{id}", JsonConvert.SerializeObject(supplier), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return supplier;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating supplier: {ex.Message}");
            }
        }

        public async Task DeleteSupplier(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () => await _supplierRepository.Delete(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting supplier: {ex.Message}");
            }
        }
    }
}
