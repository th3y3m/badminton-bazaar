using BusinessObjects;
using Repositories.Interfaces;
using Services.Interface;

namespace Services
{
    public class PriceFactorService : IPriceFactorService
    {
        private readonly IPriceFactorRepository _priceFactorRepository;

        public PriceFactorService(IPriceFactorRepository priceFactorRepository)
        {
            _priceFactorRepository = priceFactorRepository;
        }

        public async Task<PriceFactor> GetPriceFactorAsync(string priceFactorId)
        {
            try
            {
                return await _priceFactorRepository.GetPriceFactorAsync(priceFactorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve record: {ex.Message}");
            }
        }

        public async Task<List<PriceFactor>> GetAllPriceFactorsAsync()
        {
            try
            {
                return await _priceFactorRepository.GetAllPriceFactorsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve records: {ex.Message}");
            }
        }

        public async Task AddPriceFactorAsync(PriceFactor priceFactor)
        {
            try
            {
                await _priceFactorRepository.AddPriceFactorAsync(priceFactor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't add record: {ex.Message}");
            }
        }

        public async Task UpdatePriceFactorAsync(PriceFactor priceFactor)
        {
            try
            {
                await _priceFactorRepository.UpdatePriceFactorAsync(priceFactor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't update record: {ex.Message}");
            }
        }

        public async Task DeletePriceFactorAsync(string priceFactorId)
        {
            try
            {
                await _priceFactorRepository.DeletePriceFactorAsync(priceFactorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete record: {ex.Message}");
            }
        }
    }
}
