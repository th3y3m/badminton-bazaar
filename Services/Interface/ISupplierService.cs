using BusinessObjects;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISupplierService
    {
        Task<Supplier?> UpdateSupplier(SupplierModel supplierModel, string id);
        Task<Supplier> AddSupplier(SupplierModel supplierModel);
        Task<Supplier> GetSupplierById(string id);
        Task<PaginatedList<Supplier>> GetPaginatedSuppliers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize);
        Task DeleteSupplier(string id);
    }
}
