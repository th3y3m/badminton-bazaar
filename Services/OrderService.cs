using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICartService _cartService;
        private readonly IUserDetailService _userDetailService;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICartService cartService, IUserDetailService userDetailService)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _cartService = cartService;
            _userDetailService = userDetailService;
        }

        public async Task<decimal> TotalPrice(string orderId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calculating the total price.", ex);
            }
        }

        public async Task<PaginatedList<Order>> GetPaginatedOrders(
            DateOnly? start,
            DateOnly? end,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _orderRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

                if (start.HasValue && end.HasValue)
                {
                    var startDate = start.Value.ToDateTime(new TimeOnly(0, 0));
                    var endDate = end.Value.ToDateTime(new TimeOnly(23, 59, 59));
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

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<Order>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving paginated orders.", ex);
            }
        }

        public async Task<Order> GetOrderById(string id)
        {
            try
            {
                return await _orderRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order by ID.", ex);
            }
        }

        public async Task<Order?> AddOrder(string userId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the order.", ex);
            }
        }

        public async Task CancelOrder(string orderId)
        {
            try
            {
                Order order = await _orderRepository.GetById(orderId);
                if (order != null)
                {
                    order.Status = "Cancelled";
                    await _orderRepository.Update(order);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while canceling the order.", ex);
            }
        }
    }
}
