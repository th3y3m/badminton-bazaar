using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Order
    {
        [Key]
        [StringLength(10)]
        public string OrderId { get; set; }

        [ForeignKey("User")]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }     
        
        public DateTime? RequiredDate { get; set; }      
        
        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(500)]
        [Required]
        public string ShipAddress { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
