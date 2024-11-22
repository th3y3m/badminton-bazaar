using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IDiscountService
    {
        Task<Discount> GetDiscountByIdAsync(string discountId);
        Task<List<Discount>> GetAllDiscountsAsync();
        Task AddDiscountAsync(Discount discount);
        Task UpdateDiscountAsync(Discount discount);
        Task DeleteDiscountAsync(Discount discount);
    }
}
