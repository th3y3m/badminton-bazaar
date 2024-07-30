using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;

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
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated users: {ex.Message}");
            }
        }

        public async Task<UserDetail> GetUserById(string id)
        {
            try
            {
                return await _userDetailRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by ID: {ex.Message}");
            }
        }

        public async Task AddUserDetail(UserDetail userDetail)
        {
            try
            {
                await _userDetailRepository.Add(userDetail);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user detail: {ex.Message}");
            }
        }

        public async Task UpdateUserDetail(UserDetailModel userDetail, string id)
        {
            try
            {
                var user = await _userDetailRepository.GetById(id);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.FullName = userDetail.FullName;
                user.Address = userDetail.Address;
                user.ProfilePicture = userDetail.ProfilePicture;
                await _userDetailRepository.Update(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user detail: {ex.Message}");
            }
        }
    }
}
