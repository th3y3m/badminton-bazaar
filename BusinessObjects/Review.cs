using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Review
    {
        [Key]
        [StringLength(10)]
        public string ReviewId { get; set; }

        [StringLength(255)]
        public string ReviewText { get; set; }

        [Required]
        public DateTime? ReviewDate { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [ForeignKey("IdentityUser")]
        [StringLength(450)]
        public string UserId { get; set; }

        [ForeignKey("Product")]
        [StringLength(10)]
        public string ProductId { get; set; }

        // Navigation properties
        public virtual IdentityUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}
