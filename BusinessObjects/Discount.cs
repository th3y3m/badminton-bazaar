using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Discount
    {
        [Key]
        [StringLength(10)]
        public string DiscountId { get; set; }

        [Required]
        public decimal MinDiscountPercent { get; set; }

        [Required]
        public decimal MaxDiscountPercent { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public bool Status { get; set; }


        // Navigation properties
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; }
    }
}
