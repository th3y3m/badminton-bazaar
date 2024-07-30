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
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly DbContext _dbContext;

        public ProductVariantRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(ProductVariant productVariant)
        {
            try
            {
                _dbContext.ProductVariants.Add(productVariant);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product variant: {ex.Message}");
            }
        }

        public async Task Update(ProductVariant productVariant)
        {
            try
            {
                _dbContext.ProductVariants.Update(productVariant);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product variant: {ex.Message}");
            }
        }

        public async Task<ProductVariant> GetById(string id)
        {
            try
            {
                return await _dbContext.ProductVariants.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product variant by ID: {ex.Message}");
            }
        }

        public async Task<List<ProductVariant>> GetAll()
        {
            try
            {
                return await _dbContext.ProductVariants.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all product variants: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var productVariant = await GetById(id);
                _dbContext.ProductVariants.Remove(productVariant);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product variant: {ex.Message}");
            }
        }

        public async Task<DbSet<ProductVariant>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.ProductVariants);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving DbSet of product variants: {ex.Message}");
            }
        }
    }
}
