using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DbContext _dbContext;

        public ReviewRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Review review)
        {
            try
            {
                _dbContext.Reviews.Add(review);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding review: {ex.Message}");
            }
        }

        public async Task Update(Review review)
        {
            try
            {
                _dbContext.Reviews.Update(review);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating review: {ex.Message}");
            }
        }

        public async Task<Review> GetById(string id)
        {
            try
            {
                return await _dbContext.Reviews.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving review by ID: {ex.Message}");
            }
        }

        public async Task<List<Review>> GetAll()
        {
            try
            {
                return await _dbContext.Reviews.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all reviews: {ex.Message}");
            }
        }

        public async Task<DbSet<Review>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Reviews);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving DbSet of reviews: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var review = await GetById(id);
                _dbContext.Reviews.Remove(review);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting review: {ex.Message}");
            }
        }
    }
}
