using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Product
    {
        [Key]
        [StringLength(10)]
        public string ProductId { get; set; }

        [StringLength(100)]
        public string ProductName { get; set; }

        [ForeignKey("Category")]
        [StringLength(10)]
        public string CategoryId { get; set; }

        [ForeignKey("Supplier")]
        [StringLength(10)]
        public string SupplierId { get; set; }

        [StringLength(int.MaxValue)]
        public string? ProductDescription { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string ImageUrl { get; set; }

        [Required]
        public decimal BasePrice { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        public decimal MinPrice { get; set; }

        [Required]
        public decimal MaxPrice { get; set; }

        [Required]
        public decimal DefaultPrice { get; set; }


        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; }
        public virtual ICollection<UserProductDiscount> UserProductDiscounts { get; set; }
    }
}
