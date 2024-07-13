using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class CartItem
    {
        public string ItemId { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal getSubTotal()
        {
            return Quantity * UnitPrice;
        }
    }
}
