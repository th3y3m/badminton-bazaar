using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
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

        public OrderService(OrderRepository orderRepository, OrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
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
    }
}
