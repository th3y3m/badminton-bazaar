using BusinessObjects;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPaymentService
    {
        Task<PaginatedList<Payment>> GetPaginatedPayments(
            string searchQuery,
            string sortBy,
            string status,
            string orderId,
            int pageIndex,
            int pageSize);
        Task<Payment> GetPaymentById(string id);

        Task AddPayment(Payment payment);

        Task UpdatePayment(Payment payment);

        Task DeletePayment(string id);

        Task<string?> ProcessBookingPayment(string role, string orderId);
        Task<Payment> GetPaymentByOrderId(string id);
    }
}
