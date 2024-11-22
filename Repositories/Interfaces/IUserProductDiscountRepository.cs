using BusinessObjects;

namespace Repositories.Interfaces
{
    public interface IUserProductDiscountRepository
    {
        Task<UserProductDiscount> AddAsync(UserProductDiscount userProductDiscount);
        Task<UserProductDiscount> UpdateAsync(UserProductDiscount userProductDiscount);
        Task<UserProductDiscount> DeleteAsync(UserProductDiscount userProductDiscount);
        Task<UserProductDiscount> GetByIdAsync(string id);
        Task<List<UserProductDiscount>> GetAllAsync();
        Task<List<UserProductDiscount>> GetByIdAsync(string userId, string productId);
    }
}
