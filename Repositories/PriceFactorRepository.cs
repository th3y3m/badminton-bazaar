using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class PriceFactorRepository : IPriceFactorRepository
    {
        private readonly DbContext _dbContext;

        public PriceFactorRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PriceFactor> GetPriceFactorAsync(string priceFactorId)
        {
            try
            {
                return await _dbContext.PriceFactors.FindAsync(priceFactorId);
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
                return await _dbContext.PriceFactors.ToListAsync();
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
                _dbContext.PriceFactors.Add(priceFactor);
                await _dbContext.SaveChangesAsync();
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
                _dbContext.PriceFactors.Update(priceFactor);
                await _dbContext.SaveChangesAsync();
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
                var priceFactor = await _dbContext.PriceFactors.FindAsync(priceFactorId);
                _dbContext.PriceFactors.Remove(priceFactor);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete record: {ex.Message}");
            }
        }
    }
}
