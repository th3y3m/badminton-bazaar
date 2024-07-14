using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly OrderDetailRepository _orderDetailRepository;
        private readonly CartService _cartService;
        private readonly UserDetailService _userDetailService;

        public OrderService(OrderRepository orderRepository, OrderDetailRepository orderDetailRepository, CartService cartService)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _cartService = cartService;
        }

        public decimal TotalPrice(string orderId)
        {
            var orderDetails = _orderDetailRepository.GetAll().Where(p => p.OrderId == orderId).ToList();
            decimal totalPrice = 0;

            foreach (var orderDetail in orderDetails)
            {
                totalPrice += orderDetail.UnitPrice * orderDetail.Quantity;
            }

            return totalPrice;
        }

        public PaginatedList<Order> GetPaginatedOrders(
            DateOnly? start,
            DateOnly? end,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            var source = _orderRepository.GetDbSet().AsNoTracking();

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

        public Order GetOrderById(string id)
        {
            return _orderRepository.GetById(id);
        }

        public Order AddOrder(string userId)
        {
            List<CartItem> itemsInCart = _cartService.GetCart(userId);
            UserDetail userDetail = _userDetailService.GetUserById(userId);

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
            _orderRepository.Add(order);

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
                _orderDetailRepository.Add(orderDetail);
            }
            return order;
        }

        public void CancelOrder(string orderId)
        {
            Order order = _orderRepository.GetById(orderId);
            if (order != null)
            {
                order.Status = "Cancelled";
                _orderRepository.Update(order);
            }
        }


    }
}
