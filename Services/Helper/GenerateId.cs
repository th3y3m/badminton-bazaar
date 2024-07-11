using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class GenerateId
    {
        public static string GenerateRandomId(int length)
        {
            return Guid.NewGuid().ToString().Substring(0, length);
        }
    }
}
