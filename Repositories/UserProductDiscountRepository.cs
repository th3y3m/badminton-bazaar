using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class UserProductDiscountRepository : IUserProductDiscountRepository
    {
        private readonly DbContext _context;

        public UserProductDiscountRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<UserProductDiscount> AddAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                _context.UserProductDiscounts.Add(userProductDiscount);
                await _context.SaveChangesAsync();
                return userProductDiscount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserProductDiscount> UpdateAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                _context.UserProductDiscounts.Update(userProductDiscount);
                await _context.SaveChangesAsync();
                return userProductDiscount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserProductDiscount> DeleteAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                _context.UserProductDiscounts.Remove(userProductDiscount);
                await _context.SaveChangesAsync();
                return userProductDiscount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserProductDiscount> GetByIdAsync(string id)
        {
            try
            {
                return await _context.UserProductDiscounts.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserProductDiscount>> GetAllAsync()
        {
            try
            {
                return await _context.UserProductDiscounts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserProductDiscount>> GetByIdAsync(string userId, string productId)
        {
            try
            {
                return await _context.UserProductDiscounts.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
