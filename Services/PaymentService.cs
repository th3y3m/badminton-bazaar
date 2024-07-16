using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository;

        public PaymentService(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public PaginatedList<Payment> GetPaginatedPayments(
            string searchQuery,
            string sortBy,
            string status,
            string orderId,
            int pageIndex,
            int pageSize)
        {
            var source = _paymentRepository.GetDbSet().AsNoTracking();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.PaymentMessage.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrEmpty(status))
            {
                source = source.Where(p => p.PaymentStatus == status);
            }

            if (!string.IsNullOrEmpty(orderId))
            {
                source = source.Where(p => p.OrderId == orderId);
            }

            source = sortBy switch
            {
                "paymentdate_asc" => source.OrderBy(p => p.PaymentDate),
                "paymentdate_desc" => source.OrderByDescending(p => p.PaymentDate),
                "orderid_asc" => source.OrderBy(p => p.OrderId),
                "orderid_desc" => source.OrderByDescending(p => p.OrderId),
                _ => source
            };

            // Apply pagination
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Payment>(items, count, pageIndex, pageSize);
        }

        public Payment GetPaymentById(string id) => _paymentRepository.GetById(id);

        public void AddPayment(Payment payment) => _paymentRepository.Add(payment);

        public void UpdatePayment(Payment payment) => _paymentRepository.Update(payment);

        public void DeletePayment(string id) => _paymentRepository.Delete(id);
        
    }
}
