using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderRepository
    {
        private readonly DbContext _dbContext;

        public OrderRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Order order)
        {
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
        }

        public void Update(Order order)
        {
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
        }

        public Order GetById(string id) => _dbContext.Orders.Find(id);

        public List<Order> GetAll()
        {
            return _dbContext.Orders.ToList();
        }
        
        public DbSet<Order> GetDbSet()
        {
            return _dbContext.Orders;
        }

        public void Delete(string id) {
            var order = GetById(id);
            order.Status = "Inactive";
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
        }
    }
}
