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
    public class UserDetailService : IUserDetailService
    {
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IReviewService _reviewService;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public UserDetailService(IUserDetailRepository userDetailRepository, IReviewService reviewService, IConnectionMultiplexer redisConnection)
        {
            _userDetailRepository = userDetailRepository;
            _reviewService = reviewService;
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

        public async Task<PaginatedList<UserDetail>> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () => await _userDetailRepository.GetDbSet());

                var source = dbSet.AsNoTracking();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.FullName.ToLower().Contains(searchQuery.ToLower()));
                }

                // Apply sorting
                source = sortBy switch
                {
                    "name_asc" => source.OrderBy(p => p.FullName),
                    "name_desc" => source.OrderByDescending(p => p.FullName),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<UserDetail>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated users: {ex.Message}");
            }
        }

        public async Task<UserDetail> GetUserById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        var cachedUserDetail = await _redisPolicyWrap.ExecuteAsync(async () => await _redisDb.StringGetAsync($"userDetail:{id}"));
                        if (!cachedUserDetail.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<UserDetail>(cachedUserDetail);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var userDetail = await _dbPolicyWrap.ExecuteAsync(async () => await _userDetailRepository.GetById(id));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"userDetail:{id}", JsonConvert.SerializeObject(userDetail), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return userDetail;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by ID: {ex.Message}");
            }
        }

        public async Task AddUserDetail(UserDetail userDetail)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _userDetailRepository.Add(userDetail)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"userDetail:{userDetail.UserId}", JsonConvert.SerializeObject(userDetail), TimeSpan.FromHours(1))
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
                throw new Exception($"Error adding user detail: {ex.Message}");
            }
        }

        public async Task UpdateUserDetail(UserDetailModel userDetail, string id)
        {
            try
            {

                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userDetailRepository.GetById(id));

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.FullName = userDetail.FullName;
                user.Address = userDetail.Address;
                user.ProfilePicture = userDetail.ProfilePicture;
                await _dbPolicyWrap.ExecuteAsync(async () => await _userDetailRepository.Update(user));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"userDetail:{id}", JsonConvert.SerializeObject(userDetail), TimeSpan.FromHours(1))
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
                throw new Exception($"Error updating user detail: {ex.Message}");
            }
        }

        public async Task<UserDetail> GetUserByReview(string reviewId)
        {
            try
            {
                var review = await _reviewService.GetReviewById(reviewId);
                if (review == null)
                    {
                    throw new Exception("Review not found");
                }

                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userDetailRepository.GetById(review.UserId));
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by review ID: {ex.Message}");
            }
        }
    }
}
