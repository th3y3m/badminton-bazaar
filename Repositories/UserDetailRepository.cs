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
    public class UserDetailRepository : IUserDetailRepository
    {
        private readonly DbContext _dbContext;

        public UserDetailRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(UserDetail userDetail)
        {
            _dbContext.UserDetails.Add(userDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(UserDetail userDetail)
        {
            _dbContext.UserDetails.Update(userDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserDetail> GetById(string id) => await _dbContext.UserDetails.FindAsync(id);

        public async Task<List<UserDetail>> GetAll()
        {
            return await _dbContext.UserDetails.ToListAsync();
        }
        
        public async Task<DbSet<UserDetail>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.UserDetails);
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
