using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task Add(Payment payment);
        Task Update(Payment payment);
        Task<Payment> GetById(string id);
        Task<List<Payment>> GetAll();
        Task<DbSet<Payment>> GetDbSet();
        Task Delete(string id);
    }
}
