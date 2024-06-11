using MageWin.Models.Api.YoutubeChatResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MageWin.Models.Api.ChannelResponse;
using MageWin.Models.Api.PlatformInfoResponse;

namespace MageWin.Interfaces
{
    public interface IMageServices
    {
        Task<YoutubeChatResponse> GetYoutubeLiveChatMessages(string userId, string pageToken = null);
        Task<ChannelResponse> GetChannelDetailsAsync(string channelId);
        Task<PlatformInfoResponse> GetPlatformsLiveAsync(string channelId);
    }
}
