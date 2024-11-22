using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProductDiscountController : ControllerBase
    {
        private readonly IUserProductDiscountService _userProductDiscountService;

        public UserProductDiscountController(IUserProductDiscountService userProductDiscountService)
        {
            _userProductDiscountService = userProductDiscountService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserProductDiscountAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                var result = await _userProductDiscountService.AddUserProductDiscountAsync(userProductDiscount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProductDiscountUserProductDiscountAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                var result = await _userProductDiscountService.UpdateUserProductDiscountUserProductDiscountAsync(userProductDiscount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserProductDiscountAsync(UserProductDiscount userProductDiscount)
        {
            try
            {
                var result = await _userProductDiscountService.DeleteUserProductDiscountAsync(userProductDiscount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserProductDiscountAsync()
        {
            try
            {
                var result = await _userProductDiscountService.GetAllUserProductDiscountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProductDiscountByIdAsync(string id)
        {
            try
            {
                var result = await _userProductDiscountService.GetUserProductDiscountByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}/{productId}")]
        public async Task<IActionResult> GetUserProductDiscountByIdAsync(string userId, string productId)
        {
            try
            {
                var result = await _userProductDiscountService.GetUserProductDiscountByIdAsync(userId, productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
