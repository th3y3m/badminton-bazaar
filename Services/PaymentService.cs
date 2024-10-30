using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderService _orderService;
        private readonly IVnpayService _vnpayService;
        private readonly IMoMoService _moMoService;
        private readonly IUserDetailService _userDetailService;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        public PaymentService(IPaymentRepository paymentRepository, IVnpayService vnpayService, IUserDetailService userDetailService, IOrderService orderService, IMoMoService moMoService, IConnectionMultiplexer redisConnection)
        {
            _paymentRepository = paymentRepository;
            _vnpayService = vnpayService;
            _userDetailService = userDetailService;
            _orderService = orderService;
            _moMoService = moMoService;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
        }

        public async Task<PaginatedList<Payment>> GetPaginatedPayments(
            string searchQuery,
            string sortBy,
            string status,
            string orderId,
            int pageIndex,
            int pageSize)
        {
            try
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
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving paginated payments: {ex.Message}");
            }
        }

        public async Task<Payment> GetPaymentById(string id)
        {
            try
            {
                var cachedPayment = await _redisDb.StringGetAsync($"payment:{id}");

                if (!cachedPayment.IsNullOrEmpty)
                    return JsonConvert.DeserializeObject<Payment>(cachedPayment);
                var payment = await _paymentRepository.GetById(id);
                await _redisDb.StringSetAsync($"payment:{id}", JsonConvert.SerializeObject(payment), TimeSpan.FromHours(1));
                return payment;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving payment by ID: {ex.Message}");
            }
        }

        public async Task<Payment> GetPaymentByOrderId(string id)
        {
            try
            {
                var payments = await _paymentRepository.GetAll();
                return payments.FirstOrDefault(p => p.OrderId == id);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving payment by ID: {ex.Message}");
            }
        }

        public async Task AddPayment(Payment payment)
        {
            try
            {
                await _paymentRepository.Add(payment);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error adding payment: {ex.Message}");
            }
        }

        public async Task UpdatePayment(Payment payment)
        {
            try
            {
                await _paymentRepository.Update(payment);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error updating payment: {ex.Message}");
            }
        }

        public async Task DeletePayment(string id)
        {
            try
            {
                await _paymentRepository.Delete(id);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error deleting payment: {ex.Message}");
            }
        }

        public async Task<string?> ProcessBookingPayment(string role, string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                var price = await _orderService.TotalPrice(orderId);
                if (order == null)
                {
                    return null;
                }

                var paymentURL = _vnpayService.CreatePaymentUrl(price, role, order.OrderId);
                return paymentURL;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error processing booking payment: {ex.Message}");
            }
        }
        public async Task<string?> ProcessBookingPaymentMoMo(string role, string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                var price = await _orderService.TotalPrice(orderId);
                if (order == null)
                {
                    return null;
                }

                var paymentURL = await _moMoService.CreatePaymentUrl(price, order.OrderId, role);
                //string paymentURL = await _moMoService.GenerateMomoUrl(order.OrderId, price);

                return paymentURL;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error processing booking payment: {ex.Message}");
            }
        }

        public async Task<(decimal revenue, decimal changePercentage)> GetTodayRevenue()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var todayPayments = payments.Where(p => p.PaymentDate.Date == today && p.PaymentStatus == "Complete").ToList();
                var revenue = todayPayments.Sum(p => p.PaymentAmount);
                var changePercentage = 0.0m;
                if (todayPayments.Count > 0)
                {
                    var yesterday = today.AddDays(-1);
                    var yesterdayPayments = payments.Where(p => p.PaymentDate.Date == yesterday && p.PaymentStatus == "Complete").ToList();
                    var yesterdayRevenue = yesterdayPayments.Sum(p => p.PaymentAmount);
                    changePercentage = (revenue - yesterdayRevenue) / yesterdayRevenue * 100;
                }

                return (revenue, changePercentage);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving today's revenue: {ex.Message}");
            }
        }

        public async Task<(decimal revenue, decimal changePercentage)> GetThisWeekRevenue()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var firstDayOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var thisWeekPayments = payments.Where(p => p.PaymentDate.Date >= firstDayOfWeek && p.PaymentStatus == "Complete").ToList();
                var revenue = thisWeekPayments.Sum(p => p.PaymentAmount);
                var changePercentage = 0.0m;
                if (thisWeekPayments.Count > 0)
                {
                    var lastWeek = firstDayOfWeek.AddDays(-7);
                    var lastWeekPayments = payments.Where(p => p.PaymentDate.Date >= lastWeek && p.PaymentDate.Date < firstDayOfWeek && p.PaymentStatus == "Complete").ToList();
                    var lastWeekRevenue = lastWeekPayments.Sum(p => p.PaymentAmount);
                    changePercentage = (revenue - lastWeekRevenue) / lastWeekRevenue * 100;
                }

                return (revenue, changePercentage);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving this week's revenue: {ex.Message}");
            }
        }

        //GetThisMonthRevenue
        public async Task<(decimal revenue, decimal changePercentage)> GetThisMonthRevenue()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var thisMonthPayments = payments.Where(p => p.PaymentDate.Date >= firstDayOfMonth && p.PaymentStatus == "Complete").ToList();
                var revenue = thisMonthPayments.Sum(p => p.PaymentAmount);
                var changePercentage = 0.0m;
                if (thisMonthPayments.Count > 0)
                {
                    var lastMonth = firstDayOfMonth.AddMonths(-1);
                    var lastMonthPayments = payments.Where(p => p.PaymentDate.Date >= lastMonth && p.PaymentDate.Date < firstDayOfMonth && p.PaymentStatus == "Complete").ToList();
                    var lastMonthRevenue = lastMonthPayments.Sum(p => p.PaymentAmount);
                    changePercentage = (revenue - lastMonthRevenue) / lastMonthRevenue * 100;
                }

                return (revenue, changePercentage);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving this month's revenue: {ex.Message}");
            }
        }

        //GetThisYearRevenue

        public async Task<(decimal revenue, decimal changePercentage)> GetThisYearRevenue()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var firstDayOfYear = new DateTime(today.Year, 1, 1);
                var thisYearPayments = payments.Where(p => p.PaymentDate.Date >= firstDayOfYear && p.PaymentStatus == "Complete").ToList();
                var revenue = thisYearPayments.Sum(p => p.PaymentAmount);
                var changePercentage = 0.0m;
                if (thisYearPayments.Count > 0)
                {
                    var lastYear = firstDayOfYear.AddYears(-1);
                    var lastYearPayments = payments.Where(p => p.PaymentDate.Date >= lastYear && p.PaymentDate.Date < firstDayOfYear && p.PaymentStatus == "Complete").ToList();
                    var lastYearRevenue = lastYearPayments.Sum(p => p.PaymentAmount);
                    changePercentage = (revenue - lastYearRevenue) / lastYearRevenue * 100;
                }

                return (revenue, changePercentage);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving this year's revenue: {ex.Message}");
            }
        }

        public async Task<decimal> GetTotalRevenue()
        {
            try
            {
                var payments = await _paymentRepository.GetAll();
                return payments.Where(p => p.PaymentStatus == "Complete").Sum(p => p.PaymentAmount);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving total revenue: {ex.Message}");
            }
        }
        public async Task<decimal[]> GetRevenueFromStartOfWeek()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var firstDayOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var revenue = new decimal[7];
                for (int i = 0; i < 7; i++)
                {
                    var day = firstDayOfWeek.AddDays(i);
                    var dayPayments = payments.Where(p => p.PaymentDate.Date == day && p.PaymentStatus == "Complete").ToList();
                    revenue[i] = dayPayments.Sum(p => p.PaymentAmount);
                }

                return revenue;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving revenue from start of week: {ex.Message}");
            }
        }

        public async Task<decimal[]> GetRevenueFromStartOfMonth()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var revenue = new decimal[DateTime.DaysInMonth(today.Year, today.Month)];
                for (int i = 0; i < revenue.Length; i++)
                {
                    var day = firstDayOfMonth.AddDays(i);
                    var dayPayments = payments.Where(p => p.PaymentDate.Date == day && p.PaymentStatus == "Complete").ToList();
                    revenue[i] = dayPayments.Sum(p => p.PaymentAmount);
                }

                return revenue;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving revenue from start of month: {ex.Message}");
            }

        }

        public async Task<decimal[]> GetRevenueFromStartOfYear()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _paymentRepository.GetAll();
                var today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone).Date;
                var firstDayOfYear = new DateTime(today.Year, 1, 1);
                var revenue = new decimal[12];
                for (int i = 0; i < 12; i++)
                {
                    var month = firstDayOfYear.AddMonths(i);
                    var monthPayments = payments.Where(p => p.PaymentDate.Month == month.Month && p.PaymentDate.Year == month.Year && p.PaymentStatus == "Complete").ToList();
                    revenue[i] = monthPayments.Sum(p => p.PaymentAmount);
                }

                return revenue;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception($"Error retrieving revenue from start of year: {ex.Message}");
            }
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
