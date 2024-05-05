using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Socket
{
    public class MessageDataRequest
    {
        public string eventName { get; set; }
        public string channelId { get; set; }
        public Message message { get; set; }
    }
    public class User
    {
        public string userId { get; set; }
        public string username { get; set; }
    }

    public class Message
    {
        public string body { get; set; }
        public string platform { get; set; }
        public bool isAiChatEnabled { get; set; }
        public User user { get; set; }
    }
}
