using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task Add(Supplier supplier);
        Task Update(Supplier supplier);
        Task<Supplier> GetById(string id);
        Task<List<Supplier>> GetAll();
        Task<DbSet<Supplier>> GetDbSet();
        Task Delete(string id);
    }
}
