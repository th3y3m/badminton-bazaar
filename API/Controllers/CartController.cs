using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("AddToCookie")]
        public async Task<IActionResult> SaveCartToCookie([FromQuery] string productId, [FromQuery] string userId)
        {
            try
            {
                await _cartService.SaveCartToCookie(productId, userId);
                return Ok(new { message = "Item added to cart" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("DeleteUnitItem")]
        public IActionResult DeleteUnitItem([FromQuery] string productId, [FromQuery] string userId)
        {
            try
            {
                _cartService.DeleteUnitItem(productId, userId);
                return Ok(new { message = "Item deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item from cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetCart")]
        public IActionResult GetCart([FromQuery] string userId = "")
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return StatusCode(500, "HttpContext is null");
                }

                var cart = _cartService.GetCart(userId);
                
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Remove")]
        public IActionResult RemoveFromCart([FromQuery] string productId, [FromQuery] string userId)
        {
            try
            {
                _cartService.RemoveFromCart(productId, userId);
                return Ok(new { message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("DeleteCookie")]
        public IActionResult DeleteCookie([FromQuery] string userId)
        {
            try
            {
                _cartService.DeleteCartInCookie(userId);
                return Ok(new { message = "Cookie deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cookie");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("ItemsInCart")]
        public IActionResult NumberOfItemsInCart([FromQuery] string userId)
        {
            try
            {
                var cart = _cartService.NumberOfItemsInCart(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving number of items in cart");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
