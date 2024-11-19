using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Interfaces;
using BusinessObjects;
using Services.Interface;
using Services.Helper;
using Services.Models;
using System.Data;

namespace Services
{
    public class BrowsingHistoryService : IBrowsingHistoryService
    {
        private readonly IBrowsingHistoryRepository _browsingHistoryRepository;

        public BrowsingHistoryService(IBrowsingHistoryRepository browsingHistoryRepository)
        {
            _browsingHistoryRepository = browsingHistoryRepository;
        }

        public async Task<List<BrowsingHistory>> Get()
        {
            try
            {
                return await _browsingHistoryRepository.Get();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BrowsingHistory>> GetUserHistories(string userId)
        {
            try
            {
                return await _browsingHistoryRepository.GetUserHistories(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task LogBrowsingEvent(string userId, string productId, string sessionId = null)
        {
            try
            {
                var browsingHistory = new BrowsingHistory
                {
                    BrowsingHistoryId = "BH" + GenerateId.GenerateRandomId(7),
                    UserId = userId,
                    ProductId = productId,
                    BrowsedAt = DateTime.UtcNow,
                    SessionId = sessionId
                };

                await _browsingHistoryRepository.Create(browsingHistory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<List<ProductRecommendation>> GetProductRecommendationList(string userId)
        //{
        //    List<BrowsingHistory> userBrowsingHistories = await _browsingHistoryRepository.GetUserHistories(userId);

        //    if (userBrowsingHistories.Count == 0)
        //    {
        //        return new List<ProductRecommendation>();
        //    }

        //    Dictionary<string, Product> userProducts = new Dictionary<string, Product>();

        //    var userProductMatrix = userBrowsingHistories
        //        .GroupBy(b => b.ProductId)
        //        .Select(g => new
        //        {
        //            ProductId = g.Key,
        //            BrowseCount = g.Count()
        //        })
        //        .OrderByDescending(b => b.BrowseCount)
        //        .ToList();

        //    Product mostViewedProduct = null;

        //    if (userProductMatrix.Any())
        //    {
        //        mostViewedProduct = await _productService.GetProductById(userProductMatrix.First().ProductId);

        //        if (!userProducts.ContainsKey(mostViewedProduct.ProductId))
        //        {
        //            userProducts.Add(mostViewedProduct.ProductId, mostViewedProduct);
        //        }
        //    }

        //    var recentBrowsingHistory = userBrowsingHistories
        //        .OrderByDescending(b => b.BrowsedAt)
        //        .FirstOrDefault();

        //    Product recentProduct = null;

        //    if (recentBrowsingHistory != null)
        //    {
        //        recentProduct = await _productService.GetProductById(recentBrowsingHistory.ProductId);

        //        if (!userProducts.ContainsKey(recentProduct.ProductId))
        //        {
        //            userProducts.Add(recentProduct.ProductId, recentProduct);
        //        }
        //    }

        //    var mlContext = new MLContext();

        //    var trainingData = userProductMatrix.Select(upm => new ProductInteraction
        //    {
        //        UserId = userId,
        //        ProductId = upm.ProductId,
        //        CategoryId = userProducts[upm.ProductId].CategoryId,
        //        SupplierId = userProducts[upm.ProductId].SupplierId,
        //        BasePrice = userProducts[upm.ProductId].BasePrice,
        //        ProductDescription = userProducts[upm.ProductId].ProductDescription,
        //        Gender = userProducts[upm.ProductId].Gender,
        //        ProductName = userProducts[upm.ProductId].ProductName,
        //        Status = userProducts[upm.ProductId].Status,
        //        Label = upm.BrowseCount
        //    });

        //    IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(trainingData);

        //    var pipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.UserId))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.ProductId)))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.CategoryId)))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.SupplierId)))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.BasePrice)))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.ProductDescription)))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.Gender)))
        //        .Append(mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInteraction.ProductName)))
        //        .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
        //            new MatrixFactorizationTrainer.Options
        //            {
        //                MatrixColumnIndexColumnName = nameof(ProductInteraction.ProductId),
        //                MatrixRowIndexColumnName = nameof(ProductInteraction.UserId),
        //                LabelColumnName = nameof(ProductInteraction.Label)
        //            }));

        //    var model = pipeline.Fit(trainingDataView);

        //    var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductInteraction, ProductScorePrediction>(model);

        //    var allProducts = await _productService.GetProducts();

        //    var recommendations = new List<ProductRecommendation>();

        //    foreach (var product in allProducts)
        //    {
        //        var prediction = predictionEngine.Predict(new ProductInteraction
        //        {
        //            UserId = userId,
        //            ProductId = product.ProductId,
        //            CategoryId = product.CategoryId,
        //            SupplierId = product.SupplierId,
        //            BasePrice = product.BasePrice,
        //            ProductDescription = product.ProductDescription,
        //            Gender = product.Gender,
        //            ProductName = product.ProductName,
        //            Status = product.Status
        //        });

        //        recommendations.Add(new ProductRecommendation
        //        {
        //            Product = product,
        //            Score = prediction.Score
        //        });
        //    }

        //    // Sort recommendations by Score in descending order
        //    recommendations = recommendations.OrderByDescending(r => r.Score).ToList();

        //    return recommendations;
        //}
    }
}
