using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BusinessObjects
{
    public class UserProductDiscount
    {
        [Key]
        [StringLength(10)]
        public string UserProductDiscountId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public string ProductId { get; set; }

        [Required]
        public decimal DiscountPercent { get; set; } = 0;


        // Navigation properties
        public virtual IdentityUser AspNetUsers { get; set; }
        public virtual Product Product { get; set; }
    }
}
