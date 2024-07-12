using BusinessObjects;
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
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public PaginatedList<Category> GetPaginatedCategories(
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

        public Category GetCategoryById(string id) => _categoryRepository.GetById(id);

        public Category AddCategory(CategoryModel categoryModel)
        {
            var category = new Category
            {
                CategoryId = "CAT" + GenerateId.GenerateRandomId(4),
                CategoryName = categoryModel.CategoryName,
                Description = categoryModel.Description,
                Status = categoryModel.Status,
            };
            _categoryRepository.Add(category);
            return category;
        }

        public Category UpdateCategory(CategoryModel categoryModel, string id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            category.CategoryName = categoryModel.CategoryName;
            category.Description = categoryModel.Description;
            category.Status = categoryModel.Status;
            _categoryRepository.Update(category);
            return category;
        }

        public void DeleteCategory(string id) => _categoryRepository.Delete(id);


    }
}
