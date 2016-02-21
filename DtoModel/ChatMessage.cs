using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoModel
{
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public string Sender { get; set; }
        public DateTime MsgTime { get; set; }
        public string Message { get; set; }
        public string Room { get; set; }
    }
}
