using BusinessObjects;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using Services.Helper;
using Services.Models;
using System.Text.Json;

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
            var paginatedProducts = _productService.GetPaginatedProducts(searchQuery,start, end, sortBy, status, supplierId, categoryId, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(string id)
        {
            var product = _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] ProductModel productModel)
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

            var product = _productService.AddProduct(productModel);
            return Ok(product);
        }


        [HttpPut]
        public ActionResult<Product> UpdateProduct([FromBody] ProductModel productModel, [FromQuery] string productId)
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

        //GetTopSeller
        [HttpGet("topseller/{numberOfProducts}")]
        
        public ActionResult<List<Product>> GetTopSeller(int numberOfProducts)
        {
            var products = _productService.GetTopSeller(numberOfProducts);
            return Ok(products);
        }

    }
}
