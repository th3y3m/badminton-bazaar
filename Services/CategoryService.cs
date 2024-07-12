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
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public PaginatedList<Category> GetPaginatedOrders(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            var source = _categoryRepository.GetDbSet().AsNoTracking();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.CategoryName.ToLower().Contains(searchQuery.ToLower()));
            }

            if (status.HasValue)
            {
                source = source.Where(p => p.Status == status);
            }

            source = sortBy switch
            {
                "categoryname_asc" => source.OrderBy(p => p.CategoryName),
                "categoryname_desc" => source.OrderByDescending(p => p.CategoryName),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Category>(items, count, pageIndex, pageSize);
        }
    }
}
