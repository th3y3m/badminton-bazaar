using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IPriceFactorRepository
    {
        Task<PriceFactor> GetPriceFactorAsync(string priceFactorId);
        Task<List<PriceFactor>> GetAllPriceFactorsAsync();
        Task AddPriceFactorAsync(PriceFactor priceFactor);
        Task UpdatePriceFactorAsync(PriceFactor priceFactor);
        Task DeletePriceFactorAsync(string priceFactorId);
    }
}
