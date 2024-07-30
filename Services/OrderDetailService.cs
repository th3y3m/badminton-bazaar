using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
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
                    "totalprice_asc" => source.OrderBy(p => p.TotalPrice()),
                    "totalprice_desc" => source.OrderByDescending(p => p.TotalPrice()),
                    _ => source
                };

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<OrderDetail>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving paginated order details.", ex);
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetail(string orderId)
        {
            try
            {
                var dbSet = await _orderDetailRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

                source = source.Where(p => p.OrderId == orderId);

                return await source.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving order details by order ID.", ex);
            }
        }
    }
}
