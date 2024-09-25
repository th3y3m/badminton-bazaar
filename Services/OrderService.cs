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
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICartService _cartService;
        private readonly IUserDetailService _userDetailService;
        private readonly IProductVariantService _productVariantService;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICartService cartService, IUserDetailService userDetailService, IProductVariantService productVariantService)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _cartService = cartService;
            _userDetailService = userDetailService;
            _productVariantService = productVariantService;
        }
        public async Task<decimal> TotalPrice(string orderId)
        {
            try
            {
                var getAllOrderDetails = await _orderDetailRepository.GetAll();
                var orderDetails = getAllOrderDetails.Where(p => p.OrderId == orderId).ToList();

                var order = await _orderRepository.GetById(orderId);

                decimal totalPrice = 0;

                if (order == null)
                {
                    return totalPrice;
                }

                totalPrice += order.Freight.GetValueOrDefault();

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

        //public async Task<decimal> TotalPrice(string orderId)
        //{
        //    try
        //    {
        //        var getAllOrderDetails = await _orderDetailRepository.GetAll();
        //        var orderDetails = getAllOrderDetails.Where(p => p.OrderId == orderId).ToList();

        //        var order = await _orderRepository.GetById(orderId);

        //        decimal totalPrice = 0;

        //        totalPrice += order.Freight;
        //        foreach (var orderDetail in orderDetails)
        //        {
        //            totalPrice += orderDetail.UnitPrice * orderDetail.Quantity;
        //        }

        //        return totalPrice;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while calculating the total price.", ex);
        //    }
        //}

        public async Task<PaginatedList<Order>> GetPaginatedOrders(
            DateOnly? start,
            DateOnly? end,
            string userId,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _orderRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(userId))
                {
                    source = source.Where(p => p.UserId == userId);
                }

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

        public async Task<Order> AddOrder(string userId, decimal freight, string address)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                List<CartItem> itemsInCart = await _cartService.GetCart(userId);

                if (itemsInCart == null || itemsInCart.Count == 0)
                {
                    throw new Exception("Cart is empty.");
                }

                UserDetail userDetail = await _userDetailService.GetUserById(userId);
                if (userDetail == null)
                {
                    throw new Exception($"User with ID {userId} not found.");
                }

                Order order = new Order
                {
                    OrderId = "O" + GenerateId.GenerateRandomId(5),
                    OrderDate = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone),
                    Status = "Pending",
                    UserId = userId,
                    Freight = freight,
                    ShipAddress = address,
                    ShippedDate = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date + TimeSpan.FromDays(7)
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

                    await _orderDetailRepository.Add(orderDetail);
                    var product = await _productVariantService.GetById(item.ItemId);
                    product.StockQuantity -= item.Quantity;
                    await _productVariantService.Update(product);
                }

                _cartService.DeleteCartInCookie(userId);

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

        public async Task AutomaticFailedOrder(string orderId)
        {
            try
            {
                var order = await _orderRepository.GetById(orderId);

                if (order != null && order.Status != "Completed" && order.Status != "Failed")
                {
                    order.Status = "Failed";

                    await _orderRepository.Update(order);
                    var orderDetails = await _orderDetailRepository.GetByOrderId(orderId);
                    foreach (var orderDetail in orderDetails)
                    {
                        var product = orderDetail.ProductVariantId;
                        var productDetail = await _productVariantService.GetById(product);
                        productDetail.StockQuantity += orderDetail.Quantity;
                        await _productVariantService.Update(productDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while automatically failing the order.", ex);
            }
           
        }
    }
}
