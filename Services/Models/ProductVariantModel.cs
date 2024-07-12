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
    public class ProductVariantModel
    {
        [StringLength(10)]
        public string ProductId { get; set; }

        [StringLength(10)]
        public string ColorId { get; set; }

        [StringLength(10)]
        public string SizeId { get; set; }

        public int StockQuantity { get; set; }

        public decimal Price { get; set; }

        public bool Status { get; set; }

        public List<IFormFile> ProductImageUrl { get; set; }
    }
}
