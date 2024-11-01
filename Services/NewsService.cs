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
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public NewsService(INewsRepository newsRepository, IConnectionMultiplexer redisConnection)
        {
            _newsRepository = newsRepository;
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

        public async Task<PaginatedList<News>> GetPaginatedNews(
            string searchQuery,
            string sortBy,
            bool? IsHomepageBanner,
            bool? IsHomepageSlideShow,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _newsRepository.GetDbSet()
                );

                var source = dbSet.AsNoTracking();

                if (IsHomepageBanner.HasValue)
                {
                    source = source.Where(p => p.IsHomepageBanner == IsHomepageBanner);
                }
                if (IsHomepageSlideShow.HasValue)
                {
                    source = source.Where(p => p.IsHomepageSlideshow == IsHomepageSlideShow);
                }

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.Title.ToLower().Contains(searchQuery.ToLower()) || p.Content.ToLower().Contains(searchQuery.ToLower()));
                }

                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                source = sortBy switch
                {
                    "publicationdate_asc" => source.OrderBy(p => p.PublicationDate),
                    "publicationdate_desc" => source.OrderByDescending(p => p.PublicationDate),
                    "title_asc" => source.OrderBy(p => p.Title),
                    "title_desc" => source.OrderByDescending(p => p.Title),
                    "views_asc" => source.OrderBy(p => p.Views),
                    "views_desc" => source.OrderByDescending(p => p.Views),
                    _ => source
                };

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<News>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving paginated news.", ex);
            }
        }

        public async Task<News> GetNewsById(string id)
        {
            try
            {
                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    RedisValue cachedNews = RedisValue.Null;
                    try
                    {
                        cachedNews = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringGetAsync($"news:{id}")
                        );
                        if (!cachedNews.IsNullOrEmpty)
                            return JsonConvert.DeserializeObject<News>(cachedNews);
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var news = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _newsRepository.GetById(id)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    RedisValue cachedNews = RedisValue.Null;
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"news:{id}", JsonConvert.SerializeObject(news), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the news by ID.", ex);
            }
        }

        public async Task<News> AddNews(NewsModel newsModel)
        {
            try
            {
                var news = new News
                {
                    NewId = "N" + GenerateId.GenerateRandomId(5),
                    Title = newsModel.Title,
                    Content = newsModel.Content,
                    Image = newsModel.Image,
                    PublicationDate = DateTime.Now,
                    Views = 0,
                    IsHomepageSlideshow = newsModel.IsHomepageSlideshow,
                    IsHomepageBanner = newsModel.IsHomepageBanner,
                    Status = newsModel.Status
                };

                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _newsRepository.Add(news);
                });

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"news:{news.NewId}", JsonConvert.SerializeObject(news), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the news.", ex);
            }
        }

        public async Task<News> UpdateNews(string id, NewsModel newsModel)
        {
            try
            {
                var news = await _newsRepository.GetById(id);
                if (news == null)
                {
                    throw new Exception("News not found.");
                }

                news.Title = newsModel.Title;
                news.Content = newsModel.Content;
                news.Image = newsModel.Image;
                news.IsHomepageSlideshow = newsModel.IsHomepageSlideshow;
                news.IsHomepageBanner = newsModel.IsHomepageBanner;
                news.Status = newsModel.Status;

                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _newsRepository.Update(news);
                });

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"news:{id}", JsonConvert.SerializeObject(news), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the news.", ex);
            }
        }

        public async Task<News?> DeleteNews(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _newsRepository.Delete(id);
                });

                News news = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _newsRepository.GetById(id)
                );

                if (_redisConnection != null || _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"news:{id}", JsonConvert.SerializeObject(news), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
                return news;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the news.", ex);
            }
        }

        public async Task AddAViewUnit(string id)
        {
            try
            {
                var news = await GetNewsById(id);
                news.Views++;

                await _dbPolicyWrap.ExecuteAsync(async () =>
                {
                    await _newsRepository.Update(news);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding a view unit.", ex);
            }
        }

        public async Task<List<News>> GetSlideshowNews()
        {
            try
            {
                var news = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _newsRepository.GetAll()
                );

                return news.Where(p => p.IsHomepageSlideshow == true).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving slideshow news.", ex);
            }
        }

        public async Task<List<News>> GetBannerNews()
        {
            try
            {
                var news = await _dbPolicyWrap.ExecuteAsync(async () =>
                      await _newsRepository.GetAll()
                );
                return news.Where(p => p.IsHomepageBanner == true).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving banner news.", ex);
            }
        }
    }
}
