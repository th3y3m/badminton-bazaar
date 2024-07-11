using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using Services.Helper;
using Services.Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<PaginatedList<Product>> GetPaginatedProducts(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] string status = "",
            [FromQuery] string supplierId = "",
            [FromQuery] string categoryId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedProducts = _productService.GetPaginatedProducts(searchQuery, sortBy, status, supplierId, categoryId, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(string id)
        {
            var product = _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> AddProduct([FromBody] ProductModel productModel)
        {
            var product = _productService.AddProduct(productModel);
            return Ok(product);
        }

        [HttpPut]
        public ActionResult<Product> UpdateProduct([FromBody] ProductModel productModel, [FromBody] string productId)
        {
            var product = _productService.UpdateProduct(productModel, productId);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProductById(string id)
        {
            _productService.DeleteProduct(id);
            return Ok();
        }


    }
}
