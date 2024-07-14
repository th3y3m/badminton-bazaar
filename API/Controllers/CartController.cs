using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("Add")]
        public IActionResult AddToCart([FromBody] string productId, [FromQuery] string userId)
        {
            _cartService.AddToCart(productId, userId);
            return Ok(new { message = "Item added to cart" });
        }
        
        [HttpPost("DeleteUnitItem")]
        public IActionResult DeleteUnitItem([FromBody] string productId, [FromQuery] string userId)
        {
            _cartService.DeleteUnitItem(productId, userId);
            return Ok(new { message = "Item deleted" });
        }

        [HttpGet("GetCart")]
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

        [HttpPost("remove")]
        public IActionResult RemoveFromCart([FromBody] string productId, [FromQuery] string userId)
        {
            _cartService.RemoveFromCart(productId, userId);
            return Ok(new { message = "Item removed from cart" });
        }

        [HttpPost("update")]
        public IActionResult UpdateCart([FromBody] string productId, [FromQuery] int quantity, [FromQuery] string userId)
        {
            _cartService.UpgradeCart(userId, productId, quantity);
            return Ok(new { message = "Cart updated" });
        }

        [HttpPost("DeleteCookie")]
        public IActionResult DeleteCookie([FromQuery] string userId)
        {
            _cartService.DeleteCartInCookie(userId);
            return Ok(new { message = "Cookie deleted" });
        }


        [HttpGet("ItemsInCart")]
        public IActionResult NumberOfItemsInCart([FromQuery] string userId)
        {
            var cart = _cartService.NumberOfItemsInCart(userId);
            return Ok(cart);
        }
    }


}
