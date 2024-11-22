using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BusinessObjects;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("{discountId}")]
        public async Task<IActionResult> GetDiscountByIdAsync(string discountId)
        {
            try
            {
                var discount = await _discountService.GetDiscountByIdAsync(discountId);
                return Ok(discount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscountsAsync()
        {
            try
            {
                var discounts = await _discountService.GetAllDiscountsAsync();
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscountAsync([FromBody] Discount discount)
        {
            try
            {
                await _discountService.AddDiscountAsync(discount);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDiscountAsync([FromBody] Discount discount)
        {
            try
            {
                await _discountService.UpdateDiscountAsync(discount);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDiscountAsync([FromBody] Discount discount)
        {
            try
            {
                await _discountService.DeleteDiscountAsync(discount);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
