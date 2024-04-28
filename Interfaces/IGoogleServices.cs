using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Interfaces
{
    public interface IGoogleServices
    {
        Task<Models.Services.YouTubeSearchResponse.YouTubeSearchResponse> GetYoutubeVideoLiveAsync(string channelId);

        Task<Models.Services.YouTubeVideoResponse.YouTubeVideoResponse> GetYoutubeLiveDetailsAsync(string videoId);

        Task<Models.Services.YouTubeLiveChatMessageListResponse.YouTubeLiveChatMessageListResponse> GetYoutubeLiveChatMessages(string liveChatId, int maxResult, string pageToken = null);

    }
}
