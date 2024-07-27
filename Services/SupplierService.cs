using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Supplier> GetSupplierById(string id) => await _supplierRepository.GetById(id);

        public async Task<Supplier> AddSupplier(SupplierModel supplierModel)
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

        public async Task<Supplier?> UpdateSupplier(SupplierModel supplierModel, string id)
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

        public async Task DeleteSupplier(string id)
        {
            await _supplierRepository.Delete(id);
        }
    }
}
