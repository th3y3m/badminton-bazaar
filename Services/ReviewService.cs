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
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public ReviewService(IReviewRepository reviewRepository, IConnectionMultiplexer redisConnection)
        {
            _reviewRepository = reviewRepository;
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

        public async Task<PaginatedList<Review>> GetPaginatedReviews(
            string searchQuery,
            string sortBy,
            string userId,
            string productId,
            int? rating,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () => await _reviewRepository.GetDbSet());

                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.ReviewText.ToLower().Contains(searchQuery.ToLower()));
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    source = source.Where(p => p.UserId == userId);
                }

                if (!string.IsNullOrEmpty(productId))
                {
                    source = source.Where(p => p.ProductId == productId);
                }

                if (rating.HasValue)
                {
                    source = source.Where(p => p.Rating == rating);
                }

                // Apply sorting
                source = sortBy switch
                {
                    "rating_asc" => source.OrderBy(p => p.Rating),
                    "rating_desc" => source.OrderByDescending(p => p.Rating),
                    "reviewdate_asc" => source.OrderBy(p => p.ReviewDate),
                    "reviewdate_desc" => source.OrderByDescending(p => p.ReviewDate),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<Review>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated reviews: {ex.Message}");
            }
        }

        public async Task<Review> GetReviewById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        var cachedReview = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringGetAsync($"review:{id}")
                        );

                        if (!cachedReview.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<Review>(cachedReview);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var review = await _dbPolicyWrap.ExecuteAsync(async () => await _reviewRepository.GetById(id));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                             await _redisDb.StringSetAsync($"review:{id}", JsonConvert.SerializeObject(review), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving review by ID: {ex.Message}");
            }
        }

        public async Task<Review> AddReview(ReviewModel reviewModel)
        {
            try
            {
                var review = new Review
                {
                    ReviewId = "R" + GenerateId.GenerateRandomId(5),
                    UserId = reviewModel.UserId,
                    ProductId = reviewModel.ProductId,
                    Rating = reviewModel.Rating,
                    ReviewText = reviewModel.ReviewText,
                    ReviewDate = DateTime.Now
                };

                await _dbPolicyWrap.ExecuteAsync(async () => await _reviewRepository.Add(review));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"review:{review.ReviewId}", JsonConvert.SerializeObject(review), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding review: {ex.Message}");
            }
        }

        public async Task<Review?> UpdateReview(ReviewModel reviewModel, string reviewId)
        {
            try
            {
                var review = await GetReviewById(reviewId);

                if (review == null)
                {
                    throw new Exception("Review not found");
                }

                review.UserId = reviewModel.UserId;
                review.ProductId = reviewModel.ProductId;
                review.Rating = reviewModel.Rating;
                review.ReviewText = reviewModel.ReviewText;
                review.ReviewDate = DateTime.Now;

                await _dbPolicyWrap.ExecuteAsync(async () => await _reviewRepository.Update(review));

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"review:{reviewId}", JsonConvert.SerializeObject(review), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating review: {ex.Message}");
            }
        }

        public async Task DeleteReview(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () => await _reviewRepository.Delete(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting review: {ex.Message}");
            }
        }

        public async Task<double?> GetAverageRating(string productId)
        {
            try
            {
                var reviews = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _reviewRepository.GetDbSet()
                );

                var averageRating = reviews.Where(r => r.ProductId == productId).Average(r => r.Rating);

                return averageRating == null ? 0 : averageRating;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving average rating: {ex.Message}");
            }
        }
    }
}
