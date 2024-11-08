using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserItemInteraction
    {
        public int InteractionId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string InteractionType { get; set; }
        public DateTime Timestamp { get; set; }
        public int Rating { get; set; }
    }

}
