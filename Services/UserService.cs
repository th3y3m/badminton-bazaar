using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
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

        public async Task<IdentityUser> GetUserById(string id) => await _userRepository.GetById(id);

        public async Task<IdentityUser> AddUser(IdentityUser user)
        {
            await _userRepository.Add(user);
            return user;
        }

        public async Task<IdentityUser?> UpdateUser(IdentityUser user, string userId)
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

        public async Task DeleteUser(string id) => await _userRepository.Delete(id);

        public async Task GetAllUsers() => await _userRepository.GetAll();

        public async Task BanUser(string id) => await _userRepository.Ban(id);

        public async Task UnbanUser(string id) => await _userRepository.Unban(id);
    }
}
