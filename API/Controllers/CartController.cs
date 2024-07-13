using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartItem cartItem, [FromQuery] string userId)
        {
            _cartService.AddToCart(cartItem, userId);
            return Ok(new { message = "Item added to cart" });
        }

        [HttpGet]
        public IActionResult GetCart([FromQuery] string userId)
        {
            var cart = _cartService.GetCart(userId);
            return Ok(cart);
        }

        [HttpPost("save")]
        public IActionResult SaveCart([FromQuery] string userId)
        {
            _cartService.SaveCart(userId);
            return Ok(new { message = "Cart saved to cookie" });
        }

        [HttpPost("clear")]
        public IActionResult ClearCart([FromQuery] string userId)
        {
            _cartService.ClearCart(userId);
            return Ok(new { message = "Cart cleared" });
        }
    }


}
