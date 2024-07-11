using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
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
            string status,
            int pageIndex,
            int pageSize)
        {
            var source = _supplierRepository.GetDbSet().AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.CompanyName.Contains(searchQuery));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
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
    }
}
