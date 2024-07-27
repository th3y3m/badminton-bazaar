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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DbContext _dbContext;

        public PaymentRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Payment payment)
        {
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Payment payment)
        {
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Payment> GetById(string id) => await _dbContext.Payments.FindAsync(id);

        public async Task<List<Payment>> GetAll()
        {
            return await _dbContext.Payments.ToListAsync();
        }
        public async Task<DbSet<Payment>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Payments);
        }

        public async Task Delete(string id)
        {
            var payment = await GetById(id);
            payment.PaymentStatus = "Cancelled";
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
