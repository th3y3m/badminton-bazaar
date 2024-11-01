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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public CategoryService(ICategoryRepository categoryRepository, IConnectionMultiplexer redisConnection)
        {
            _categoryRepository = categoryRepository;
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

        public async Task<PaginatedList<Category>> GetPaginatedCategories(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _categoryRepository.GetDbSet()
                );
                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.CategoryName.ToLower().Contains(searchQuery.ToLower()));
                }

                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                source = sortBy switch
                {
                    "categoryname_asc" => source.OrderBy(p => p.CategoryName),
                    "categoryname_desc" => source.OrderByDescending(p => p.CategoryName),
                    _ => source
                };

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<Category>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving paginated categories.", ex);
            }
        }

        public async Task<Category> GetCategoryById(string id)
        {
            try
            {
                RedisValue cachedCategory = RedisValue.Null;

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        cachedCategory = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringGetAsync($"category:{id}")
                        );
                        if (!cachedCategory.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<Category>(cachedCategory);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var category = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _categoryRepository.GetById(id)
                );

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"category:{id}", JsonConvert.SerializeObject(category), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return category;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the category by ID.", ex);
            }
        }

        public async Task<Category> AddCategory(CategoryModel categoryModel)
        {
            try
            {
                var category = new Category
                {
                    CategoryId = "CAT" + GenerateId.GenerateRandomId(4),
                    CategoryName = categoryModel.CategoryName,
                    Description = categoryModel.Description,
                    Status = categoryModel.Status,
                };

                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _categoryRepository.Add(category)
                );

                if (_redisConnection != null || _redisConnection.IsConnecting)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"category:{category.CategoryId}", JsonConvert.SerializeObject(category), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return category;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the category.", ex);
            }
        }

        public async Task<Category> UpdateCategory(CategoryModel categoryModel, string id)
        {
            try
            {
                var category = await _categoryRepository.GetById(id);
                if (category == null)
                {
                    throw new Exception("Category not found");
                }
                category.CategoryName = categoryModel.CategoryName;
                category.Description = categoryModel.Description;
                category.Status = categoryModel.Status;

                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _categoryRepository.Update(category)
                );

                await _redisPolicyWrap.ExecuteAsync(async () =>
                     await _redisDb.StringSetAsync($"category:{id}", JsonConvert.SerializeObject(category), TimeSpan.FromHours(1))
                );

                return category;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        public async Task DeleteCategory(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _categoryRepository.Delete(id)
                );

                Category category = await _dbPolicyWrap.ExecuteAsync(async () =>
                     await GetCategoryById(id)
                );

                await _redisPolicyWrap.ExecuteAsync(async () =>
                    await _redisDb.StringSetAsync($"category:{id}", JsonConvert.SerializeObject(category), TimeSpan.FromHours(1))
                );
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the category.", ex);
            }
        }
    }
}
