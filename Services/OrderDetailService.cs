using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<PaginatedList<OrderDetail>> GetPaginatedOrderDetails(
            string productVariantId,
            string orderId,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _orderDetailRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

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

        public async Task<List<OrderDetail>> GetOrderDetail(string orderId)
        {
            var dbSet = await _orderDetailRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

            source = source.Where(p => p.OrderId == orderId);

            return await source.ToListAsync();
        }
    }
}
