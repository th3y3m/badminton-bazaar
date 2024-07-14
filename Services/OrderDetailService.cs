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
    public class OrderDetailService
    {
        private readonly OrderDetailRepository _orderDetailRepository;

        public OrderDetailService(OrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public PaginatedList<OrderDetail> GetPaginatedOrderDetails(
            string productVariantId,
            string orderId,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            var source = _orderDetailRepository.GetDbSet().AsNoTracking();

            if (!string.IsNullOrEmpty(productVariantId))
            {
                source = source.Where(p => p.ProductVariantId == productVariantId);
            }
            if (!string.IsNullOrEmpty(orderId))
            {
                source = source.Where(p => p.OrderId == orderId);
            }

            source = sortBy switch
            {
                "totelprice_asc" => source.OrderBy(p => p.TotalPrice()),
                "totalprice_desc" => source.OrderByDescending(p => p.TotalPrice()),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<OrderDetail>(items, count, pageIndex, pageSize);
        }

        public List<OrderDetail> GetOrderDetail(string orderId)
        {
            return _orderDetailRepository.GetAll().Where(p => p.OrderId == orderId).ToList();
        }
    }
}
