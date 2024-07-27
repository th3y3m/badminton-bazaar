using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task Add(OrderDetail orderDetail);
        Task Update(OrderDetail orderDetail);
        Task<OrderDetail> GetById(string id);
        Task<List<OrderDetail>> GetAll();
        Task<DbSet<OrderDetail>> GetDbSet();
        Task Delete(string id);
        Task<OrderDetail> GetByProductId(string id);
        Task<OrderDetail> GetByOrderId(string id);
    }
}
