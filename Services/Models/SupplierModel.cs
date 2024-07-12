using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class SupplierModel
    {
        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
