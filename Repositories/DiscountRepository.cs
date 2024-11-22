using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DbContext _context;

        public DiscountRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Discount> GetByIdAsync(string discountId)
        {
            try
            {
                return await _context.Discounts.FindAsync(discountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }

        public async Task<List<Discount>> GetAllAsync()
        {
            try
            {
                return await _context.Discounts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve records: {ex.Message}");
            }
        }

        public async Task AddAsync(Discount discount)
        {
            try
            {

                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't add record: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Discount discount)
        {
            try
            {

                _context.Discounts.Update(discount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't update record: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Discount discount)
        {
            try
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete record: {ex.Message}");
            }
        }
    }
}
