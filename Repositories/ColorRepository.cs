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
    public class ColorRepository : IColorRepository
    {
        private readonly DbContext _dbContext;

        public ColorRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Color color)
        {
            _dbContext.Colors.Add(color);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Color color)
        {
            _dbContext.Colors.Update(color);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Color> GetById(string id) => await _dbContext.Colors.FindAsync(id);

        public async Task<List<Color>> GetAll()
        {
            return await _dbContext.Colors.ToListAsync();
        }

        public async Task<DbSet<Color>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Colors);
        }

        public async Task Delete(string id)
        {
            var color = await GetById(id);
            _dbContext.Colors.Remove(color);
            await _dbContext.SaveChangesAsync();
        }
    }
}
