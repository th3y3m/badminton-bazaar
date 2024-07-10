using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderDetailRepository
    {
        private readonly DbContext _dbContext;

        public OrderDetailRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Add(orderDetail);
            _dbContext.SaveChanges();
        }

        public void Update(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Update(orderDetail);
            _dbContext.SaveChanges();
        }

        public OrderDetail GetById(string id) => _dbContext.OrderDetails.Find(id);

        public List<OrderDetail> GetAll()
        {
            return _dbContext.OrderDetails.ToList();
        }

        public void Delete(string id) {
            var orderDetail = GetById(id);
            _dbContext.OrderDetails.Remove(orderDetail);
            _dbContext.SaveChanges();
        }
    }
}
