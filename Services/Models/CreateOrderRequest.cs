using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class CreateOrderRequest
    {
        [Required]
        public string UserId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Freight must be a positive value.")]
        public decimal Freight { get; set; }

        [Required]
        public string Address { get; set; }
    }

}
