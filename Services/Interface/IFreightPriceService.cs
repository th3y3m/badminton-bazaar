using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services.Interface
{
    public interface IFreightPriceService
    {
        Task Add(FreightPrice freightPrice);
        Task Update(FreightPrice freightPrice);
        Task<FreightPrice> GetById(string id);
        Task<List<FreightPrice>> GetAll();
        Task Delete(string id);
        Task<decimal> GetPriceByDistance(decimal km);
    }
}
