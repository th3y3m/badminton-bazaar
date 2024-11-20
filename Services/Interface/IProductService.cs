using BusinessObjects;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductService
    {
        Task<PaginatedList<Product>> GetPaginatedProducts(
            string searchQuery,
            decimal? start,
            decimal? end,
            string sortBy,
            bool? status,
            string supplierId,
            string categoryId,
            int pageIndex,
            int pageSize);
        Task<Product> GetProductById(string id);
        Task<List<Product>> GetProducts();
        Task<Product> AddProduct(ProductModel productModel);
        Task<Product> UpdateProduct(ProductModel productModel, string id);
        Task DeleteProduct(string id);
        Task<Product> GetProductByProductVariantId(string productVariantId);
        Task<int> ProductRemaining(string productId);
        Task<List<Product>> GetTopSeller(int n);
        Task<int> GetSelledProduct(string productId);
        Task<List<Product>> GetRelatedProduct(string productId);
        //Task<List<ProductRecommendation>> GetProductRecommendations(string userId);
        //Task<List<ProductRecommendation>> GetContentBasedRecommendations(string userId);
        //Task<List<ProductRecommendation>> GetCollaborativeFilteringRecommendations(string userId);
        Task<List<ProductRecommendation>> PredictHybridRecommendationsByRating(string userId);
        Task<List<ProductRecommendation>> PredictRecommendationsByPersonalBrowsingHistory(string userId);
        Task<List<ProductRecommendation>> PredictRecommendationsByPersonalLatestOrder(string userId);
    }
}
