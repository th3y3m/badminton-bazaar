using BusinessObjects;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System.Text.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedProducts(
            [FromQuery] decimal? start,
            [FromQuery] decimal? end,
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] bool? status = true,
            [FromQuery] string supplierId = "",
            [FromQuery] string categoryId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedProducts = await _productService.GetPaginatedProducts(searchQuery,start, end, sortBy, status, supplierId, categoryId, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel productModel)
        {
            var file = productModel.ProductImageUrl;

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                var task = new FirebaseStorage("court-callers.appspot.com")
                    .Child("NewsImages")
                    .Child(fileName)
                    .PutAsync(stream);

                var downloadUrl = await task;
                productModel.ImageUrl = downloadUrl; // Directly assign the URL
            }

            var product = await _productService.AddProduct(productModel);
            return Ok(product);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductModel productModel, [FromQuery] string productId)
        {
            var product = await _productService.UpdateProduct(productModel, productId);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            await _productService.DeleteProduct(id);
            return Ok();
        }

        //GetTopSeller
        [HttpGet("TopSeller/{numberOfProducts}")]
        
        public async Task<IActionResult> GetTopSeller(int numberOfProducts)
        {
            var products = await _productService.GetTopSeller(numberOfProducts);
            return Ok(products);
        }

        [HttpGet("ProductRemaining/{id}")]
        public async Task<IActionResult> ProductRemaining(string id)
        {
            var products = await _productService.ProductRemaining(id);
            return Ok(products);
        }
    }
}
