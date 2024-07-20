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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(CartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpPost("Add")]
        //public IActionResult AddToCart([FromQuery] string productId, [FromQuery] string userId)
        //{
        //    _cartService.AddToCart(productId, userId);
        //    //var session = _httpContextAccessor.HttpContext.Session;
        //    //session.SetObjectAsJson("Cart", _cartService.GetCart(userId));
        //    return Ok(new { message = "Item added to cart" });
        //}
        [HttpPost("AddToCookie")]
        public IActionResult SaveCartToCookie([FromQuery] string productId, [FromQuery] string userId)
        {
            _cartService.SaveCartToCookie(productId, userId);
            //var session = _httpContextAccessor.HttpContext.Session;
            //session.SetObjectAsJson("Cart", _cartService.GetCart(userId));
            return Ok(new { message = "Item added to cart" });
        }
        
        [HttpPost("DeleteUnitItem")]
        public IActionResult DeleteUnitItem([FromQuery] string productId, [FromQuery] string userId)
        {
            _cartService.DeleteUnitItem(productId, userId);
            return Ok(new { message = "Item deleted" });
        }

        [HttpGet("GetCart")]
        public IActionResult GetCart([FromQuery] string userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return StatusCode(500, "HttpContext is null");
            }

            var session = httpContext.Session;
            var cart = session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null)
            {
                cart = _cartService.GetCart(userId);
                session.SetObjectAsJson("Cart", cart);
            }
            return Ok(cart);
        }

        //[HttpPost("save")]
        //public IActionResult SaveCart([FromQuery] string userId)
        //{
        //    _cartService.SaveCart(userId);
        //    return Ok(new { message = "Cart saved to cookie" });
        //}

        //[HttpPost("clear")]
        //public IActionResult ClearCart([FromQuery] string userId)
        //{
        //    _cartService.ClearCart(userId);
        //    return Ok(new { message = "Cart cleared" });
        //}

        [HttpPost("remove")]
        public IActionResult RemoveFromCart([FromQuery] string productId, [FromQuery] string userId)
        {
            _cartService.RemoveFromCart(productId, userId);
            return Ok(new { message = "Item removed from cart" });
        }

        //[HttpPost("update")]
        //public IActionResult UpdateCart([FromBody] string productId, [FromQuery] int quantity, [FromQuery] string userId)
        //{
        //    _cartService.UpgradeCart(userId, productId, quantity);
        //    return Ok(new { message = "Cart updated" });
        //}

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
