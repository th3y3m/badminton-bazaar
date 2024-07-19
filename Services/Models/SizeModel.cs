using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class SizeModel
    {
        [StringLength(10)]
        public string SizeId { get; set; }

        [StringLength(10)]
        public string SizeName { get; set; }
    }
}
