using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductDiscountRepository
    {
        Task<ProductDiscount> GetByIdAsync(string productDiscountId);
        Task<ProductDiscount> GetByIdAsync(string productId, string discountId);
        Task<List<ProductDiscount>> GetAllAsync();
        Task AddAsync(ProductDiscount productDiscount);
        Task UpdateAsync(ProductDiscount productDiscount);
        Task DeleteAsync(ProductDiscount productDiscount);
    }
}
