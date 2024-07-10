using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductRepository
    {
        private readonly DbContext _dbContext;

        public ProductRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Product product)
        {
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        public void Update(Product product)
        {
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
        }

        public Product GetById(string id) => _dbContext.Products.Find(id);

        public List<Product> GetAll()
        {
            return _dbContext.Products.ToList();
        }
        public DbSet<Product> GetDbSet()
        {
            return _dbContext.Products;
        }

        public void Delete(string id) {
            var product = GetById(id);
            product.Status = "Inactive";
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
        }
    }
}
