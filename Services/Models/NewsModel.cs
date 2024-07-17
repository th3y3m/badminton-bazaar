using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class NewsModel
    {
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(int.MaxValue)]
        public string Content { get; set; }

        [StringLength(int.MaxValue)]
        public string Image { get; set; }

        public bool IsHomepageSlideshow { get; set; }

        public bool IsHomepageBanner { get; set; }

        public bool Status { get; set; }
    }
}
