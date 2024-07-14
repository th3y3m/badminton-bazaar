using BusinessObjects;
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
        private string GetCartSessionKey(string userId) => $"Cart_{userId}";
        private readonly ProductVariantService _productVariantService;

        public CartService(IHttpContextAccessor httpContextAccessor, ProductVariantService productVariantService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productVariantService = productVariantService;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public void AddToCart(string productId, string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            var selectedProduct = _productVariantService.GetById(productId);
            CartItem item = null;
            Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);

            if (cartItems == null)
            {
                cartItems = new Dictionary<string, CartItem>();
                Session.SetObjectAsJson(sessionKey, cartItems);
            }

            item = cartItems.GetValueOrDefault(productId);
            if (item == null)
            {
                item = new CartItem
                {
                    ItemId = productId,
                    ItemName = selectedProduct.Product.ProductName,
                    Quantity = 1,
                    UnitPrice = selectedProduct.Price
                };
                cartItems[productId] = item;
            }
            else
            {
                item.Quantity++;
            }
            Session.SetObjectAsJson(sessionKey, cartItems);
        }

        public void DeleteUnitItem(string productId, string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            var selectedProduct = _productVariantService.GetById(productId);
            CartItem item = null;
            Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);

            if (cartItems == null)
            {
                cartItems = new Dictionary<string, CartItem>();
                Session.SetObjectAsJson(sessionKey, cartItems);
            }

            item = cartItems.GetValueOrDefault(productId);
            if (item != null)
            {
                item.Quantity --;
            }
            Session.SetObjectAsJson(sessionKey, cartItems);
        }

        public void RemoveFromCart(string productId, string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);
            if (cartItems != null)
            {
                cartItems.Remove(productId);
                Session.SetObjectAsJson(sessionKey, cartItems);
            }
        }

        public List<CartItem> GetCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            List<CartItem> cartItems = null;
            Dictionary<string, CartItem> cart = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);

            if (cart == null)
            {
                var savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"SavedCart_{userId}"];
                if (!string.IsNullOrEmpty(savedCart))
                {
                    cart = CartUtil.GetCartFromCookie(savedCart);
                    if (cartItems != null)
                    {
                        cartItems = cart.Values.ToList();
                        Session.SetObjectAsJson(sessionKey, cartItems);
                    }
                }
            }
            else
            {
                cartItems = cart.Values.ToList();
            }

            return cartItems;
        }

        public void SaveCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            Dictionary<string, CartItem> cart = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);
            if (cart != null)
            {
                var strItemsInCart = CartUtil.ConvertCartToString(cart.Values.ToList());
                CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
            }
        }

        public void DeleteCartInCookie(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            CartUtil.DeleteCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, userId);
        }

        public void ClearCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            Session.Remove(sessionKey);
        }

        public void UpgradeCart(string userId, string productId, int quantity)
        {
            var sessionKey = GetCartSessionKey(userId);
            Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);
            CartItem item = null;

            if (cartItems != null)
            {
                var selectedProduct = _productVariantService.GetById(productId);
                item = cartItems.GetValueOrDefault(productId);
                if (item == null)
                {
                    item = new CartItem
                    {
                        ItemId = productId,
                        ItemName = selectedProduct.Product.ProductName,
                        Quantity = quantity,
                        UnitPrice = selectedProduct.Price
                    };
                    cartItems[productId] = item;
                }
                else
                {
                    item.Quantity = quantity;
                }
                Session.SetObjectAsJson(sessionKey, cartItems);
            }

        }

        public int NumberOfItemsInCart(string userId)
        {
            var sessionKey = GetCartSessionKey(userId);
            List<CartItem> items = null;
            int count = 0;
            Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);

            if (cartItems == null)
            {
                var savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"SavedCart_{userId}"];
                if (!string.IsNullOrEmpty(savedCart))
                {
                    cartItems = CartUtil.GetCartFromCookie(savedCart);
                    if (cartItems != null)
                    {
                        items = cartItems.Values.ToList();
                        Session.SetObjectAsJson(sessionKey, cartItems);
                    }
                }
            }
            else
            {
                items = cartItems.Values.ToList();
            }

            foreach (var item in items)
            {
                count += item.Quantity;
            }

            return count;
        }
    }

}
