using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Api.YoutubeChatResponse
{

    public class YoutubeChatResponse
    {
        public List<YoutubeMessage> Messages { get; set; }
        public string NextPageToken { get; set; }
    }

    public class YoutubeMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Platform { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
    }
}
