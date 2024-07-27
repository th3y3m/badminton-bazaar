using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private string GetCartSessionKey(string userId) => $"Cart_{userId}";
        //private ISession Session => _httpContextAccessor.HttpContext.Session;
        private readonly IProductVariantService _productVariantService;
        private readonly IProductService _productService;

        public CartService(IHttpContextAccessor httpContextAccessor, IProductVariantService productVariantService, IProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productVariantService = productVariantService;
            _productService = productService;
        }


        //public void AddToCart(string productId, string userId)
        //{
        //    var sessionKey = GetCartSessionKey(userId);
        //    var selectedProduct = _productVariantService.GetById(productId);
        //    var product = _productService.GetProductByProductVariantId(productId);

        //    CartItem item;
        //    Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);

        //    if (cartItems == null)
        //    {
        //        cartItems = new Dictionary<string, CartItem>();
        //    }

        //    if (!cartItems.TryGetValue(productId, out item))
        //    {
        //        item = new CartItem
        //        {
        //            ItemId = productId,
        //            ItemName = product.ProductName,
        //            Quantity = 1,
        //            UnitPrice = selectedProduct.Price
        //        };
        //        cartItems[productId] = item;
        //    }
        //    else
        //    {
        //        item.Quantity++;
        //    }

        //    Session.SetObjectAsJson(sessionKey, cartItems);

        //    // Logging for debugging
        //    var currentCartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);
        //    Console.WriteLine("Current Cart Items: " + JsonSerializer.Serialize(currentCartItems));
        //}


        public void DeleteUnitItem(string productId, string userId)
        {
            var savedCart = _httpContextAccessor.HttpContext?.Request.Cookies[$"Cart_{userId}"] ?? string.Empty;
            if (!string.IsNullOrEmpty(savedCart))
            {
                var cartItems = CartUtil.GetCartFromCookie(savedCart);
                if (cartItems.TryGetValue(productId, out CartItem item))
                {
                    item.Quantity--;
                    if (item.Quantity <= 0)
                    {
                        cartItems.Remove(productId); // Remove the item if quantity is zero or less
                    }
                }

                var strItemsInCart = CartUtil.ConvertCartToString(cartItems.Values.ToList());
                CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
            }
        }


        public void RemoveFromCart(string productId, string userId)
        {
            Dictionary<string, CartItem> cartItems = new Dictionary<string, CartItem>();
            var savedCart = _httpContextAccessor.HttpContext?.Request.Cookies[$"Cart_{userId}"] ?? string.Empty;

            if (!string.IsNullOrEmpty(savedCart))
            {
                cartItems = CartUtil.GetCartFromCookie(savedCart);
                cartItems.Remove(productId);
            }

            var strItemsInCart = CartUtil.ConvertCartToString(cartItems.Values.ToList());
            CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
        }

        //public List<CartItem> GetCart(string userId)
        //{
        //    var sessionKey = GetCartSessionKey(userId);
        //    var session = _httpContextAccessor.HttpContext?.Session;

        //    if (session == null)
        //    {
        //        throw new System.InvalidOperationException("Session is not available");
        //    }

        //    Dictionary<string, CartItem> cart = session.GetObjectFromJson<Dictionary<string, CartItem>>("Cart");

        //    if (cart == null)
        //    {
        //        var savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"SavedCart_{userId}"];
        //        if (!string.IsNullOrEmpty(savedCart))
        //        {
        //            cart = CartUtil.GetCartFromCookie(savedCart);
        //            if (cart != null)
        //            {
        //                session.SetObjectAsJson("Cart", cart);
        //            }
        //        }
        //    }

        //    return cart?.Values.ToList() ?? new List<CartItem>();
        //}

        //public void AddToCart(string productId, string userId)
        //{
        //    var sessionKey = GetCartSessionKey(userId);
        //    var session = _httpContextAccessor.HttpContext?.Session;

        //    if (session == null)
        //    {
        //        throw new System.InvalidOperationException("Session is not available");
        //    }

        //    var selectedProduct = _productVariantService.GetById(productId);
        //    var product = _productService.GetProductByProductVariantId(productId);

        //    if (selectedProduct == null || product == null)
        //    {
        //        throw new System.ArgumentException("Invalid product ID");
        //    }

        //    Dictionary<string, CartItem> cartItems = session.GetObjectFromJson<Dictionary<string, CartItem>>("Cart") ?? new Dictionary<string, CartItem>();

        //    if (!cartItems.TryGetValue(productId, out var item))
        //    {
        //        item = new CartItem
        //        {
        //            ItemId = productId,
        //            ItemName = product.ProductName,
        //            Quantity = 1,
        //            UnitPrice = selectedProduct.Price
        //        };
        //        cartItems[productId] = item;
        //    }
        //    else
        //    {
        //        item.Quantity++;
        //    }

        //    session.SetObjectAsJson("Cart", cartItems);

        //    // Logging for debugging
        //    var currentCartItems = session.GetObjectFromJson<Dictionary<string, CartItem>>("Cart");
        //    Console.WriteLine("Current Cart Items: " + JsonSerializer.Serialize(currentCartItems));
        //}

        public List<CartItem> GetCart(string userId)
        {
            var savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"Cart_{userId}"];
            if (!string.IsNullOrEmpty(savedCart))
            {
                var cart = CartUtil.GetCartFromCookie(savedCart);
                return cart.Values.ToList();
            }
            return new List<CartItem>();
        }

        //public void SaveCart(string userId)
        //{
        //    var sessionKey = GetCartSessionKey(userId);
        //    Dictionary<string, CartItem> cart = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);
        //    if (cart != null)
        //    {
        //        var strItemsInCart = CartUtil.ConvertCartToString(cart.Values.ToList());
        //        CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
        //    }
        //}

        public void DeleteCartInCookie(string userId)
        {
            CartUtil.DeleteCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, userId);
        }

        //public void ClearCart(string userId)
        //{
        //    CartUtil.DeleteCartToCookie
        //}

        //public void UpgradeCart(string userId, string productId, int quantity)
        //{
        //    var sessionKey = GetCartSessionKey(userId);
        //    Dictionary<string, CartItem> cartItems = Session.GetObjectFromJson<Dictionary<string, CartItem>>(sessionKey);
        //    CartItem item = null;

        //    if (cartItems != null)
        //    {
        //        var selectedProduct = _productVariantService.GetById(productId);
        //        item = cartItems.GetValueOrDefault(productId);
        //        if (item == null)
        //        {
        //            item = new CartItem
        //            {
        //                ItemId = productId,
        //                ItemName = selectedProduct.Product.ProductName,
        //                Quantity = quantity,
        //                UnitPrice = selectedProduct.Price
        //            };
        //            cartItems[productId] = item;
        //        }
        //        else
        //        {
        //            item.Quantity = quantity;
        //        }
        //        Session.SetObjectAsJson(sessionKey, cartItems);
        //    }

        //}

        public int NumberOfItemsInCart(string userId)
        {
            int count = 0;

            var savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"Cart_{userId}"];
            if (!string.IsNullOrEmpty(savedCart))
            {
                var cartItems = CartUtil.GetCartFromCookie(savedCart);
                // Directly sum up the quantities from cartItems
                count = cartItems.Values.Sum(item => item.Quantity);
            }

            return count;
        }


        public async Task SaveCartToCookie(string productId, string userId)
        {
            Dictionary<string, CartItem> cartItems = new Dictionary<string, CartItem>();
            CartItem? item; // Declare item as nullable
            var selectedProduct = await _productVariantService.GetById(productId);
            var product = await _productService.GetProductByProductVariantId(productId);

            var savedCart = _httpContextAccessor.HttpContext?.Request.Cookies[$"Cart_{userId}"] ?? string.Empty;

            if (!string.IsNullOrEmpty(savedCart))
            {
                cartItems = CartUtil.GetCartFromCookie(savedCart);
            }

            // Check if the item exists in the cart, add or update accordingly
            if (!cartItems.TryGetValue(productId, out item))
            {
                item = new CartItem
                {
                    ItemId = productId,
                    ItemName = product.ProductName,
                    Quantity = 1,
                    UnitPrice = selectedProduct.Price
                };
                cartItems[productId] = item;
            }
            else
            {
                item.Quantity++;
            }

            // Convert the updated cart to string and save it back to the cookie
            var strItemsInCart = CartUtil.ConvertCartToString(cartItems.Values.ToList());
            CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
        }
    }
}


