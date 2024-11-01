using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Polly.Timeout;
using Polly.Retry;
using Polly.Wrap;
using Polly;
using StackExchange.Redis;

namespace Services
{
    public class MoMoService : IMoMoService
    {
        //private IConfiguration _config;
        //private readonly IOrderRepository _orderRepository;
        //private readonly IPaymentRepository _paymentRepository;
        //private readonly MoMoSettings _settings;

        //static readonly HttpClient client = new HttpClient();

        //public MoMoService(IOrderRepository orderRepository, IPaymentRepository paymentRepository, IConfiguration config, IOptions<MoMoSettings> settings)
        //{
        //    _orderRepository = orderRepository;
        //    _paymentRepository = paymentRepository;
        //    _config = config;
        //    _settings = settings.Value;
        //}

        //public async Task<MoMoResponse> CreatePaymentRequest(PaymentRequest request)
        //{
        //    var rawData = $"accessKey={_settings.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}&ipnUrl={request.IpnUrl}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={_settings.PartnerCode}&redirectUrl={request.RedirectUrl}&requestId={request.RequestId}&requestType={request.RequestType}";
        //    var signature = HashUtil.HmacSHA256(_settings.SecretKey, rawData);
        //    var paymentRequest = new
        //    {
        //        partnerCode = _settings.PartnerCode,
        //        requestType = request.RequestType,
        //        ipnUrl = request.IpnUrl,
        //        redirectUrl = request.RedirectUrl,
        //        orderId = request.OrderId,
        //        amount = request.Amount,
        //        orderInfo = request.OrderInfo,
        //        requestId = request.RequestId,
        //        extraData = request.ExtraData,
        //        signature = signature,
        //        lang = request.Lang
        //    };

        //    var content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(_settings.Endpoint, content);

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"Response Status Code: {response.StatusCode}");
        //        Console.WriteLine($"Response Reason Phrase: {response.ReasonPhrase}");
        //        Console.WriteLine($"Response Headers: {response.Headers}");
        //        Console.WriteLine($"Response Content: {responseString}");
        //    }

        //    return JsonConvert.DeserializeObject<MoMoResponse>(responseString);
        //}

        //private string GenerateSignature(string data, string key)
        //{
        //    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        //    {
        //        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        //        return BitConverter.ToString(hash).Replace("-", "").ToLower();
        //    }
        //}

        //public async Task<string> GenerateMomoUrl(string orderInfo, decimal amount)
        //{
        //    try
        //    {
        //        MoMoRequestData reqData = CreateRequestData(orderInfo, amount.ToString(), null);
        //        MoMoResponse res = await SendMoMoRequest(reqData);

        //        if (res != null && !string.IsNullOrEmpty(res.PayUrl))
        //        {
        //            return res.PayUrl;
        //        }
        //        else
        //        {
        //            throw new Exception("Failed to generate MoMo payment URL.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception here (e.g., using ILogger)
        //        Debug.WriteLine($"Error in GenerateMomoUrl: {ex.Message}");
        //        return null;
        //    }
        //}

        //public string CreateRawData(MoMoRequestData request) => $"partnerCode={request.PartnerCode}" +
        //        $"&accessKey={request.AccessKey}" +
        //        $"&requestId={request.OrderID}" +
        //        $"&amount={request.Amount}" +
        //        $"&orderId={request.OrderID}" +
        //        $"&orderInfo={request.OrderInfo}" +
        //        $"&returnUrl={request.ReturnUrl}" +
        //        $"&notifyUrl={request.NotifyUrl}" +
        //        $"&extraData=";


        //public string SignSHA256(string msg, string secretKey)
        //{
        //    using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        //    {
        //        return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(msg))).ToLower().Replace("-", "");
        //    }
        //}

        //public string CreateRequestBody(MoMoRequestData request) => JsonConvert.SerializeObject(new
        //{
        //    accessKey = request.AccessKey,
        //    partnerCode = request.PartnerCode,
        //    requestType = request.RequestType,
        //    notifyUrl = request.NotifyUrl,
        //    returnUrl = request.ReturnUrl,
        //    orderId = request.OrderID,
        //    amount = request.Amount,
        //    orderInfo = request.OrderInfo,
        //    requestId = request.OrderID,
        //    extraData = request.ExtraData,
        //    signature = SignSHA256(CreateRawData(request), request.SecretKey)
        //});


        //public async Task<MoMoResponse> SendMoMoRequest(MoMoRequestData request)
        //{
        //    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, request.Endpoint)
        //    {
        //        Content = new StringContent(CreateRequestBody(request), Encoding.UTF8, "application/json")
        //    };

        //    try
        //    {
        //        Debug.WriteLine(httpRequestMessage.Headers.ToString());
        //        var response = await client.SendAsync(httpRequestMessage);

