using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BusinessObjects;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDiscountController : ControllerBase
    {
        private readonly IProductDiscountService _productDiscountService;

        public ProductDiscountController(IProductDiscountService productDiscountService)
        {
            _productDiscountService = productDiscountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductDiscountsAsync()
        {
            try
            {
                var discounts = await _productDiscountService.GetAllProductDiscountsAsync();
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDiscountByIdAsync(string id)
        {
            try
            {
                var discounts = await _productDiscountService.GetProductDiscountByIdAsync(id);
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("{productId}/{discountId}")]
        public async Task<IActionResult> GetProductDiscountByIdAsync(string productId, string discountId)
        {
            try
            {
                var discount = await _productDiscountService.GetProductDiscountByIdAsync(productId, discountId);
                return Ok(discount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProductDiscountAsync([FromBody] ProductDiscount productDiscount)
        {
            try
            {
                await _productDiscountService.AddProductDiscountAsync(productDiscount);
                return Ok("Product discount added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductDiscountAsync([FromBody] ProductDiscount productDiscount)
        {
            try
            {
                await _productDiscountService.UpdateProductDiscountAsync(productDiscount);
                return Ok("Product discount updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductDiscountAsync([FromBody] ProductDiscount productDiscount)
        {
            try
            {
                await _productDiscountService.DeleteProductDiscountAsync(productDiscount);
                return Ok("Product discount deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
