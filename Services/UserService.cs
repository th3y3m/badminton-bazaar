using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                var dbSet = await _userRepository.GetDbSet();
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
                return await _userRepository.GetById(id);
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
                await _userRepository.Update(user);
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
                await _userRepository.Add(user);
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
                var existingUser = await _userRepository.GetById(userId);
                if (existingUser == null)
                {
                    return null;
                }

                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.PhoneNumber;

                await _userRepository.Update(existingUser);
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
                await _userRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task GetAllUsers()
        {
            try
            {
                await _userRepository.GetAll();
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
    }
}
