using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ProductVariant
    {
        [Key]
        [StringLength(10)]
        public string ProductVariantId { get; set; }

        [ForeignKey("Product")]
        [StringLength(10)]
        public string ProductId { get; set; }

        [ForeignKey("Color")]
        [StringLength(10)]
        public string? ColorId { get; set; }

        [ForeignKey("Size")]
        [StringLength(10)]
        public string? SizeId { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(int.MaxValue)]
        public string VariantImageURL { get; set; }

        [Required]
        public bool Status { get; set; }



        // Navigation properties
        public virtual Color? Color { get; set; }
        public virtual Size? Size { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