        //        response.EnsureSuccessStatusCode();
        //        var responseBody = await response.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<MoMoResponse>(responseBody);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error in SendMoMoRequest: {ex.Message}");
        //        throw;
        //    }
        //}

        //public MoMoRequestData CreateRequestData(string orderInfo, string amount, string returnUrl) => new MoMoRequestData()
        //{
        //    OrderInfo = orderInfo,
        //    Amount = amount,
        //    ReturnUrl = string.IsNullOrEmpty(returnUrl) ? _config["MoMoSettings:ReturnUrl"] : returnUrl,
        //    NotifyUrl = string.IsNullOrEmpty(returnUrl) ? _config["MoMoSettings:NotifyUrl"] : returnUrl,
        //    PartnerCode = _config["MoMoSettings:PartnerCode"],
        //    Endpoint = _config["MoMoSettings:Endpoint"],
        //    AccessKey = _config["MoMoSettings:AccessKey"],
        //    SecretKey = _config["MoMoSettings:SecretKey"],
        //    RequestType = _config["MoMoSettings:RequestType"],
        //    ExtraData = _config["MoMoSettings:ExtraData"],
        //    OrderID = DateTime.UtcNow.Ticks.ToString() + orderInfo.GetHashCode(),
        //};

        //public async Task<PaymentStatusModel> ValidatePaymentResponse(string queryString)
        //{
        //    try
        //    {
        //        var queryParameters = HttpUtility.ParseQueryString(queryString);
        //        string requestId = queryParameters["requestId"];
        //        string orderId = queryParameters["orderId"];
        //        string amount = queryParameters["amount"];
        //        string resultCode = queryParameters["resultCode"];
        //        string signature = queryParameters["signature"];

        //        // Validate MoMo signature
        //        string rawHash = $"accessKey={_config["MoMoSettings:AccessKey"]}&amount={amount}&extraData={queryParameters["extraData"]}&orderId={orderId}&orderInfo={queryParameters["orderInfo"]}&orderType=momo_wallet&partnerCode={_config["MoMoSettings:PartnerCode"]}&requestId={requestId}&responseTime={queryParameters["responseTime"]}&resultCode={resultCode}&transId={queryParameters["transId"]}";
        //        bool isSignatureValid = HashUtil.HmacSHA256(_config["MoMoSettings:SecretKey"], rawHash) == signature;

        //        if (!isSignatureValid)
        //        {
        //            return new PaymentStatusModel { IsSuccessful = false, RedirectUrl = "https://localhost:3000/reject" };
        //        }

        //        var order = await _orderRepository.GetById(orderId);
        //        if (order == null || order.Status == "Completed")
        //        {
        //            return new PaymentStatusModel { IsSuccessful = false, RedirectUrl = "https://localhost:3000/reject" };
        //        }

        //        if (resultCode == "0") // MoMo Success Result Code
        //        {
        //            var payment = new Payment
        //            {
        //                PaymentId = "P" + HashUtil.GenerateRandomId(7),
        //                OrderId = orderId,
        //                PaymentAmount = decimal.Parse(amount) / 1000,
        //                PaymentDate = DateTime.Now,
        //                PaymentStatus = "Completed"
        //            };
        //            await _paymentRepository.Add(payment);

        //            order.Status = "Completed";
        //            await _orderRepository.Update(order);

        //            return new PaymentStatusModel
        //            {
        //                IsSuccessful = true,
        //                RedirectUrl = $"https://localhost:3000/confirm?orderId={orderId}"
        //            };
        //        }
        //        else
        //        {
        //            order.Status = "Failed";
        //            await _orderRepository.Update(order);

        //            return new PaymentStatusModel
        //            {
        //                IsSuccessful = false,
        //                RedirectUrl = $"https://localhost:3000/reject?orderId={orderId}"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PaymentStatusModel { IsSuccessful = false, RedirectUrl = "LINK_ERROR" };
        //    }
        //}
        private readonly ILogger<MoMoService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncRetryPolicy _redisRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncTimeoutPolicy _redisTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncPolicyWrap _redisPolicyWrap;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        // MoMo API URLs and keys
        private readonly string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        private readonly string partnerCode = "MOMOBKUN20180529";
        private readonly string accessKey = "klm05TvNBzhg7h7j";
        private readonly string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        private readonly string returnUrl = "https://localhost:7173/MomoAPI/paymentconfirm";
        private readonly string notifyUrl = "https://localhost:7173/MomoAPI/ipn";

        public MoMoService(ILogger<MoMoService> logger, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IConnectionMultiplexer redisConnection, IOrderService orderService)
        {
            _logger = logger;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
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
            _orderService = orderService;
        }

