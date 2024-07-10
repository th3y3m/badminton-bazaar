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

        [StringLength(50)]
        public string ProductName { get; set; }

        [ForeignKey("Category")]
        [StringLength(10)]
        public string CategoryId { get; set; }

        [ForeignKey("Supplier")]
        [StringLength(10)]
        public string SupplierId { get; set; }

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int QuantityInStock { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
