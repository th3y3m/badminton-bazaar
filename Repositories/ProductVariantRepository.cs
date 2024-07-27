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
            _dbContext.ProductVariants.Add(productVariant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(ProductVariant productVariant)
        {
            _dbContext.ProductVariants.Update(productVariant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ProductVariant> GetById(string id) => await _dbContext.ProductVariants.FindAsync(id);

        public async Task<List<ProductVariant>> GetAll()
        {
            return await _dbContext.ProductVariants.ToListAsync();
        }

        public async Task Delete(string id)
        {
            var productVariant = await GetById(id);
            _dbContext.ProductVariants.Remove(productVariant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<DbSet<ProductVariant>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.ProductVariants);
        }
    }
}