        public async Task<string> CreatePaymentUrl(decimal amount, string orderId, string orderInfo)
        {
            try
            {
                string requestId = Guid.NewGuid().ToString(); // Unique request ID
                string extraData = ""; // Base64-encoded extra data if needed
                string requestType = "captureWallet";

                // Ensure the amount is formatted correctly
                long formattedAmount = (long)(amount * 1000); // Convert to VND

                // Create raw signature string
                string rawHash = $"accessKey={accessKey}&amount={formattedAmount}&extraData={extraData}&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";
                string signature = HashUtil.HmacSHA256(secretKey, rawHash);

                // Build request payload
                var paymentRequest = new
                {
                    partnerCode,
                    partnerName = "MoMo",
                    storeId = "MoMoStore",
                    requestId,
                    amount = formattedAmount.ToString(),
                    orderId,
                    orderInfo,
                    redirectUrl = returnUrl,
                    ipnUrl = notifyUrl,
                    extraData,
                    requestType,
                    signature,
                    lang = "en"
                };

                // Send POST request to MoMo API
                string response = await HashUtil.SendHttpRequest(endpoint, paymentRequest);

                // Parse response and extract payment URL
                var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                if (jsonResponse.ContainsKey("payUrl"))
                {
                    return jsonResponse["payUrl"].ToString();
                }
                else
                {
                    throw new Exception($"Error creating payment URL: {jsonResponse["message"]}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to MoMo API failed.");
                throw new Exception($"Error processing payment: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreatePaymentUrl method.");
                throw;
            }
        }



        public async Task<PaymentStatusModel> ValidatePaymentResponse(string queryString)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var queryParameters = HttpUtility.ParseQueryString(queryString);
                string requestId = queryParameters["requestId"];
                string orderId = queryParameters["orderId"];
                string amount = queryParameters["amount"];
                string resultCode = queryParameters["resultCode"];
                string signature = queryParameters["signature"];

                // Validate MoMo signature
                //string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={queryParameters["extraData"]}&orderId={orderId}&orderInfo={queryParameters["orderInfo"]}&orderType=momo_wallet&partnerCode={partnerCode}&requestId={requestId}&responseTime={queryParameters["responseTime"]}&resultCode={resultCode}&transId={queryParameters["transId"]}";

                //string sign = HashUtil.HmacSHA256(secretKey, rawHash);
                //bool isSignatureValid = sign == signature;

                //if (!isSignatureValid)
                //{
                //    _logger.LogWarning("Invalid MoMo signature.");
                //    return new PaymentStatusModel { IsSuccessful = false, RedirectUrl = "https://localhost:3000/reject" };
                //}

                var order = await _orderService.GetOrderById(orderId);
                if (order == null || order.Status == "Completed")
                {
                    return new PaymentStatusModel { IsSuccessful = false, RedirectUrl = "https://localhost:3000/reject" };
                }

                if (resultCode == "0") // MoMo Success Result Code
                {
                    var payment = new Payment
                    {
                        PaymentId = "P" + HashUtil.GenerateRandomId(7),
                        OrderId = orderId,
                        PaymentAmount = decimal.Parse(amount) / 1000,
                        PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone),
                        PaymentMessage = "Complete",
                        PaymentStatus = "True",
                        PaymentSignature = signature,
                        PaymentMethod = "MoMo"
                    };

                    await _dbPolicyWrap.ExecuteAsync(async () => await _paymentRepository.Add(payment));

                    order.Status = "Completed";

                    await _dbPolicyWrap.ExecuteAsync(async () =>
                        await _orderRepository.Update(order)
                    );

                    return new PaymentStatusModel
                    {
                        IsSuccessful = true,
                        RedirectUrl = $"https://localhost:3000/confirm?orderId={orderId}"
                    };
                }
                else
                {
                    order.Status = "Failed";
                    await _dbPolicyWrap.ExecuteAsync(async () =>
                        await _orderRepository.Update(order)
                    );

                    var payment = new Payment
                    {
                        PaymentId = "P" + HashUtil.GenerateRandomId(7),
                        OrderId = orderId,
                        PaymentAmount = decimal.Parse(amount) / 1000,
                        PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZone),
                        PaymentMessage = "Failed",
                        PaymentStatus = "Failed",
                        PaymentSignature = signature,
                        PaymentMethod = "MoMo"
                    };

                    await _dbPolicyWrap.ExecuteAsync(async () =>
                        await _paymentRepository.Add(payment)
                    );

                    return new PaymentStatusModel
                    {
                        IsSuccessful = false,
                        RedirectUrl = $"https://localhost:3000/reject?orderId={orderId}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ValidatePaymentResponse method.");
                return new PaymentStatusModel { IsSuccessful = false, RedirectUrl = "https://localhost:3000/reject" };
            }
        }

    }
}
