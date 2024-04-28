using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.YoutubeMessageData
{
    public class YoutubeMessageData : MessageData
    {
        public new User user { get; set; }
    }
    public class User
    {
        public string userId { get; set; }
        public string username { get; set; }
    }
}
