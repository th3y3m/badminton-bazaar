using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Review review)
        {
            _dbContext.Reviews.Update(review);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Review> GetById(string id) => await _dbContext.Reviews.FindAsync(id);

        public async Task<List<Review>> GetAll()
        {
            return await _dbContext.Reviews.ToListAsync();
        }
        
        public async Task<DbSet<Review>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Reviews);
        }

        public async Task Delete(string id) {
            var review = await GetById(id);
            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();
        }
    }
}
