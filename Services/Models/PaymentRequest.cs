using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class PaymentRequest
    {
        public string RequestType { get; set; }
        public string IpnUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string RequestId { get; set; }
        public string ExtraData { get; set; }
        public string Lang { get; set; }
    }
}
