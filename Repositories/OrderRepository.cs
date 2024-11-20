using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContext _dbContext;

        public OrderRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Order order)
        {
            try
            {
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the order.", ex);
            }
        }

        public async Task Update(Order order)
        {
            try
            {
                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order.", ex);
            }
        }

        public async Task<Order> GetById(string id)
        {
            try
            {
                return await _dbContext.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order by ID.", ex);
            }
        }

        public async Task<Order> GetLatestOrder(string userId)
        {
            try
            {
                return await _dbContext.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order by ID.", ex);
            }
        }

        public async Task<List<Order>> GetAll()
        {
            try
            {
                return await _dbContext.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all orders.", ex);
            }
        }

        public async Task<DbSet<Order>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Orders);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the DbSet of orders.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var order = await GetById(id);
                if (order != null)
                {
                    order.Status = "Inactive";
                    _dbContext.Orders.Update(order);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Order not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the order.", ex);
            }
        }
    }
}
