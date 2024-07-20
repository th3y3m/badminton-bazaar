using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using Services.Models;
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
        private readonly OrderService _orderService;
        private readonly VnpayService _vnpayService;
        private readonly UserDetailService _userDetailService;

        public PaymentService(PaymentRepository paymentRepository, OrderService orderService, VnpayService vnpayService, UserDetailService userDetailService)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
            _vnpayService = vnpayService;
            _userDetailService = userDetailService;
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

        public async Task<string> ProcessBookingPayment(string role, string orderId)
        {
            var order =  _orderService.GetOrderById(orderId);
            var price = _orderService.TotalPrice(orderId);
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
