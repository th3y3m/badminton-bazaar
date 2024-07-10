using BusinessObjects;
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
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public PaginatedList<Product> GetPaginatedProducts(
            string searchQuery,
            string sortBy,
            string status,
            string supplierId,
            string categoryId,
            int pageIndex,
            int pageSize)
        {
            var source = _productRepository.GetDbSet().AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.ProductName.Contains(searchQuery));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                source = source.Where(p => p.Status == status);
            }

            // Apply supplier filter
            if (!string.IsNullOrEmpty(supplierId))
            {
                source = source.Where(p => p.SupplierId == supplierId);
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(categoryId))
            {
                source = source.Where(p => p.CategoryId == categoryId);
            }

            // Apply sorting
            source = sortBy switch
            {
                "name_asc" => source.OrderBy(p => p.ProductName),
                "name_desc" => source.OrderByDescending(p => p.ProductName),
                "price_asc" => source.OrderBy(p => p.UnitPrice),
                "price_desc" => source.OrderByDescending(p => p.UnitPrice),
                _ => source
            };

            // Apply pagination
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Product>(items, count, pageIndex, pageSize);
        }
    }
}
