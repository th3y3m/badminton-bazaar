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
    public interface ICategoryService
    {
        Task<PaginatedList<Category>> GetPaginatedCategories(
            string searchQuery,
            string sortBy,
            bool? status,
            int pageIndex,
            int pageSize);

        Task<Category> GetCategoryById(string id);

        Task<Category> AddCategory(CategoryModel categoryModel);

        Task<Category> UpdateCategory(CategoryModel categoryModel, string id);

        Task DeleteCategory(string id);
    }
}
