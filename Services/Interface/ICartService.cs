using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICartService
    {
        Task SaveCartToCookie(string productId, string userId);
        void DeleteUnitItem(string productId, string userId);
        void RemoveFromCart(string productId, string userId);
        Task<List<CartItem>> GetCart(string userId);
        void DeleteCartInCookie(string userId);
        int NumberOfItemsInCart(string userId);
    }
}
