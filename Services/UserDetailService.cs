using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserDetailService : IUserDetailService
    {
        private readonly IUserDetailRepository _userDetailRepository;

        public UserDetailService(IUserDetailRepository userDetailRepository)
        {
            _userDetailRepository = userDetailRepository;
        }

        public async Task<PaginatedList<UserDetail>> GetPaginatedUsers(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _userDetailRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

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

        public async Task<UserDetail> GetUserById(string id) => await _userDetailRepository.GetById(id);

        public async Task AddUserDetail(UserDetail userDetail) => await _userDetailRepository.Add(userDetail);

        public async Task UpdateUserDetail(UserDetailModel userDetail, string id) 
        {
            
            var user = await _userDetailRepository.GetById(id);

            if (user == null)
            {
                return;
            }

            user.FullName = userDetail.FullName;
            user.Address = userDetail.Address;
            user.ProfilePicture = userDetail.ProfilePicture;
            await _userDetailRepository.Update(user);
        }
    }
}
