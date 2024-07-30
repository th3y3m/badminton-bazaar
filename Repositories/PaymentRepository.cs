using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
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
            try
            {
                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error adding payment: {ex.Message}");
            }
        }

        public async Task Update(Payment payment)
        {
            try
            {
                _dbContext.Payments.Update(payment);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error updating payment: {ex.Message}");
            }
        }

        public async Task<Payment> GetById(string id)
        {
            try
            {
                return await _dbContext.Payments.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving payment by ID: {ex.Message}");
            }
        }

        public async Task<List<Payment>> GetAll()
        {
            try
            {
                return await _dbContext.Payments.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving all payments: {ex.Message}");
            }
        }

        public async Task<DbSet<Payment>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Payments);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving payment DbSet: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var payment = await GetById(id);
                payment.PaymentStatus = "Cancelled";
                _dbContext.Payments.Update(payment);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error deleting payment: {ex.Message}");
            }
        }
    }
}
