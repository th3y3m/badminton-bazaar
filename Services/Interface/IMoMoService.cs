using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMoMoService
    {
        //Task<MoMoResponse> CreatePaymentRequest(PaymentRequest request);
        Task<PaymentStatusModel> ValidatePaymentResponse(string queryString);
        Task<string> CreatePaymentUrl(decimal amount, string orderId, string orderInfo);
        //Task<PaymentStatusModel> ValidatePaymentResponse(MoMoRedirectResult result);
        //public string CreateRawData(MoMoRequestData request);
        //public MoMoRequestData CreateRequestData(string orderInfo, string amount, string returnUrl);
        //public string CreateRequestBody(MoMoRequestData request);
        //public Task<MoMoResponse> SendMoMoRequest(MoMoRequestData request);
        //public string SignSHA256(string msg, string secretKey);
        //Task<string> GenerateMomoUrl(string orderInfo, decimal amount);
    }
}
