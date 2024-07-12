using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ColorRepository
    {
        private readonly DbContext _dbContext;

        public ColorRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Color color)
        {
            _dbContext.Colors.Add(color);
            _dbContext.SaveChanges();
        }

        public void Update(Color color)
        {
            _dbContext.Colors.Update(color);
            _dbContext.SaveChanges();
        }

        public Color GetById(string id) => _dbContext.Colors.Find(id);

        public List<Color> GetAll()
        {
            return _dbContext.Colors.ToList();
        }

        public DbSet<Color> GetDbSet()
        {
            return _dbContext.Colors;
        }

        public void Delete(string id)
        {
            var color = GetById(id);
            _dbContext.Colors.Remove(color);
            _dbContext.SaveChanges();
        }


    }
}
