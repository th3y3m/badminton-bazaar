using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DbContext context, ILogger<UserRepository> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task<List<IdentityUser>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all users");
                return await _dbContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw new Exception($"Error retrieving all users: {ex.Message}");
            }
        }

        public async Task<DbSet<IdentityUser>> GetDbSet()
        {
            try
            {
                _logger.LogInformation("Retrieving DbSet of users");
                return await Task.FromResult(_dbContext.Users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DbSet of users");
                throw new Exception($"Error retrieving DbSet of users: {ex.Message}");
            }
        }

        public async Task<IdentityUser?> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Retrieving user by ID: {UserId}", id);
                return await _dbContext.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID: {UserId}", id);
                throw new Exception($"Error retrieving user by ID: {ex.Message}");
            }
        }

        public async Task Add(IdentityUser user)
        {
            try
            {
                _logger.LogInformation("Adding new user: {UserId}", user.Id);
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {UserId}", user.Id);
                throw new Exception($"Error adding user: {ex.Message}");
            }
        }

        public async Task Update(IdentityUser user)
        {
            try
            {
                _logger.LogInformation("Updating user: {UserId}", user.Id);
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting user by ID: {UserId}", id);
                var user = await GetById(id);
                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user by ID: {UserId}", id);
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task Ban(string id)
        {
            try
            {
                _logger.LogInformation("Banning user by ID: {UserId}", id);
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
                _logger.LogError(ex, "Error banning user by ID: {UserId}", id);
                throw new Exception($"Error banning user: {ex.Message}");
            }
        }

        public async Task Unban(string id)
        {
            try
            {
                _logger.LogInformation("Unbanning user by ID: {UserId}", id);
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
                _logger.LogError(ex, "Error unbanning user by ID: {UserId}", id);
                throw new Exception($"Error unbanning user: {ex.Message}");
            }
        }
    }
}
