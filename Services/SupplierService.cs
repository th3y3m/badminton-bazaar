using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SupplierService
    {
        private readonly SupplierRepository _supplierRepository;

        public SupplierService(SupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public PaginatedList<Supplier> GetPaginatedSuppliers(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            var source = _supplierRepository.GetDbSet().AsNoTracking();

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

        public Supplier GetSupplierById(string id) => _supplierRepository.GetById(id);

        public Supplier AddSupplier(SupplierModel supplierModel)
        {
            var supplier = new Supplier
            {
                SupplierId = "SUP" + GenerateId.GenerateRandomId(5),
                CompanyName = supplierModel.CompanyName,
                Address = supplierModel.Address,
                Status = supplierModel.Status
            };
            _supplierRepository.Add(supplier);
            return supplier;
        }

        public Supplier UpdateSupplier(SupplierModel supplierModel, string id)
        {
            var supplier = _supplierRepository.GetById(id);
            supplier.CompanyName = supplierModel.CompanyName;
            supplier.Address = supplierModel.Address;
            supplier.Status = supplierModel.Status;
            _supplierRepository.Update(supplier);
            return supplier;
        }

        public void DeleteSupplier(string id)
        {
            _supplierRepository.Delete(id);
        }
    }
}
