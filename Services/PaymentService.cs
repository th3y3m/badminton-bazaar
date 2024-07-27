using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderService _orderService;
        private readonly IVnpayService _vnpayService;
        private readonly IUserDetailService _userDetailService;

        public PaymentService(IPaymentRepository paymentRepository, IVnpayService vnpayService, IUserDetailService userDetailService, IOrderService orderService)
        {
            _paymentRepository = paymentRepository;
            _vnpayService = vnpayService;
            _userDetailService = userDetailService;
            _orderService = orderService;
        }

        public async Task<PaginatedList<Payment>> GetPaginatedPayments(
            string searchQuery,
            string sortBy,
            string status,
            string orderId,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _paymentRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

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

        public async Task<Payment> GetPaymentById(string id) => await _paymentRepository.GetById(id);

        public async Task AddPayment(Payment payment) => await _paymentRepository.Add(payment);

        public async Task UpdatePayment(Payment payment) => await _paymentRepository.Update(payment);

        public async Task DeletePayment(string id) => await _paymentRepository.Delete(id);

        public async Task<string?> ProcessBookingPayment(string role, string orderId)
        {
            var order = await  _orderService.GetOrderById(orderId);
            var price = await _orderService.TotalPrice(orderId);
            if (order == null)
            {
                return null;
            }

            var paymentURL = _vnpayService.CreatePaymentUrl(price, role, order.OrderId);


            return paymentURL;
        }
        //public async Task<ResponseModel> ProcessBookingPaymentByBalance(string orderId)
        //{
        //    try
        //    {
        //        var order = _orderService.GetOrderById(orderId);
        //        var user = _userDetailService.GetUserById(order.UserId);

        //        if (order == null || user == null)
        //        {
        //            return new ResponseModel
        //            {
        //                Status = "Error",
        //                Message = "Booking information is required."
        //            };
        //        }

        //        if (user.Balance < order.TotalPrice || user.Balance - order.TotalPrice < 0)
        //        {
        //            return new ResponseModel
        //            {
        //                Status = "Error",
        //                Message = "Error While Processing Balance(Not enough balance)"
        //            };
        //        }
        //        user.Balance -= order.TotalPrice;
        //        order.Status = "Complete";
        //        await _userDetailService.UpdateUserDetailAsync(user.UserId);
        //        await _orderRepository.UpdateBooking(order);
        //        var payment = new Payment
        //        {
        //            PaymentId = "P" + GenerateId.GenerateShortBookingId(),
        //            orderId = orderId,
        //            PaymentAmount = order.TotalPrice,
        //            PaymentDate = DateTime.Now,
        //            PaymentMessage = "Complete",
        //            PaymentStatus = "True",

        //        };
        //        _paymentRepository.AddPayment(payment);
        //        return new ResponseModel
        //        {
        //            Status = "Success",
        //            Message = "Payment Success"
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        return new ResponseModel
        //        {
        //            Status = "Error",
        //            Message = e.Message
        //        };
        //    }
        //}
    }
}
