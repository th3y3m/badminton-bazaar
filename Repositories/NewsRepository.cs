using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
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
            try
            {
                _dbContext.News.Add(news);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the news.", ex);
            }
        }

        public async Task Update(News news)
        {
            try
            {
                _dbContext.News.Update(news);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the news.", ex);
            }
        }

        public async Task<News> GetById(string id)
        {
            try
            {
                return await _dbContext.News.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the news by ID.", ex);
            }
        }

        public async Task<List<News>> GetAll()
        {
            try
            {
                return await _dbContext.News.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all news.", ex);
            }
        }

        public async Task<DbSet<News>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.News);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the DbSet of news.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var news = await GetById(id);
                if (news != null)
                {
                    news.Status = false;
                    _dbContext.News.Update(news);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("News not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the news.", ex);
            }
        }
    }
}
