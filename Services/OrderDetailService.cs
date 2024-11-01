using BusinessObjects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
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

        public async Task<PaginatedList<OrderDetail>> GetPaginatedOrderDetails(
            string productVariantId,
            string orderId,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _orderDetailRepository.GetDbSet()
                );
                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(productVariantId))
                {
                    source = source.Where(p => p.ProductVariantId == productVariantId);
                }
                if (!string.IsNullOrEmpty(orderId))
                {
                    source = source.Where(p => p.OrderId == orderId);
                }

                source = sortBy switch
                {
                    "totalprice_asc" => source.OrderBy(p => p.TotalPrice()),
                    "totalprice_desc" => source.OrderByDescending(p => p.TotalPrice()),
                    _ => source
                };

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<OrderDetail>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving paginated order details.", ex);
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetail(string orderId)
        {
            try
            {
                var dbSet = await _dbPolicyWrap.ExecuteAsync(async () =>
                    await _orderDetailRepository.GetDbSet()
                );
                var source = dbSet.AsNoTracking();

                source = source.Where(p => p.OrderId == orderId);

                return await source.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving order details by order ID.", ex);
            }
        }
    }
}
