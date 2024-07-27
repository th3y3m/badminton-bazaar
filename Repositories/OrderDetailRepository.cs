using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly DbContext _dbContext;

        public OrderDetailRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Add(orderDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Update(orderDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderDetail> GetById(string id) => await _dbContext.OrderDetails.FindAsync(id);

        public async Task <OrderDetail> GetByOrderId(string id) => 
            await Task.FromResult(_dbContext.OrderDetails.FirstOrDefault(x => x.OrderId == id));

        public async Task<OrderDetail> GetByProductId(string id) =>
            await Task.FromResult(_dbContext.OrderDetails.FirstOrDefault(x => x.ProductVariantId == id));

        public async Task<List<OrderDetail>> GetAll()
        {
            return await _dbContext.OrderDetails.ToListAsync();
        }
        public async Task<DbSet<OrderDetail>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.OrderDetails);
        }

        public async Task Delete(string id) {
            var orderDetail = await GetById(id);
            _dbContext.OrderDetails.Remove(orderDetail);
            _dbContext.SaveChanges();
        }
    }
}
