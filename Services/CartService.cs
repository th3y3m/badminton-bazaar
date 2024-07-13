using Microsoft.AspNetCore.Http;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public void AddToCart(CartItem cartItem, string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            var cart = Session.GetObjectFromJson<List<CartItem>>(sessionKey) ?? new List<CartItem>();
            cart.Add(cartItem);
            Session.SetObjectAsJson(sessionKey, cart);
        }

        public List<CartItem> GetCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            return Session.GetObjectFromJson<List<CartItem>>(sessionKey) ?? new List<CartItem>();
        }

        public void SaveCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            var cart = Session.GetObjectFromJson<List<CartItem>>(sessionKey);
            if (cart != null)
            {
                string cartJson = JsonSerializer.Serialize(cart);
                string encodedCart = Convert.ToBase64String(Encoding.UTF8.GetBytes(cartJson));
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7), // Set expiry as needed
                    HttpOnly = true,
                    IsEssential = true
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append($"SavedCart_{userId}", encodedCart, options);
            }
        }

        public void ClearCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            Session.Remove(sessionKey);
        }

        private string GetCartSessionKey(string userId) => $"Cart_{userId}";
    }

}
