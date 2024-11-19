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
    public class BrowsingHistory
    {
        [Key]
        [StringLength(10)]
        public string BrowsingHistoryId { get; set; }

        [ForeignKey("IdentityUser")]
        [StringLength(450)]
        public string UserId { get; set; }

        [ForeignKey("Product")]
        [StringLength(10)]
        public string ProductId { get; set; }

        [Required]
        public DateTime BrowsedAt { get; set; }

        [StringLength(255)]
        public string? SessionId { get; set; }

        // Navigation properties
        public virtual IdentityUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}
