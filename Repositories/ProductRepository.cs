using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbContext _dbContext;

        public ProductRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Product product)
        {
            try
            {
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product: {ex.Message}");
            }
        }

        public async Task Update(Product product)
        {
            try
            {
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}");
            }
        }

        public async Task<Product> GetById(string id)
        {
            try
            {
                return await _dbContext.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product by ID: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetAll()
        {
            try
            {
                return await _dbContext.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all products: {ex.Message}");
            }
        }

        public async Task<DbSet<Product>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Products);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product DbSet: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var product = await GetById(id);
                product.Status = false;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetProducts(string id)
        {
            try
            {
                return await _dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.ProductVariants)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products: {ex.Message}");
            }
        }
    }
}
