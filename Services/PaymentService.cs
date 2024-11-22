using BusinessObjects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly.Timeout;
using Polly;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly.Retry;
using Polly.Wrap;
using Microsoft.ML;

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
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public PaymentService(IPaymentRepository paymentRepository, IVnpayService vnpayService, IUserDetailService userDetailService, IOrderService orderService, IMoMoService moMoService, IConnectionMultiplexer redisConnection)
        {
            _paymentRepository = paymentRepository;
            _vnpayService = vnpayService;
            _userDetailService = userDetailService;
            _orderService = orderService;
            _moMoService = moMoService;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
            _redisRetryPolicy = Policy.Handle<SqlException>()
                                   .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                   (exception, timeSpan, retryCount, context) =>
                                   {
                                       Console.WriteLine($"Retry {retryCount} for {context.PolicyKey} at {timeSpan} due to: {exception}.");
                                   });
            _dbRetryPolicy = Policy.Handle<SqlException>()
                                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                (exception, timeSpan, retryCount, context) =>
                                {
                                    Console.WriteLine($"[Db Retry] Attempt {retryCount} after {timeSpan} due to: {exception.Message}");
                                });
            _dbTimeoutPolicy = Policy.TimeoutAsync(10, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Db Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _redisTimeoutPolicy = Policy.TimeoutAsync(2, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Redis Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _dbPolicyWrap = Policy.WrapAsync(_dbRetryPolicy, _dbTimeoutPolicy);
            _redisPolicyWrap = Policy.WrapAsync(_redisRetryPolicy, _redisTimeoutPolicy);
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
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetDbSet()
                );

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
                throw new Exception($"Error retrieving paginated payments: {ex.Message}");
            }
        }

        public async Task<Payment> GetPaymentById(string id)
        {
            try
            {
                if (_redisConnection != null && _redisConnection.IsConnected)
                {
                    try
                    {
                        RedisValue cachedPayment = await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringGetAsync($"payment:{id}")
                        );

                        if (!cachedPayment.IsNullOrEmpty)
                        {
                            return JsonConvert.DeserializeObject<Payment>(cachedPayment);
                        }
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                var payment = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetById(id)
                );

                if (_redisConnection != null && _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                        {
                            await _redisDb.StringSetAsync($"payment:{id}", JsonConvert.SerializeObject(payment), TimeSpan.FromHours(1));
                        });
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }

                return payment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payment by ID: {ex.Message}");
            }
        }

        public async Task<Payment> GetPaymentByOrderId(string id)
        {
            try
            {
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );

                return payments.FirstOrDefault(p => p.OrderId == id);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error retrieving payment by ID: {ex.Message}");
            }
        }

        public async Task AddPayment(Payment payment)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.Add(payment)
                );

                if (_redisConnection != null && _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"payment:{payment.PaymentId}", JsonConvert.SerializeObject(payment), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding payment: {ex.Message}");
            }
        }

        public async Task UpdatePayment(Payment payment)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.Update(payment)
                );

                if (_redisConnection != null && _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"payment:{payment.PaymentId}", JsonConvert.SerializeObject(payment), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating payment: {ex.Message}");
            }
        }

        public async Task DeletePayment(string id)
        {
            try
            {
                await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.Delete(id)
                );

                var payment = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await GetPaymentById(id)
                );

                if (_redisConnection != null && _redisConnection.IsConnected)
                {
                    try
                    {
                        await _redisPolicyWrap.ExecuteAsync(async () =>
                            await _redisDb.StringSetAsync($"payment:{id}", JsonConvert.SerializeObject(payment), TimeSpan.FromHours(1))
                        );
                    }
                    catch (RedisConnectionException ex)
                    {
                        Console.WriteLine($"Redis connection failed: {ex.Message}. Falling back to database.");
                    }
                }
            }
            catch (Exception ex)
            {
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

                throw new Exception($"Error processing booking payment: {ex.Message}");
            }
        }

        public async Task<(decimal revenue, decimal changePercentage)> GetTodayRevenue()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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

                throw new Exception($"Error retrieving today's revenue: {ex.Message}");
            }
        }

        public async Task<(decimal revenue, decimal changePercentage)> GetThisWeekRevenue()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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

                throw new Exception($"Error retrieving this year's revenue: {ex.Message}");
            }
        }

        public async Task<decimal> GetTotalRevenue()
        {
            try
            {
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
                return payments.Where(p => p.PaymentStatus == "Complete").Sum(p => p.PaymentAmount);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error retrieving total revenue: {ex.Message}");
            }
        }

        public async Task<decimal[]> GetRevenueFromStartOfWeek()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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

                throw new Exception($"Error retrieving revenue from start of week: {ex.Message}");
            }
        }

        public async Task<decimal[]> GetRevenueFromStartOfMonth()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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

                throw new Exception($"Error retrieving revenue from start of month: {ex.Message}");
            }

        }

        public async Task<decimal[]> GetRevenueFromStartOfYear()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var payments = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _paymentRepository.GetAll()
                );
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

                throw new Exception($"Error retrieving revenue from start of year: {ex.Message}");
            }
        }

        public async Task<float> PredictNextDayRevenue()
        {
            try
            {
                var mlContext = new MLContext();

                // Load data
                var payments = (await _paymentRepository.GetAll()).Where(p => p.PaymentStatus == "True").ToList();

                if (payments.Count == 0)
                {
                    return 0;
                }

                // Get the date range
                var startDate = payments.Min(p => p.PaymentDate).Date;
                var endDate = DateTime.Now.Date;

                // Create a list of all dates within the range
                var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                         .Select(offset => startDate.AddDays(offset))
                                         .ToList();

                // Aggregate data by day
                var aggregatedData = payments
                    .GroupBy(p => p.PaymentDate.Date)
                    .Select(g => new PaymentDayData
                    {
                        PaymentAmount = (float)g.Sum(p => p.PaymentAmount),
                        DayOfWeek = (float)g.Key.DayOfWeek,
                        Month = (float)g.Key.Month,
                        Year = (float)g.Key.Year
                    }).ToList();

                // Ensure all dates are included, with zero revenue for missing days
                var completeData = allDates.Select(date =>
                    aggregatedData.FirstOrDefault(d => d.DayOfWeek == (float)date.DayOfWeek && d.Month == (float)date.Month && d.Year == (float)date.Year)
                    ?? new PaymentDayData
                    {
                        PaymentAmount = 0,
                        DayOfWeek = (float)date.DayOfWeek,
                        Month = (float)date.Month,
                        Year = (float)date.Year
                    }).ToList();

                var dataView = mlContext.Data.LoadFromEnumerable(completeData);

                // Define the training pipeline
                var pipeline = mlContext.Transforms
                    .Concatenate("Features", nameof(PaymentDayData.DayOfWeek), nameof(PaymentDayData.Month), nameof(PaymentDayData.Year))
                    .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(PaymentDayData.PaymentAmount), maximumNumberOfIterations: 100));

                // Train the model
                var model = pipeline.Fit(dataView);

                // Make a prediction
                var predictionEngine = mlContext.Model.CreatePredictionEngine<PaymentDayData, PaymentPrediction>(model);
                var nextDay = DateTime.Now.AddDays(1);
                var nextDayData = new PaymentDayData
                {
                    DayOfWeek = (float)nextDay.DayOfWeek,
                    Month = (float)nextDay.Month,
                    Year = (float)nextDay.Year
                };

                var prediction = predictionEngine.Predict(nextDayData);
                return prediction.PredictedAmount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error predicting next day's revenue: {ex.Message}");
            }
        }

        public async Task<float> PredictNextMonthRevenue()
        {
            try
            {
                var mlContext = new MLContext();

                // Load data
                var payments = (await _paymentRepository.GetAll()).Where(p => p.PaymentStatus == "True").ToList();

                if (payments.Count == 0)
                {
                    return 0;
                }

                // Get the date range
                var startDate = payments.Min(p => p.PaymentDate).Date;
                var endDate = DateTime.Now.Date;

                // Create a list of all months within the range
                var allMonths = Enumerable.Range(0, (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + 1)
                                          .Select(offset => new DateTime(startDate.Year, startDate.Month, 1).AddMonths(offset))
                                          .ToList();

                // Aggregate data by month
                var aggregatedData = payments
                    .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                    .Select(g => new PaymentMonthData
                    {
                        PaymentAmount = (float)g.Sum(p => p.PaymentAmount),
                        Month = (float)g.Key.Month,
                        Year = (float)g.Key.Year
                    }).ToList();

                // Ensure all months are included, with zero revenue for missing months
                var completeData = allMonths.Select(date =>
                    aggregatedData.FirstOrDefault(d => d.Month == (float)date.Month && d.Year == (float)date.Year)
                    ?? new PaymentMonthData
                    {
                        PaymentAmount = 0,
                        Month = (float)date.Month,
                        Year = (float)date.Year
                    }).ToList();

                var dataView = mlContext.Data.LoadFromEnumerable(completeData);

                // Define the training pipeline
                var pipeline = mlContext.Transforms
                    .Concatenate("Features", nameof(PaymentMonthData.Month), nameof(PaymentMonthData.Year))
                    .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(PaymentMonthData.PaymentAmount), maximumNumberOfIterations: 100));

                // Train the model
                var model = pipeline.Fit(dataView);

                // Make a prediction
                var predictionEngine = mlContext.Model.CreatePredictionEngine<PaymentMonthData, PaymentPrediction>(model);
                var nextMonth = DateTime.Now.AddMonths(1);
                var nextMonthData = new PaymentMonthData
                {
                    Month = (float)nextMonth.Month,
                    Year = (float)nextMonth.Year
                };

                var prediction = predictionEngine.Predict(nextMonthData);
                return prediction.PredictedAmount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error predicting next month's revenue: {ex.Message}");
            }
        }

        public async Task<float> PredictNextYearRevenue()
        {
            try
            {
                var mlContext = new MLContext();

                // Load data
                var payments = (await _paymentRepository.GetAll()).Where(p => p.PaymentStatus == "True").ToList();

                if (payments.Count == 0)
                {
                    return 0;
                }

                // Get the date range
                var startDate = payments.Min(p => p.PaymentDate).Date;
                var endDate = DateTime.Now.Date;

                // Create a list of all years within the range
                var allYears = Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1)
                                         .Select(year => new DateTime(year, 1, 1))
                                         .ToList();

                // Aggregate data by year
                var aggregatedData = payments
                    .GroupBy(p => p.PaymentDate.Year)
                    .Select(g => new PaymentYearData
                    {
                        PaymentAmount = (float)g.Sum(p => p.PaymentAmount),
                        Year = (float)g.Key
                    }).ToList();

                // Ensure all years are included, with zero revenue for missing years
                var completeData = allYears.Select(date =>
                    aggregatedData.FirstOrDefault(d => d.Year == (float)date.Year)
                    ?? new PaymentYearData
                    {
                        PaymentAmount = 0,
                        Year = (float)date.Year
                    }).ToList();

                var dataView = mlContext.Data.LoadFromEnumerable(completeData);

                // Define the training pipeline
                var pipeline = mlContext.Transforms
                    .Concatenate("Features", nameof(PaymentYearData.Year))
                    .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(PaymentYearData.PaymentAmount), maximumNumberOfIterations: 100));

                // Train the model
                var model = pipeline.Fit(dataView);

                // Make a prediction
                var predictionEngine = mlContext.Model.CreatePredictionEngine<PaymentYearData, PaymentPrediction>(model);
                var nextYear = DateTime.Now.AddYears(1);
                var nextYearData = new PaymentYearData
                {
                    Year = (float)nextYear.Year
                };

                var prediction = predictionEngine.Predict(nextYearData);
                return prediction.PredictedAmount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error predicting next year's revenue: {ex.Message}");
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
