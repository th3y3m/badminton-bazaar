using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class PriceFactor
    {
        [Key]
        [StringLength(10)]
        public string PriceFactorId { get; set; }

        [Required]
        [StringLength(50)]
        public string PriceFactorName { get; set; }

        [Required]
        [StringLength(500)]
        public string PriceFactorDescription { get; set; }

        [Required]
        public decimal PriceFactorValue { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
