using BusinessObjects;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IOrderService
    {
        Task<decimal> TotalPrice(string orderId);
        Task<PaginatedList<Order>> GetPaginatedOrders(
                    DateOnly? start,
                    DateOnly? end,
                    string userId,
                    string sortBy,
                    string status,
                    int pageIndex,
                    int pageSize);
        Task<Order> GetOrderById(string id);
        Task<Order> AddOrder(string userId, decimal freight, string address);
        Task CancelOrder(string orderId);
        Task AutomaticFailedOrder(string orderId);
        Task<Order> GetLatestOrder(string userId);
    }
}
