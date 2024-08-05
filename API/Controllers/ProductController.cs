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
            try
            {
                var paginatedProducts = await _productService.GetPaginatedProducts(searchQuery, start, end, sortBy, status, supplierId, categoryId, pageIndex, pageSize);
                return Ok(paginatedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated products: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving product by ID: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel productModel)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding product: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductModel productModel, [FromQuery] string productId)
        {
            try
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

                var product = await _productService.UpdateProduct(productModel, productId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating product: {ex.Message}");
            }

            //try
            //{
            //    var product = await _productService.UpdateProduct(productModel, productId);
            //    return Ok(product);
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"Error updating product: {ex.Message}");
            //}
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting product: {ex.Message}");
            }
        }

        [HttpGet("TopSeller/{numberOfProducts}")]
        public async Task<IActionResult> GetTopSeller(int numberOfProducts)
        {
            try
            {
                var products = await _productService.GetTopSeller(numberOfProducts);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving top sellers: {ex.Message}");
            }
        }

        [HttpGet("ProductRemaining/{id}")]
        public async Task<IActionResult> ProductRemaining(string id)
        {
            try
            {
                var products = await _productService.ProductRemaining(id);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving product remaining: {ex.Message}");
            }
        }

        [HttpGet("GetRelatedProduct/{productId}")]
        public async Task<IActionResult> GetRelatedProduct(string productId)
        {
            try
            {
                var products = await _productService.GetRelatedProduct(productId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving related products: {ex.Message}");
            }
        }
    }
}
