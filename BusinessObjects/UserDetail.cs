using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BusinessObjects
{
    public class UserDetail
    {
        [Key]
        [ForeignKey("User")]
        [StringLength(450)]
        public string UserId { get; set; }

        public decimal Balance { get; set; }

        public decimal? Point { get; set; }

        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(500)]
        public string? ProfilePicture { get; set; }

        [StringLength(int.MaxValue)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiration { get; set; }


        // Navigation property
        public virtual IdentityUser User { get; set; }
    }
}
