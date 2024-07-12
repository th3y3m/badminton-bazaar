using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;

namespace Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public PaginatedList<IdentityUser> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            var source = _userRepository.GetDbSet().AsNoTracking();

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

        public IdentityUser GetUserById(string id) => _userRepository.GetById(id);

        public IdentityUser AddUser(IdentityUser user)
        {
            _userRepository.Add(user);
            return user;
        }

        public IdentityUser UpdateUser(IdentityUser user, string userId)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.Email = user.Email;
            existingUser.UserName = user.UserName;
            existingUser.PhoneNumber = user.PhoneNumber;

            _userRepository.Update(existingUser);
            return existingUser;
        }

        public void DeleteUser(string id) => _userRepository.Delete(id);

        public void GetAllUsers() => _userRepository.GetAll();

        public void BanUser(string id) => _userRepository.Ban(id);

        public void UnbanUser(string id) => _userRepository.Unban(id);
    }
}
