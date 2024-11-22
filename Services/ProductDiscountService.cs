using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Interfaces;
using BusinessObjects;
using Services.Interface;

namespace Services
{
    public class ProductDiscountService : IProductDiscountService
    {
        private readonly IProductDiscountRepository _productDiscountRepository;

        public ProductDiscountService(IProductDiscountRepository productDiscountRepository)
        {
            _productDiscountRepository = productDiscountRepository;
        }

        public async Task<ProductDiscount> GetProductDiscountByIdAsync(string productDiscountId)
        {
            try
            {
                return await _productDiscountRepository.GetByIdAsync(productDiscountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }

        public async Task<ProductDiscount> GetProductDiscountByIdAsync(string productId, string discountId)
        {
            try
            {
                return await _productDiscountRepository.GetByIdAsync(productId, discountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }

        public async Task<List<ProductDiscount>> GetAllProductDiscountsAsync()
        {
            try
            {
                return await _productDiscountRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve records: {ex.Message}");
            }
        }

        public async Task AddProductDiscountAsync(ProductDiscount productDiscount)
        {
            try
            {
                await _productDiscountRepository.AddAsync(productDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't add record: {ex.Message}");
            }
        }

        public async Task UpdateProductDiscountAsync(ProductDiscount productDiscount)
        {
            try
            {
                await _productDiscountRepository.UpdateAsync(productDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't update record: {ex.Message}");
            }
        }

        public async Task DeleteProductDiscountAsync(ProductDiscount productDiscount)
        {
            try
            {
                await _productDiscountRepository.DeleteAsync(productDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete record: {ex.Message}");
            }
        }
    }
}
