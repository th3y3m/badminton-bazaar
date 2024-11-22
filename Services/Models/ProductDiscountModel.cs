using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ProductDiscountModel
    {
        public int NumberOfPurchases { get; set; }
        public int ProductStock { get; set; }
        public decimal MinDiscountPercent { get; set; }
        public decimal MaxDiscountPercent { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
