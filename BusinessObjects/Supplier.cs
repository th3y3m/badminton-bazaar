using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Supplier
    {
        [Key]
        [StringLength(10)]
        public string SupplierId { get; set; }

        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }        
        
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        public bool Status { get; set; }



        public virtual ICollection<Product> Products { get; set; }
    }
}
