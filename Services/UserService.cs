using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using StackExchange.Redis;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        public UserService(IUserRepository userRepository, IConnectionMultiplexer redisConnection, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
            _userManager = userManager;
        }

        public async Task<PaginatedList<IdentityUser>> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                //var dbSet = await _userRepository.GetDbSet();
                var dbSet = _userManager.Users;
                var source = dbSet.AsNoTracking();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.Email.ToLower().Contains(searchQuery.ToLower()));
                }

                // Apply status filter
                if (status.HasValue)
                {
                    source = source.Where(p => p.LockoutEnabled == status);
                }

                // Apply sorting
                source = sortBy switch
                {
                    "email_asc" => source.OrderBy(p => p.Email),
                    "email_desc" => source.OrderByDescending(p => p.Email),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<IdentityUser>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated users: {ex.Message}");
            }
        }

        public async Task<IdentityUser> GetUserById(string id)
        {
            try
            {
                var cachedUser = await _redisDb.StringGetAsync($"user:{id}");

                if (!cachedUser.IsNullOrEmpty)
                    return JsonConvert.DeserializeObject<IdentityUser>(cachedUser);

                var user = _userManager.FindByIdAsync(id).Result;
                await _redisDb.StringSetAsync($"user:{id}", JsonConvert.SerializeObject(user), TimeSpan.FromHours(1));
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by ID: {ex.Message}");
            }
        }

        public async void UpdateUser(IdentityUser user)
        {
            try
            {
                await _userManager.UpdateAsync(user);

                await _redisDb.StringSetAsync($"user:{user.Id}", JsonConvert.SerializeObject(user), TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task<IdentityUser> AddUser(IdentityUser user)
        {
            try
            {
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user");
                }
                await _redisDb.StringSetAsync($"user:{user.Id}", JsonConvert.SerializeObject(user), TimeSpan.FromHours(1));

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}");
            }
        }

        public async Task<IdentityUser?> UpdateUser(IdentityUser user, string userId)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(userId);
                if (existingUser == null)
                {
                    return null;
                }

                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.PhoneNumber;

                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user");
                }
                await _redisDb.StringSetAsync($"user:{userId}", JsonConvert.SerializeObject(existingUser), TimeSpan.FromHours(1));
                return existingUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task DeleteUser(string id)
        {
            try
            {
                var result = await _userManager.DeleteAsync(await _userManager.FindByIdAsync(id));
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to delete user");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<List<IdentityUser>> GetAllUsers()
        {
            try
            {
                return await _userManager.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all users: {ex.Message}");
            }
        }

        public async Task BanUser(string id)
        {
            try
            {
                await _userRepository.Ban(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error banning user: {ex.Message}");
            }
        }

        public async Task UnbanUser(string id)
        {
            try
            {
                await _userRepository.Unban(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error unbanning user: {ex.Message}");
            }
        }

        public async Task<int> CountUsers()
        {
            try
            {
                List<IdentityUser> users = await _userManager.Users.ToListAsync();
                return users.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting users: {ex.Message}");
            }

        }
    }
}
