using BusinessObjects;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
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

        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedProducts(
            [FromQuery] string sortBy = "price_asc",
            [FromQuery] bool? status = true,
            [FromQuery] string colorId = "",
            [FromQuery] string sizeId = "",
            [FromQuery] string productId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedProducts = await _productVariantService.GetPaginatedProductVariants(sortBy, status, colorId, sizeId, productId, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productVariantService.GetById(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductVariant([FromBody] ProductVariantModel productVariantModel)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductVariantModel productVariant, string id)
        {
            await _productVariantService.Update(productVariant, id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productVariantService.DeleteById(id);
            return Ok();
        }
    }
}
