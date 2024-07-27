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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DbContext _dbContext;

        public CategoryRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Category category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Category> GetById(string id) => await _dbContext.Categories.FindAsync(id);

        public async Task<List<Category>> GetAll()
        {
            return await _dbContext.Categories.ToListAsync();
        }
        public async Task<DbSet<Category>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Categories);
        }
        public async Task Delete(string id)
        {
            var category = await GetById(id);
            category.Status = false;
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }
    }
}
