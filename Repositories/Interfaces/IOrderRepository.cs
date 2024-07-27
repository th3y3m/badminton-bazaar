using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task Add(Order order);
        Task Update(Order order);
        Task<Order> GetById(string id);
        Task<List<Order>> GetAll();
        Task<DbSet<Order>> GetDbSet();
        Task Delete(string id);
    }
}
