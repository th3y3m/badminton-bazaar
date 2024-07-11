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
                source = source.Where(p => p.Email.Contains(searchQuery));
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
    }
}
