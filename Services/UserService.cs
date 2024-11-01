using BusinessObjects;
using Microsoft.AspNetCore.Identity;
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
using StackExchange.Redis;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public UserService(IUserRepository userRepository, IConnectionMultiplexer redisConnection, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
            _userManager = userManager;
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

        public async Task<PaginatedList<IdentityUser>> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                //var dbSet = await _userRepository.GetDbSet();
                var dbSet = _userManager.Users;
                var source = dbSet.AsNoTracking();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.Email.ToLower().Contains(searchQuery.ToLower()));
                }

                // Apply status filter
                if (status.HasValue)
                {
                    source = source.Where(p => p.LockoutEnabled == status);
                }

                // Apply sorting
                source = sortBy switch
                {
                    "email_asc" => source.OrderBy(p => p.Email),
                    "email_desc" => source.OrderByDescending(p => p.Email),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<IdentityUser>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated users: {ex.Message}");
            }
        }

        public async Task<IdentityUser> GetUserById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        var cachedUser = await _redisPolicyWrap.ExecuteAsync(async () => await _redisDb.StringGetAsync($"user:{id}"));
                        if (!cachedUser.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<IdentityUser>(cachedUser);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var user = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userManager.FindByIdAsync(id)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"user:{id}", JsonConvert.SerializeObject(user), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by ID: {ex.Message}");
            }
        }

        public async void UpdateUser(IdentityUser user)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userManager.UpdateAsync(user)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"user:{user.Id}", JsonConvert.SerializeObject(user), TimeSpan.FromHours(1))
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
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task<IdentityUser> AddUser(IdentityUser user)
        {
            try
            {
                var result = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userManager.CreateAsync(user)
                );

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user");
                }

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"user:{user.Id}", JsonConvert.SerializeObject(user), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}");
            }
        }

        public async Task<IdentityUser?> UpdateUser(IdentityUser user, string userId)
        {
            try
            {
                var existingUser = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByIdAsync(userId));
                if (existingUser == null)
                {
                    return null;
                }

                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.PhoneNumber;

                var result = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.UpdateAsync(existingUser));
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user");
                }

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"user:{userId}", JsonConvert.SerializeObject(existingUser), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return existingUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task DeleteUser(string id)
        {
            try
            {
                var result = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userManager.DeleteAsync(await _userManager.FindByIdAsync(id))
                );

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to delete user");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<List<IdentityUser>> GetAllUsers()
        {
            try
            {
                return await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userManager.Users.ToListAsync()
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all users: {ex.Message}");
            }
        }

        public async Task BanUser(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userRepository.Ban(id)
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error banning user: {ex.Message}");
            }
        }

        public async Task UnbanUser(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () => await _userRepository.Unban(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error unbanning user: {ex.Message}");
            }
        }

        public async Task<int> CountUsers()
        {
            try
            {
                return await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userManager.Users.CountAsync()
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting users: {ex.Message}");
            }

        }
    }
}
