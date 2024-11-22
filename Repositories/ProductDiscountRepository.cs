using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Repositories.Interfaces;

namespace Repositories
{
    public class ProductDiscountRepository : IProductDiscountRepository
    {
        private readonly DbContext _context;

        public ProductDiscountRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<ProductDiscount> GetByIdAsync(string productDiscountId)
        {
            try
            {
                return await _context.ProductDiscounts.FindAsync(productDiscountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }
        
        public async Task<ProductDiscount> GetByIdAsync(string productId, string discountId)
        {
            try
            {
                return await _context.ProductDiscounts.FirstOrDefaultAsync(pd => pd.ProductId == productId && pd.DiscountId == discountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }

        public async Task<List<ProductDiscount>> GetAllAsync()
        {
            try
            {
                return await _context.ProductDiscounts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve records: {ex.Message}");
            }
        }

        public async Task AddAsync(ProductDiscount productDiscount)
        {
            try
            {
                _context.ProductDiscounts.Add(productDiscount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't add record: {ex.Message}");
            }
        }

        public async Task UpdateAsync(ProductDiscount productDiscount)
        {
            try
            {
                _context.ProductDiscounts.Update(productDiscount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't update record: {ex.Message}");
            }
        }

        public async Task DeleteAsync(ProductDiscount productDiscount)
        {
            try
            {
                _context.ProductDiscounts.Remove(productDiscount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete record: {ex.Message}");
            }
        }
    }
}
