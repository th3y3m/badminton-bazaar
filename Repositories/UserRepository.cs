using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _dbContext;

        public UserRepository(DbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<IdentityUser>> GetAll() {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<DbSet<IdentityUser>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Users);
        }

        public async Task<IdentityUser?> GetById(string id) {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task Add(IdentityUser user) {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(IdentityUser user) {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(string id) {
            var user = await GetById(id);
            user.LockoutEnabled = false;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Ban(string id) {
            var user = await GetById(id);
            user.LockoutEnabled = false;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Unban(string id) {
            var user = await GetById(id);
            user.LockoutEnabled = true;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
