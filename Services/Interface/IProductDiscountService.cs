using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductDiscountService
    {
        Task<ProductDiscount> GetProductDiscountByIdAsync(string productDiscountId);
         Task<ProductDiscount> GetProductDiscountByIdAsync(string productId, string discountId);
         Task<List<ProductDiscount>> GetAllProductDiscountsAsync();
         Task AddProductDiscountAsync(ProductDiscount productDiscount);
         Task UpdateProductDiscountAsync(ProductDiscount productDiscount);
         Task DeleteProductDiscountAsync(ProductDiscount productDiscount);
    }
}
