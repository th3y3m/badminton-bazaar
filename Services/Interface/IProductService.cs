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
        Task<Product> AddProduct(ProductModel productModel);
        Task<Product> UpdateProduct(ProductModel productModel, string id);
        Task DeleteProduct(string id);
        Task<Product> GetProductByProductVariantId(string productVariantId);
        Task<int> ProductRemaining(string productId);
        Task<List<Product>> GetTopSeller(int n);
        Task<int> GetSelledProduct(string productId);
    }
}
