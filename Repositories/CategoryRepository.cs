using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
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
            try
            {
                _dbContext.Categories.Add(category);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the category.", ex);
            }
        }

        public async Task Update(Category category)
        {
            try
            {
                _dbContext.Categories.Update(category);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        public async Task<Category> GetById(string id)
        {
            try
            {
                return await _dbContext.Categories.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the category by ID.", ex);
            }
        }

        public async Task<List<Category>> GetAll()
        {
            try
            {
                return await _dbContext.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving all categories.", ex);
            }
        }

        public async Task<DbSet<Category>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Categories);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the DbSet of categories.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var category = await GetById(id);
                if (category != null)
                {
                    category.Status = false;
                    _dbContext.Categories.Update(category);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Category not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the category.", ex);
            }
        }
    }
}
