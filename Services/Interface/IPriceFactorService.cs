using BusinessObjects;

namespace Services.Interface
{
    public interface IPriceFactorService
    {
        Task<PriceFactor> GetPriceFactorAsync(string priceFactorId);
        Task<List<PriceFactor>> GetAllPriceFactorsAsync();
        Task AddPriceFactorAsync(PriceFactor priceFactor);
        Task UpdatePriceFactorAsync(PriceFactor priceFactor);
        Task DeletePriceFactorAsync(string priceFactorId);
    }
}
