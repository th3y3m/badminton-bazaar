using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using Services.Models;
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

        public PaginatedList<UserDetail> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            var source = _userDetailRepository.GetDbSet().AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.FullName.ToLower().Contains(searchQuery.ToLower()));
            }

            // Apply sorting
            source = sortBy switch
            {
                "name_asc" => source.OrderBy(p => p.FullName),
                "name_desc" => source.OrderByDescending(p => p.FullName),
                _ => source
            };

            // Apply pagination
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<UserDetail>(items, count, pageIndex, pageSize);
        }

        public UserDetail GetUserById(string id) => _userDetailRepository.GetById(id);

        public void AddUserDetail(UserDetail userDetail) => _userDetailRepository.Add(userDetail);

        public void UpdateUserDetail(UserDetailModel userDetail, string id) 
        {
            
            var user = _userDetailRepository.GetById(id);
            user.FullName = userDetail.FullName;
            user.Address = userDetail.Address;
            user.ProfilePicture = userDetail.ProfilePicture;
            _userDetailRepository.Update(user);
        }
    }
}
