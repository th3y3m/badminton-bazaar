using BusinessObjects;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserDetailService
    {
        Task<PaginatedList<UserDetail>> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize);
        Task<UserDetail> GetUserById(string id);

        Task AddUserDetail(UserDetail userDetail);

        Task UpdateUserDetail(UserDetailModel userDetail, string id);

        Task<UserDetail> GetUserByReview(string reviewId);
    }
}
