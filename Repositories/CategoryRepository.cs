using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository
    {
        private readonly DbContext _dbContext;

        public CategoryRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
        }

        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
            _dbContext.SaveChanges();
        }

        public Category GetById(string id) => _dbContext.Categories.Find(id);

        public List<Category> GetAll()
        {
            return _dbContext.Categories.ToList();
        }

        public void Delete(string id) {
            var category = GetById(id);
            category.Status = "Inactive";
            _dbContext.Categories.Update(category);
            _dbContext.SaveChanges();
        }
    }
}
