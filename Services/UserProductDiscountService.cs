using BusinessObjects;
using Repositories.Interfaces;
using Services.Interface;

namespace Services
{
    public class UserProductDiscountService : IUserProductDiscountService
    {
        private readonly IUserProductDiscountRepository _userProductDiscountRepository;

        public UserProductDiscountService(IUserProductDiscountRepository userProductDiscountRepository)
        {
            _userProductDiscountRepository = userProductDiscountRepository;
        }

        public Task<UserProductDiscount> AddUserProductDiscountAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                return _userProductDiscountRepository.AddAsync(userProductDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<UserProductDiscount> DeleteUserProductDiscountAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                return _userProductDiscountRepository.DeleteAsync(userProductDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<UserProductDiscount>> GetAllUserProductDiscountAsync()
        {
            try
            {
                return _userProductDiscountRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<UserProductDiscount> GetUserProductDiscountByIdAsync(string id)
        {
            try
            {
                return _userProductDiscountRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<UserProductDiscount>> GetUserProductDiscountByIdAsync(string userId, string productId)
        {
            try
            {
                return _userProductDiscountRepository.GetByIdAsync(userId, productId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<UserProductDiscount> UpdateUserProductDiscountUserProductDiscountAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                return _userProductDiscountRepository.UpdateAsync(userProductDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
