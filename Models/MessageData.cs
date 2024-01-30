using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models
{
    public class MessageData
    {
        public string eventName { get; set; }
        public User user { get; set; }
        public long timestamp { get; set; }
        public string message { get; set; }
    }

    public class User
    {
        public int userId { get; set; }
        public string username { get; set; }
    }
}
