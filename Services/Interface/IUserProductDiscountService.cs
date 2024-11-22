using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserProductDiscountService
    {
        Task<UserProductDiscount> AddUserProductDiscountAsync(UserProductDiscount userProductDiscount);
        Task<UserProductDiscount> UpdateUserProductDiscountUserProductDiscountAsync(UserProductDiscount userProductDiscount);
        Task<UserProductDiscount> DeleteUserProductDiscountAsync(UserProductDiscount userProductDiscount);
        Task<UserProductDiscount> GetUserProductDiscountByIdAsync(string id);
        Task<List<UserProductDiscount>> GetAllUserProductDiscountAsync();
        Task<List<UserProductDiscount>> GetUserProductDiscountByIdAsync(string userId, string productId);
    }
}
