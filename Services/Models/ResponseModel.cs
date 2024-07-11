using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
