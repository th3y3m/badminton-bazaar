using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserDetailService
    {
        private readonly UserDetailRepository _userDetailRepository;

        public UserDetailService(UserDetailRepository userDetailRepository)
        {
            _userDetailRepository = userDetailRepository;
        }

        public PaginatedList<UserDetail> GetPaginatedUserDetails(int pageIndex, int pageSize)
        {
            var source = _dbContext.Products.AsNoTracking();
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<UserDetail>(items, count, pageIndex, pageSize);
        }
    }
}
