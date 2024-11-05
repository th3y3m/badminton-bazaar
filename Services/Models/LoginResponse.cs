using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class LoginResponse
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }
    }
}
