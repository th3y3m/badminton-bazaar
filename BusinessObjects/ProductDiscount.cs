using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ProductDiscount
    {
        [Key]
        [StringLength(10)]
        public string ProductDiscountId { get; set; }

        [ForeignKey("Discount")]
        [StringLength(10)]
        public string DiscountId { get; set; }    
        
        [ForeignKey("Product")]
        [StringLength(10)]
        public string ProductId { get; set; }


        // Navigation properties
        public virtual Discount Discount { get; set; }
        public virtual Product Product { get; set; }
    }
}
