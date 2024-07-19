using BusinessObjects;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : Controller
    {
        private readonly ProductVariantService _productVariantService;

        public ProductVariantController(ProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public ActionResult<PaginatedList<ProductVariant>> GetPaginatedProducts(
            [FromQuery] string sortBy = "price_asc",
            [FromQuery] bool? status = true,
            [FromQuery] string colorId = "",
            [FromQuery] string sizeId = "",
            [FromQuery] string productId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedProducts = _productVariantService.GetPaginatedProductVariants(sortBy, status, colorId, sizeId, productId, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public ActionResult<ProductVariant> GetProductById(string id)
        {
            var product = _productVariantService.GetById(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductVariant>> AddProductVariant([FromBody] ProductVariantModel productVariantModel)
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
            var product = _productVariantService.Add(productVariantModel);
            return Ok(product);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct([FromBody] ProductVariantModel productVariant, string id)
        {
            _productVariantService.Update(productVariant, id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(string id)
        {
            _productVariantService.DeleteById(id);
            return Ok();
        }
    }
}
