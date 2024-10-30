using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        public CategoryService(ICategoryRepository categoryRepository, IConnectionMultiplexer redisConnection)
        {
            _categoryRepository = categoryRepository;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
        }

        public async Task<PaginatedList<Category>> GetPaginatedCategories(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving paginated categories.", ex);
            }
        }

        public async Task<Category> GetCategoryById(string id)
        {
            try
            {
                var cachedCategory = await _redisDb.StringGetAsync($"category:{id}");

                if (!cachedCategory.IsNullOrEmpty)
                    return JsonConvert.DeserializeObject<Category>(cachedCategory);
                var category = await _categoryRepository.GetById(id);
                await _redisDb.StringSetAsync($"category:{id}", JsonConvert.SerializeObject(category), TimeSpan.FromHours(1));
                return category;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the category by ID.", ex);
            }
        }

        public async Task<Category> AddCategory(CategoryModel categoryModel)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the category.", ex);
            }
        }

        public async Task<Category> UpdateCategory(CategoryModel categoryModel, string id)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        public async Task DeleteCategory(string id)
        {
            try
            {
                await _categoryRepository.Delete(id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the category.", ex);
            }
        }
    }
}
