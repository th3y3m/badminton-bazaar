using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;

namespace Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<PaginatedList<Supplier>> GetPaginatedSuppliers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _supplierRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.CompanyName.ToLower().Contains(searchQuery.ToLower()));
                }

                // Apply status filter
                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                // Apply sorting
                source = sortBy switch
                {
                    "companyname_asc" => source.OrderBy(p => p.CompanyName),
                    "companyname_desc" => source.OrderByDescending(p => p.CompanyName),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<Supplier>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated suppliers: {ex.Message}");
            }
        }

        public async Task<Supplier> GetSupplierById(string id)
        {
            try
            {
                return await _supplierRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier by ID: {ex.Message}");
            }
        }

        public async Task<Supplier> AddSupplier(SupplierModel supplierModel)
        {
            try
            {
                var supplier = new Supplier
                {
                    SupplierId = "SUP" + GenerateId.GenerateRandomId(5),
                    CompanyName = supplierModel.CompanyName,
                    Address = supplierModel.Address,
                    Status = supplierModel.Status
                };
                await _supplierRepository.Add(supplier);
                return supplier;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding supplier: {ex.Message}");
            }
        }

        public async Task<Supplier?> UpdateSupplier(SupplierModel supplierModel, string id)
        {
            try
            {
                var supplier = await _supplierRepository.GetById(id);

                if (supplier == null)
                {
                    return null;
                }
                supplier.CompanyName = supplierModel.CompanyName;
                supplier.Address = supplierModel.Address;
                supplier.Status = supplierModel.Status;
                await _supplierRepository.Update(supplier);
                return supplier;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating supplier: {ex.Message}");
            }
        }

        public async Task DeleteSupplier(string id)
        {
            try
            {
                await _supplierRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting supplier: {ex.Message}");
            }
        }
    }
}
