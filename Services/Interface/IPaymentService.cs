using BusinessObjects;
using Microsoft.ML;
using Services.Helper;
using Services.Models;
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
        Task<string?> ProcessBookingPaymentMoMo(string role, string orderId);
        Task<Payment> GetPaymentByOrderId(string id);
        Task<decimal> GetTotalRevenue();
        Task<(decimal revenue, decimal changePercentage)> GetTodayRevenue();
        Task<(decimal revenue, decimal changePercentage)> GetThisWeekRevenue();
        Task<(decimal revenue, decimal changePercentage)> GetThisMonthRevenue();
        Task<(decimal revenue, decimal changePercentage)> GetThisYearRevenue();
        Task<decimal[]> GetRevenueFromStartOfWeek();
        Task<decimal[]> GetRevenueFromStartOfMonth();
        Task<decimal[]> GetRevenueFromStartOfYear();
        Task<float> PredictNextDayRevenue();
        Task<float> PredictNextMonthRevenue();
        Task<float> PredictNextYearRevenue();
    }
}
