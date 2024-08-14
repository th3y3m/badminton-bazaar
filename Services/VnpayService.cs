using BusinessObjects;
using Services.Models;
using Microsoft.Extensions.Logging;
using Repositories;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Repositories.Interfaces;
using Services.Interface;

namespace Services
{
    public class VnpayService : IVnpayService
    {
        private readonly ILogger<VnpayService> _logger;
        public string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // HTTPS
        public string returnUrl = $"https://localhost:7173/VNpayAPI/paymentconfirm\r\n";
        public string tmnCode = "FKUXJX95";
        public string hashSecret = "0D3EAMNJYSY9INENB5JYP8XW2U8MD8WE";
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        public VnpayService(ILogger<VnpayService> logger, IOrderRepository OrderRepository, IPaymentRepository payment)
        {
            _logger = logger;
            _orderRepository = OrderRepository;
            _paymentRepository = payment;
        }

        public string CreatePaymentUrl(decimal amount, string infor, string? orderinfor)
        {
            try
            {
                string hostName = Dns.GetHostName();
                string clientIPAddress = Dns.GetHostAddresses(hostName).GetValue(0).ToString();
                PayLib pay = new PayLib();
                var vnp_Amount = amount * 100000;
                pay.AddRequestData("vnp_Version", PayLib.VERSION);
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", tmnCode);
                pay.AddRequestData("vnp_Amount", vnp_Amount.ToString("0"));
                pay.AddRequestData("vnp_BankCode", "");
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", clientIPAddress);
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", infor);
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", returnUrl);
                pay.AddRequestData("vnp_TxnRef", orderinfor);

                string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreatePaymentUrl method");
                throw;
            }
        }



        public async Task<PaymentStatusModel> ValidatePaymentResponse(string queryString)
        {
            try
            {
                var json = HttpUtility.ParseQueryString(queryString);
                var booking = await _orderRepository.GetById((json["vnp_TxnRef"]).ToString());
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString();
                string vnp_SecureHash = json["vnp_SecureHash"].ToString();
                var pos = queryString.IndexOf("&vnp_SecureHash");
                bool checkSignature = ValidateSignature(queryString.Substring(1, pos - 1), vnp_SecureHash, hashSecret);

                if (booking.Status == "true" && booking != null)
                {
                    return new PaymentStatusModel
                    {
                        IsSuccessful = false,
                        RedirectUrl = "LINK_INVALID"
                    };
                }




                if (checkSignature && tmnCode == json["vnp_TmnCode"].ToString())
                {
                    var bookingid = json["vnp_TxnRef"].ToString();

                    if (vnp_ResponseCode == "00" && json["vnp_TransactionStatus"] == "00")
                    {
                        var payment = new Payment
                        {
                            PaymentId = "P" + GenerateId.GenerateRandomId(7),
                            OrderId = bookingid,
                            PaymentAmount = decimal.Parse(json["vnp_Amount"]) / 100000,
                            PaymentDate = DateTime.Now,
                            PaymentMessage = "Complete",
                            PaymentStatus = "True",
                            PaymentSignature = json["vnp_BankTranNo"].ToString(),
                        };
                        await _paymentRepository.Add(payment);


                        booking.Status = "Complete";
                        await _orderRepository.Update(booking);



                        return new PaymentStatusModel
                        {
                            IsSuccessful = true,
                            RedirectUrl = $"https://localhost:3000/confirm?vnp_TxnRef={json["vnp_TxnRef"].ToString()}"
                        };
                    }
                    else
                    {
                        var amount = decimal.Parse(json["vnp_Amount"]);
                        if (json["vnp_BankTranNo"]?.ToString() != null || json["vnp_TxnRef"]?.ToString() != null)
                        {
                            booking.Status = "cancel";
                            await _orderRepository.Update(booking);

                            var payment = new Payment
                            {
                                PaymentId = "P" + GenerateId.GenerateRandomId(7),
                                OrderId = bookingid,
                                PaymentAmount = amount / 100000,
                                PaymentDate = DateTime.Now,
                                PaymentMessage = "Fail",
                                PaymentStatus = "False",

                            };
                            await _paymentRepository.Add(payment);
                        }
                        return new PaymentStatusModel
                        {
                            IsSuccessful = false,
                            RedirectUrl = $"https://localhost:3000/reject?vnp_TxnRef={json["vnp_TxnRef"].ToString()}"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning("Signature validation failed or tmnCode mismatch");
                    return new PaymentStatusModel
                    {
                        IsSuccessful = false,
                        RedirectUrl = "LINK_INVALID"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ValidatePaymentResponse method");
                return new PaymentStatusModel
                {
                    IsSuccessful = false,
                    RedirectUrl = "LINK_ERROR"
                };
            }
        }



        public bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = Utils.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
