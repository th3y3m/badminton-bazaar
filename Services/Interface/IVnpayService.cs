using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IVnpayService
    {
        public string CreatePaymentUrl(decimal amount, string infor, string? orderinfor);
        bool ValidateSignature(string rspraw, string inputHash, string secretKey);
        Task<PaymentStatusModel> ValidatePaymentResponse(string queryString);
    }
}
