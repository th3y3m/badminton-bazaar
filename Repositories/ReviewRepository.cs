using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ReviewRepository
    {
        private readonly DbContext _dbContext;

        public ReviewRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Review review)
        {
            _dbContext.Reviews.Add(review);
            _dbContext.SaveChanges();
        }

        public void Update(Review review)
        {
            _dbContext.Reviews.Update(review);
            _dbContext.SaveChanges();
        }

        public Review GetById(string id) => _dbContext.Reviews.Find(id);

        public List<Review> GetAll()
        {
            return _dbContext.Reviews.ToList();
        }
        
        public DbSet<Review> GetDbSet()
        {
            return _dbContext.Reviews;
        }

        public void Delete(string id) {
            var review = GetById(id);
            _dbContext.Reviews.Remove(review);
            _dbContext.SaveChanges();
        }
    }
}
