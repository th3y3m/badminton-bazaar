using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ColorModel
    {
        [StringLength(10)]
        public string ColorId { get; set; }

        [StringLength(50)]
        public string ColorName { get; set; }
    }
}
