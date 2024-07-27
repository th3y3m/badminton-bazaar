using BusinessObjects;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetail>> GetOrderDetail(string orderId);

        Task<PaginatedList<OrderDetail>> GetPaginatedOrderDetails(
            string productVariantId,
            string orderId,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize);
    }
}
