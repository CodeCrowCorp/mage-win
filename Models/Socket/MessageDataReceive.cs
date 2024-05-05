using MageWin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Socket.MessageDataReceive
{
    public class MessageDataReceive
    {
        public string eventName { get; set; }
        public User user { get; set; }
        public long timestamp { get; set; }
        public string message { get; set; }
        public string platform { get; set; }
        public string iconUrl { get; set; }

    }

    public class User
    {
        public string userId { get; set; }
        public string username { get; set; }
    }


}
