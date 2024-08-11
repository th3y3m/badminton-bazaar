using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class FreightPrice
    {
        [Key]
        [StringLength(10)]
        public string PriceId { get; set; }

        [Required]
        public decimal MinDistance { get; set; }

        [Required]
        public decimal MaxDistance { get; set; }

        [Required]
        public decimal PricePerKm { get; set; }
    }
}
