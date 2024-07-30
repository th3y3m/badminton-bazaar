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

        public async Task<List<IdentityUser>> GetAll()
        {
            try
            {
                return await _dbContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all users: {ex.Message}");
            }
        }

        public async Task<DbSet<IdentityUser>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Users);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving DbSet of users: {ex.Message}");
            }
        }

        public async Task<IdentityUser?> GetById(string id)
        {
            try
            {
                return await _dbContext.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by ID: {ex.Message}");
            }
        }

        public async Task Add(IdentityUser user)
        {
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}");
            }
        }

        public async Task Update(IdentityUser user)
        {
            try
            {
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var user = await GetById(id);
                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task Ban(string id)
        {
            try
            {
                var user = await GetById(id);
                if (user != null)
                {
                    user.LockoutEnabled = true;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error banning user: {ex.Message}");
            }
        }

        public async Task Unban(string id)
        {
            try
            {
                var user = await GetById(id);
                if (user != null)
                {
                    user.LockoutEnabled = false;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error unbanning user: {ex.Message}");
            }
        }
    }
}
