using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models
{
    public class YoutubeModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Platform { get; set; }
        public UserModel User { get; set; }
    }

    public class UserModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
    }
}
