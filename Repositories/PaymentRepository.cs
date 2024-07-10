using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PaymentRepository
    {
        private readonly DbContext _dbContext;

        public PaymentRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Payment payment)
        {
            _dbContext.Payments.Add(payment);
            _dbContext.SaveChanges();
        }

        public void Update(Payment payment)
        {
            _dbContext.Payments.Update(payment);
            _dbContext.SaveChanges();
        }

        public Payment GetById(string id) => _dbContext.Payments.Find(id);

        public List<Payment> GetAll()
        {
            return _dbContext.Payments.ToList();
        }
        public DbSet<Payment> GetDbSet()
        {
            return _dbContext.Payments;
        }

        public void Delete(string id) {
            var payment = GetById(id);
            payment.PaymentStatus = "Cancelled";
            _dbContext.Payments.Update(payment);
            _dbContext.SaveChanges();
        }
    }
}
