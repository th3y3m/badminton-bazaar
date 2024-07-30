using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class SizeRepository : ISizeRepository
    {
        private readonly DbContext _dbContext;

        public SizeRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Size size)
        {
            try
            {
                _dbContext.Sizes.Add(size);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding size: {ex.Message}");
            }
        }

        public async Task Update(Size size)
        {
            try
            {
                _dbContext.Sizes.Update(size);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating size: {ex.Message}");
            }
        }

        public async Task<Size> GetById(string id)
        {
            try
            {
                return await _dbContext.Sizes.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving size by ID: {ex.Message}");
            }
        }

        public async Task<List<Size>> GetAll()
        {
            try
            {
                return await _dbContext.Sizes.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all sizes: {ex.Message}");
            }
        }

        public async Task<DbSet<Size>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Sizes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving DbSet of sizes: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var size = await GetById(id);
                _dbContext.Sizes.Remove(size);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting size: {ex.Message}");
            }
        }
    }
}
