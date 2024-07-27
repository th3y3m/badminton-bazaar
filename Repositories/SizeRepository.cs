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
    public class SizeRepository : ISizeRepository
    {
        private readonly DbContext _dbContext;

        public SizeRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Size size)
        {
            _dbContext.Sizes.Add(size);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Size size)
        {
            _dbContext.Sizes.Update(size);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Size> GetById(string id) => await _dbContext.Sizes.FindAsync(id);

        public async Task<List<Size>> GetAll()
        {
            return await _dbContext.Sizes.ToListAsync();
        }

        public async Task<DbSet<Size>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Sizes);
        }

        public async Task Delete(string id)
        {
            var size = await GetById(id);
            _dbContext.Sizes.Remove(size);
            await _dbContext.SaveChangesAsync();
        }
    }
}
