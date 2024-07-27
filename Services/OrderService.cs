using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICartService _cartService;
        private readonly IUserDetailService _userDetailService;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _cartService = cartService;
        }

        public async Task<decimal> TotalPrice(string orderId)
        {
            var getAll = await _orderDetailRepository.GetAll();
            var orderDetails = getAll.Where(p => p.OrderId == orderId).ToList();
            decimal totalPrice = 0;

            foreach (var orderDetail in orderDetails)
            {
                totalPrice += orderDetail.UnitPrice * orderDetail.Quantity;
            }

            return totalPrice;
        }

        public async Task<PaginatedList<Order>> GetPaginatedOrders(
            DateOnly? start,
            DateOnly? end,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _orderRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

            // Apply search filter

            if (start.HasValue && end.HasValue)
            {
                // Convert start to DateTime at the beginning of the day
                var startDate = start.Value.ToDateTime(new TimeOnly(0, 0)); // 00:00 hours

                // Convert end to DateTime at the end of the day
                var endDate = end.Value.ToDateTime(new TimeOnly(23, 59, 59)); // 23:59:59 hours

                source = source.Where(p => p.OrderDate >= startDate && p.OrderDate <= endDate);
            }

            if (!string.IsNullOrEmpty(status))
            {
                source = source.Where(p => p.Status == status);
            }

            source = sortBy switch
            {
                "orderdate_asc" => source.OrderBy(p => p.OrderDate),
                "orderdate_desc" => source.OrderByDescending(p => p.OrderDate),
                "totalprice_asc" => source.OrderBy(p => TotalPrice(p.OrderId)),
                "totalprice_desc" => source.OrderByDescending(p => TotalPrice(p.OrderId)),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Order>(items, count, pageIndex, pageSize);
        }

        public async Task<Order> GetOrderById(string id)
        {
            return await _orderRepository.GetById(id);
        }

        public async Task<Order?> AddOrder(string userId)
        {
            List<CartItem> itemsInCart = _cartService.GetCart(userId);
            UserDetail userDetail = await _userDetailService.GetUserById(userId);

            if (itemsInCart == null || userDetail.Address == null)
            {
                return null;
            }

            decimal totalPrice = 0;

            Order order = new Order
            {
                OrderId = "O" + GenerateId.GenerateRandomId(5),
                OrderDate = DateTime.Now,
                Status = "Pending",
                UserId = userId,
                Freight = 0,
                ShipAddress = userDetail.Address
            };
            await _orderRepository.Add(order);

            foreach (var item in itemsInCart)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderDetailId = "OD" + GenerateId.GenerateRandomId(5),
                    OrderId = order.OrderId,
                    ProductVariantId = item.ItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                totalPrice += orderDetail.TotalPrice();
                await _orderDetailRepository.Add(orderDetail);
            }
            return order;
        }

        public async Task CancelOrder(string orderId)
        {
            Order order = await _orderRepository.GetById(orderId);
            if (order != null)
            {
                order.Status = "Cancelled";
                await _orderRepository.Update(order);
            }
        }
    }
}
