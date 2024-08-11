using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories.Interfaces
{
    public interface IFreightPriceRepository
    {
        Task Add(FreightPrice freightPrice);
        Task Update(FreightPrice freightPrice);
        Task<FreightPrice> GetById(string id);
        Task<List<FreightPrice>> GetAll();
        Task Delete(string id);
    }
}
