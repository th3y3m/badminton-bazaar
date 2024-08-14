using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                _dbContext.OrderDetails.Add(orderDetail);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the order detail.", ex);
            }
        }

        public async Task Update(OrderDetail orderDetail)
        {
            try
            {
                _dbContext.OrderDetails.Update(orderDetail);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order detail.", ex);
            }
        }

        public async Task<OrderDetail> GetById(string id)
        {
            try
            {
                return await _dbContext.OrderDetails.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order detail by ID.", ex);
            }
        }

        public async Task<List<OrderDetail>> GetByOrderId(string id)
        {
            try
            {
                return await Task.FromResult(_dbContext.OrderDetails.Where(x => x.OrderId == id).ToList());
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order detail by order ID.", ex);
            }
        }

        public async Task<OrderDetail> GetByProductId(string id)
        {
            try
            {
                return await Task.FromResult(_dbContext.OrderDetails.FirstOrDefault(x => x.ProductVariantId == id));
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order detail by product ID.", ex);
            }
        }

        public async Task<List<OrderDetail>> GetAll()
        {
            try
            {
                return await _dbContext.OrderDetails.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all order details.", ex);
            }
        }

        public async Task<DbSet<OrderDetail>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.OrderDetails);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the DbSet of order details.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var orderDetail = await GetById(id);
                if (orderDetail != null)
                {
                    _dbContext.OrderDetails.Remove(orderDetail);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Order detail not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the order detail.", ex);
            }
        }
    }
}
