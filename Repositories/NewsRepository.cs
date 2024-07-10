using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class NewsRepository
    {
        private readonly DbContext _dbContext;

        public NewsRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(News news)
        {
            _dbContext.News.Add(news);
            _dbContext.SaveChanges();
        }

        public void Update(News news)
        {
            _dbContext.News.Update(news);
            _dbContext.SaveChanges();
        }

        public News GetById(string id) => _dbContext.News.Find(id);

        public List<News> GetAll()
        {
            return _dbContext.News.ToList();
        }
        public DbSet<News> GetDbSet()
        {
            return _dbContext.News;
        }

        public void Delete(string id) {
            var news = GetById(id);
            news.Status = "Inactive";
            _dbContext.News.Update(news);
            _dbContext.SaveChanges();
        }
    }
}
