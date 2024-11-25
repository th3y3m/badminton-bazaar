using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(DbContext dbContext, ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Add(Product product)
        {
            try
            {
                _logger.LogInformation("Adding new product: {ProductId}", product.ProductId);
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product: {ProductId}", product.ProductId);
                throw new Exception($"Error adding product: {ex.Message}");
            }
        }

        public async Task Update(Product product)
        {
            try
            {
                _logger.LogInformation("Updating product: {ProductId}", product.ProductId);
                var existingProduct = await _dbContext.Products.FindAsync(product.ProductId);
                if (existingProduct != null)
                {
                    _dbContext.Entry(existingProduct).State = EntityState.Detached;
                }

                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductId}", product.ProductId);
                throw new Exception($"Error updating product: {ex.Message}");
            }
        }

        public async Task<Product> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Retrieving product by ID: {ProductId}", id);
                return await _dbContext.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product by ID: {ProductId}", id);
                throw new Exception($"Error retrieving product by ID: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all products");
                return await _dbContext.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                throw new Exception($"Error retrieving all products: {ex.Message}");
            }
        }

        public async Task<DbSet<Product>> GetDbSet()
        {
            try
            {
                _logger.LogInformation("Retrieving product DbSet");
                return await Task.FromResult(_dbContext.Products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product DbSet");
                throw new Exception($"Error retrieving product DbSet: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting product by ID: {ProductId}", id);
                var product = await GetById(id);
                product.Status = false;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product by ID: {ProductId}", id);
                throw new Exception($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetProducts(string id)
        {
            try
            {
                _logger.LogInformation("Retrieving products with related data");
                return await _dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.ProductVariants)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                throw new Exception($"Error retrieving products: {ex.Message}");
            }
        }
    }
}
