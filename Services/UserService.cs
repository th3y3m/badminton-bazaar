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

        public PaginatedList<IdentityUser> GetPaginatedUsers(int pageIndex, int pageSize)
        {
            var source = _userRepository.GetDbSet().AsNoTracking();
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<IdentityUser>(items, count, pageIndex, pageSize);
        }
    }
}
