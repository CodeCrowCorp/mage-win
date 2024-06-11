using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models
{
    public class ChatModel
    {
        public YoutubeChat YoutubeChat { get; set; }     
    }

    public class YoutubeChat {
        public List<YoutubeModel> HistoryMessage = new List<YoutubeModel>();
        public string NextPageToken { get; set; }
    }


}
