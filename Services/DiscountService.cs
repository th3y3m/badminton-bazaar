using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Services.Interface;

namespace Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<Discount> GetDiscountByIdAsync(string discountId)
        {
            try
            {
                return await _discountRepository.GetByIdAsync(discountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            try
            {
                return await _discountRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve records: {ex.Message}");
            }
        }

        public async Task AddDiscountAsync(Discount discount)
        {
            try
            {
                await _discountRepository.AddAsync(discount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't add record: {ex.Message}");
            }
        }

        public async Task UpdateDiscountAsync(Discount discount)
        {
            try
            {
                await _discountRepository.UpdateAsync(discount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't update record: {ex.Message}");
            }
        }

        public async Task DeleteDiscountAsync(Discount discount)
        {
            try
            {
                await _discountRepository.DeleteAsync(discount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete record: {ex.Message}");
            }
        }
    }
}
