using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductVariantRepository
    {
        private readonly DbContext _dbContext;

        public ProductVariantRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ProductVariant productVariant)
        {
            _dbContext.ProductVariants.Add(productVariant);
            _dbContext.SaveChanges();
        }

        public void Update(ProductVariant productVariant)
        {
            _dbContext.ProductVariants.Update(productVariant);
            _dbContext.SaveChanges();
        }

        public ProductVariant GetById(string id) => _dbContext.ProductVariants.Find(id);

        public List<ProductVariant> GetAll()
        {
            return _dbContext.ProductVariants.ToList();
        }

        public void DeleteById(string id)
        {
            var productVariant = GetById(id);
            _dbContext.ProductVariants.Remove(productVariant);
            _dbContext.SaveChanges();
        }

        public DbSet<ProductVariant> GetDbSet()
        {
            return _dbContext.ProductVariants;
        }
    }
}
