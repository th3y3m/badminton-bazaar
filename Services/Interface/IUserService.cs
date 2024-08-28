using Microsoft.AspNetCore.Identity;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<PaginatedList<IdentityUser>> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize);
        Task<IdentityUser?> UpdateUser(IdentityUser user, string userId);
        Task<IdentityUser> AddUser(IdentityUser user);
        Task<IdentityUser> GetUserById(string id);
        Task BanUser(string id);
        Task UnbanUser(string id);
        Task DeleteUser(string id);
        Task<List<IdentityUser>> GetAllUsers();
        Task<int> CountUsers();
    }
}
