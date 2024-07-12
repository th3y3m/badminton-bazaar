using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult GetPaginatedCategories(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "categoryname_asc",
            [FromQuery] bool? status = true,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedCategories = _categoryService.GetPaginatedCategories(searchQuery, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedCategories);
        }

        [HttpGet("{id}")]
        public ActionResult GetCategoryById(string id)
        {
            var category = _categoryService.GetCategoryById(id);
            return Ok(category);
        }

        [HttpPost]
        public ActionResult AddCategory([FromBody] CategoryModel categoryModel)
        {
            var category = _categoryService.AddCategory(categoryModel);
            return Ok(category);
        }

        [HttpPut]
        public ActionResult UpdateCategory([FromBody] CategoryModel categoryModel, [FromQuery] string categoryId)
        {
            var category = _categoryService.UpdateCategory(categoryModel, categoryId);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategoryById(string id)
        {
            _categoryService.DeleteCategory(id);
            return Ok();
        }


    }
}
