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
    public interface IProductVariantService
    {
        Task<PaginatedList<ProductVariant>> GetPaginatedProductVariants(
            //string searchQuery,
            string sortBy,
            bool? status,
            string colorId,
            string sizeId,
            string productId,
            int pageIndex,
            int pageSize);
        Task<ProductVariant> Add(ProductVariantModel productVariantModel);
        Task Update(ProductVariantModel productVariantModel, string id);
        Task<ProductVariant> GetById(string id);
        Task DeleteById(string id);
        Task<List<ProductVariant>> GetAll();
        Task<bool> CheckStock(List<CartItem> productVariants);
        Task<bool> CheckStock(CartItem cartItem);
        Task Update(ProductVariant productVariantModel);
    }
}
