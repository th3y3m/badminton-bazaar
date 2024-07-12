﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class OrderDetail
    {
        [Key]
        [StringLength(10)]
        public string OrderDetailId { get; set; }

        [ForeignKey("Order")]
        [StringLength(10)]
        public string OrderId { get; set; }
        
        [ForeignKey("Product")]
        [StringLength(10)]
        public string ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        public virtual Product Product { get; set; }

        public virtual Order Order { get; set; }

        public decimal TotalPrice() => Quantity * UnitPrice;
    }
}
