using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<Discount> GetByIdAsync(string discountId);
        Task<List<Discount>> GetAllAsync();
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(Discount discount);
    }
}
