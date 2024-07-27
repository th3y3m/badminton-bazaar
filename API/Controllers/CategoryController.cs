using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedCategories(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "categoryname_asc",
            [FromQuery] bool? status = true,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedCategories = await _categoryService.GetPaginatedCategories(searchQuery, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedCategories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetCategoryById(id);
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryModel categoryModel)
        {
            var category = await _categoryService.AddCategory(categoryModel);
            return Ok(category);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryModel categoryModel, [FromQuery] string categoryId)
        {
            var category = await _categoryService.UpdateCategory(categoryModel, categoryId);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryById(string id)
        {
            await _categoryService.DeleteCategory(id);
            return Ok();
        }


    }
}
