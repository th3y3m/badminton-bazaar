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
    public class ProductRepository : IProductRepository
    {
        private readonly DbContext _dbContext;

        public ProductRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> GetById(string id) => await _dbContext.Products.FindAsync(id);

        public async Task<List<Product>> GetAll()
        {
            return await _dbContext.Products.ToListAsync();
        }
        public async Task<DbSet<Product>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Products);
        }

        public async Task Delete(string id) {
            var product = await GetById(id);
            product.Status = false;
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}
