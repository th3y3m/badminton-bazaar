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
using Microsoft.ML.Trainers;
using Microsoft.ML;
using Repositories;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IBrowsingHistoryRepository _browsingHistoryRepository;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _policyWrap;

        public ProductService(IProductRepository productRepository, IProductVariantRepository productVariantRepository, IOrderDetailRepository orderDetailRepository, IConnectionMultiplexer redisConnection, IElasticsearchService elasticClient, IBrowsingHistoryRepository browsingHistoryRepository, IReviewRepository reviewRepository)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _orderDetailRepository = orderDetailRepository;
            _reviewRepository = reviewRepository;
            _browsingHistoryRepository = browsingHistoryRepository;
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

        public async Task<List<Product>> GetProducts()
        {
            try
            {
                return await _productRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products: {ex.Message}");
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

        //public async Task<List<ProductRecommendation>> GetCollaborativeFilteringRecommendations(string userId)
        //{
        //    try
        //    {
        //        List<BrowsingHistory> userBrowsingHistories = await _browsingHistoryRepository.GetUserHistories(userId);

        //        if (userBrowsingHistories.Count == 0)
        //        {
        //            return new List<ProductRecommendation>();
        //        }

        //        var userProductMatrix = userBrowsingHistories
        //            .GroupBy(b => b.ProductId)
        //            .Select(g => new ProductInteractionV2
        //            {
        //                UserId = userId,
        //                ProductId = g.Key,
        //                Label = g.Count() // Using view count as the label
        //            })
        //            .ToList();

        //        var mlContext = new MLContext();

        //        IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(userProductMatrix);

        //        var pipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteractionV2.UserId))
        //            .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteractionV2.ProductId)))
        //            .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
        //                new MatrixFactorizationTrainer.Options
        //                {
        //                    MatrixColumnIndexColumnName = nameof(ProductInteractionV2.ProductId),
        //                    MatrixRowIndexColumnName = nameof(ProductInteractionV2.UserId),
        //                    LabelColumnName = nameof(ProductInteractionV2.Label)
        //                }));

        //        var model = pipeline.Fit(trainingDataView);

        //        var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductInteractionV2, ProductScorePrediction>(model);

        //        var allProducts = await GetProducts();

        //        var recommendations = new List<ProductRecommendation>();

        //        foreach (var product in allProducts)
        //        {
        //            var prediction = predictionEngine.Predict(new ProductInteractionV2
        //            {
        //                UserId = userId,
        //                ProductId = product.ProductId
        //            });

        //            var score = float.IsNaN(prediction.Score) ? 0 : prediction.Score;

        //            recommendations.Add(new ProductRecommendation
        //            {
        //                Product = product,
        //                Score = score
        //            });
        //        }

        //        // Sort recommendations by Score in descending order
        //        recommendations = recommendations.OrderByDescending(r => r.Score).ToList();

        //        return recommendations;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while generating collaborative filtering recommendations.", ex);
        //    }
        //}

        //public async Task<List<ProductRecommendation>> GetContentBasedRecommendations(string userId)
        //{
        //    try
        //    {
        //        List<BrowsingHistory> userBrowsingHistories = await _browsingHistoryRepository.GetUserHistories(userId);

        //        if (userBrowsingHistories.Count == 0)
        //        {
        //            return new List<ProductRecommendation>();
        //        }

        //        var recentBrowsingHistory = userBrowsingHistories
        //            .OrderByDescending(b => b.BrowsedAt)
        //            .FirstOrDefault();

        //        if (recentBrowsingHistory == null)
        //        {
        //            return new List<ProductRecommendation>();
        //        }

        //        var recentProduct = await GetProductById(recentBrowsingHistory.ProductId);

        //        var allProducts = await GetProducts();

        //        var recommendations = allProducts
        //            .Where(p => p.CategoryId == recentProduct.CategoryId && p.SupplierId == recentProduct.SupplierId && p.Gender == recentProduct.Gender && p.ProductId != recentProduct.ProductId)
        //            .Select(p => new ProductRecommendation
        //            {
        //                Product = p,
        //                Score = 1 // Assign a default score for content-based recommendations
        //            })
        //            .ToList();

        //        return recommendations;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while generating content based recommendations.", ex);
        //    }
        //}

        //public async Task<List<ProductRecommendation>> GetProductRecommendations(string userId)
        //{
        //    try
        //    {
        //        var collaborativeRecommendations = await GetCollaborativeFilteringRecommendations(userId);
        //        var contentBasedRecommendations = await GetContentBasedRecommendations(userId);

        //        var combinedRecommendations = collaborativeRecommendations
        //            .Concat(contentBasedRecommendations)
        //            .GroupBy(r => r.Product.ProductId)
        //            .Select(g => new ProductRecommendation
        //            {
        //                Product = g.First().Product,
        //                Score = g.Max(r => r.Score) // Use the highest score from either approach
        //            })
        //            .OrderByDescending(r => r.Score)
        //            .ToList();

        //        return combinedRecommendations;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<List<ProductRecommendation>> PredictHybridRecommendationsByRating(string userId)
        {
            double alpha = 0.5; // Weight for collaborative filtering score
            var mlContext = new MLContext();

            // Load collaborative filtering data
            var ratings = (await _reviewRepository.GetAll())
                .Select(r => new UserProductRating
                {
                    UserId = r.UserId,
                    ProductId = r.ProductId,
                    Rating = r.Rating ?? 0 // Handle nullable ratings
                })
                .ToList();

            // Load data into IDataView
            IDataView dataView = mlContext.Data.LoadFromEnumerable(ratings);

            // Define pipeline: Transform UserId and ProductId to key types
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: nameof(UserProductRating.UserId),
                    outputColumnName: "UserIdKey")
                .Append(mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: nameof(UserProductRating.ProductId),
                    outputColumnName: "ProductIdKey"))
                .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
                    labelColumnName: nameof(UserProductRating.Rating),
                    matrixColumnIndexColumnName: "UserIdKey",
                    matrixRowIndexColumnName: "ProductIdKey"));

            // Train the model
            var model = pipeline.Fit(dataView);

            // Create prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<UserProductRating, RatingPrediction>(model);

            // Prepare hybrid recommendations
            var recommendations = new List<ProductRecommendation>();
            var products = await _productRepository.GetAll();
            var productVectors = GetProductFeatureVectors(products);

            foreach (var product in products)
            {
                // Collaborative Filtering Score
                var collaborativePrediction = predictionEngine.Predict(new UserProductRating
                {
                    UserId = userId,
                    ProductId = product.ProductId
                });
                var collaborativeScore = float.IsNaN(collaborativePrediction.Score) ? 0 : collaborativePrediction.Score;

                // Content-Based Filtering Score
                double contentScore = 0;
                if (productVectors.ContainsKey(product.ProductId))
                {
                    foreach (var otherProduct in products)
                    {
                        if (otherProduct.ProductId != product.ProductId)
                        {
                            var similarity = CalculateCosineSimilarity(
                                productVectors[product.ProductId],
                                productVectors[otherProduct.ProductId]);
                            contentScore += similarity;
                        }
                    }
                    contentScore /= products.Count - 1; // Average similarity
                }

                // Hybrid Score
                var hybridScore = alpha * collaborativeScore + (1 - alpha) * contentScore;

                // Add to recommendations
                recommendations.Add(new ProductRecommendation
                {
                    Product = product,
                    Score = (float)hybridScore
                });
            }

            // Sort by score descending
            return recommendations.OrderByDescending(r => r.Score).ToList();
        }

        public async Task<List<ProductRecommendation>> PredictRecommendationsByPersonalBrowsingHistory(string userId)
        {
            var userHistories = await _browsingHistoryRepository.GetUserHistories(userId);

            if (userHistories.Count == 0)
            {
                return new List<ProductRecommendation>();
            }

            var topBrowsedProducts = userHistories
                .GroupBy(b => b.ProductId)
                .Select(g => new BrowsingData
                {
                    UserId = userId,
                    ProductId = g.Key,
                    BrowseCount = g.Count()
                })
                .OrderByDescending(b => b.BrowseCount)
                .ToList();


            // Prepare hybrid recommendations
            var recommendations = new List<ProductRecommendation>();
            var products = await _productRepository.GetAll();
            var productVectors = GetProductFeatureVectors(products);

            foreach (var product in products)
            {
                double contentScore = 0;

                if (productVectors.ContainsKey(product.ProductId))
                {
                    // Calculate similarity to the top-browsed product
                    contentScore = CalculateCosineSimilarity(
                        productVectors[product.ProductId],
                        productVectors[topBrowsedProducts[0].ProductId]
                    );
                }

                // Add to recommendations
                recommendations.Add(new ProductRecommendation
                {
                    Product = product,
                    Score = (float)contentScore
                });
            }


            // Sort by score descending
            return recommendations.OrderByDescending(r => r.Score).ToList();
        }

        private Dictionary<string, float[]> GetProductFeatureVectors(IEnumerable<Product> products)
        {
            return products.ToDictionary(p => p.ProductId, p => new float[]
            {
                (float)p.BasePrice / 1000, // Normalize price to a scale
                p.Gender == "Unisex" ? 1 : 0,
                p.Gender == "Men" ? 1 : 0,
                p.Gender == "Women" ? 1 : 0,
                (float)p.CategoryId.GetHashCode() / int.MaxValue, // Scale hashed values
                (float)p.SupplierId.GetHashCode() / int.MaxValue
            });
        }


        private static double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
            double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));
            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
