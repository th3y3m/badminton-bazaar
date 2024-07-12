using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Models
{
    public class ProductModel
    {

        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(10)]
        public string CategoryId { get; set; }

        [StringLength(10)]
        public string SupplierId { get; set; }

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        public decimal BasePrice { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(int.MaxValue)]
        public string? ImageUrl { get; set; }

        public bool Status { get; set; }

        public IFormFile ProductImageUrl { get; set; }
    }
}
