using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SizeRepository
    {
        private readonly DbContext _dbContext;

        public SizeRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void Add(Size size)
        {
            _dbContext.Sizes.Add(size);
            _dbContext.SaveChanges();
        }

        public void Update(Size size)
        {
            _dbContext.Sizes.Update(size);
            _dbContext.SaveChanges();
        }

        public Size GetById(string id) => _dbContext.Sizes.Find(id);

        public List<Size> GetAll()
        {
            return _dbContext.Sizes.ToList();
        }

        public DbSet<Size> GetDbSet()
        {
            return _dbContext.Sizes;
        }

        public void Delete(string id)
        {
            var size = GetById(id);
            _dbContext.Sizes.Remove(size);
            _dbContext.SaveChanges();
        }
    }
}
