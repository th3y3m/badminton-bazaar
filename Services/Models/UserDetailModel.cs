using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class UserDetailModel
    {
        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(int.MaxValue)]
        public string? ProfilePicture { get; set; }

        public IFormFile? ImageUrl { get; set; }
    }
}
