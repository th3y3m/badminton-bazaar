using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

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
            try
            {
                _dbContext.UserDetails.Add(userDetail);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user detail: {ex.Message}");
            }
        }

        public async Task Update(UserDetail userDetail)
        {
            try
            {
                _dbContext.UserDetails.Update(userDetail);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user detail: {ex.Message}");
            }
        }

        public async Task<UserDetail> GetById(string id)
        {
            try
            {
                return await _dbContext.UserDetails.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user detail by ID: {ex.Message}");
            }
        }

        public async Task<List<UserDetail>> GetAll()
        {
            try
            {
                return await _dbContext.UserDetails.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all user details: {ex.Message}");
            }
        }

        public async Task<DbSet<UserDetail>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.UserDetails);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving DbSet of user details: {ex.Message}");
            }
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}