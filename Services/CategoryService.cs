using BusinessObjects;
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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PaginatedList<Category>> GetPaginatedCategories(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _categoryRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

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

            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<Category>(items, count, pageIndex, pageSize);
        }

        public async Task<Category> GetCategoryById(string id) => await _categoryRepository.GetById(id);

        public async Task<Category> AddCategory(CategoryModel categoryModel)
        {
            var category = new Category
            {
                CategoryId = "CAT" + GenerateId.GenerateRandomId(4),
                CategoryName = categoryModel.CategoryName,
                Description = categoryModel.Description,
                Status = categoryModel.Status,
            };
            await _categoryRepository.Add(category);
            return category;
        }

        public async Task<Category> UpdateCategory(CategoryModel categoryModel, string id)
        {
            var category = await _categoryRepository.GetById(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            category.CategoryName = categoryModel.CategoryName;
            category.Description = categoryModel.Description;
            category.Status = categoryModel.Status;
            await _categoryRepository.Update(category);
            return category;
        }

        public async Task DeleteCategory(string id) => await _categoryRepository.Delete(id);
    }
}
