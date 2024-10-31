using BusinessObjects;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services;
using Services.Helper;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : Controller
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductVariantController(IProductVariantService productVariantService, IHubContext<ProductHub> hubContext)
        {
            _productVariantService = productVariantService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetPaginatedProducts(
            [FromQuery] string sortBy = "price_asc",
            [FromQuery] bool? status = true,
            [FromQuery] string colorId = "",
            [FromQuery] string sizeId = "",
            [FromQuery] string productId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedProducts = await _productVariantService.GetPaginatedProductVariants(sortBy, status, colorId, sizeId, productId, pageIndex, pageSize);
                return Ok(paginatedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated products: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
                var product = await _productVariantService.GetById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving product by ID: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProductVariant([FromBody] ProductVariantModel productVariantModel)
        {
            try
            {
                var imageUrls = new List<string>();

                foreach (var file in productVariantModel.ProductImageUrl)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    using (var stream = file.OpenReadStream())
                    {
                        var task = new FirebaseStorage("court-callers.appspot.com")
                            .Child("BranchImage")
                            .Child(fileName)
                            .PutAsync(stream);

                        var downloadUrl = await task;
                        imageUrls.Add(downloadUrl);
                    }
                }
                var product = await _productVariantService.Add(productVariantModel);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding product variant: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductVariantModel productVariant, string id)
        {
            try
            {
                await _productVariantService.Update(productVariant, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating product variant: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                await _productVariantService.DeleteById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting product variant: {ex.Message}");
            }
        }

        [HttpGet("CheckStock")]
        public async Task<IActionResult> CheckStock([FromQuery] CartItem productId)
        {
            try
            {
                var stock = await _productVariantService.CheckStock(productId);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error checking stock: {ex.Message}");
            }
        }
    }
}
