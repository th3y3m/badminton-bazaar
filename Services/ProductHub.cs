using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductHub : Hub
    {
        public async Task UpdateProductStock(string productId, int newStockQuantity)
        {
            await Clients.All.SendAsync("ReceiveProductStockUpdate", productId, newStockQuantity);
        }
    }
}
