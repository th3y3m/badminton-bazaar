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
    public class NewsRepository : INewsRepository
    {
        private readonly DbContext _dbContext;

        public NewsRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(News news)
        {
            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(News news)
        {
            _dbContext.News.Update(news);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<News> GetById(string id) => await _dbContext.News.FindAsync(id);

        public async Task<List<News>> GetAll()
        {
            return await _dbContext.News.ToListAsync();
        }
        public async Task<DbSet<News>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.News);
        }

        public async Task Delete(string id)
        {
            var news = await GetById(id);
            news.Status = false;
            _dbContext.News.Update(news);
            await _dbContext.SaveChangesAsync();
        }
    }
}
