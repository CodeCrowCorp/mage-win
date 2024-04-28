using CommunityToolkit.Common;
using Google.Apis.Services;
using MageWin.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;

namespace MageWin.Entities
{
    public class Youtube
    {
        public readonly IGoogleServices _googleServices;
        public string ChannelId { get; set; }
        public Live ActiveLive { get; set; }
        public bool IsLive => ActiveLive != null && !string.IsNullOrWhiteSpace(ActiveLive.ActiveLiveChatId) && !string.IsNullOrWhiteSpace(ActiveLive.VideoId);

        public Youtube(string channelId)
        {
            this.ChannelId = channelId;
            _googleServices = App.ServiceProvider.GetRequiredService<IGoogleServices>();
           
        }
        public async Task Initialize() {
            try
            {
                var videoResponse = await _googleServices.GetYoutubeVideoLiveAsync(this.ChannelId);
                var liveDetailsResponse = await _googleServices.GetYoutubeLiveDetailsAsync(videoResponse.Items?.FirstOrDefault()?.Id.VideoId);

                this.ActiveLive = new Live
                {
                    ActiveLiveChatId = liveDetailsResponse.Items[0]?.LiveStreamingDetails.ActiveLiveChatId,
                    VideoId = videoResponse.Items?.FirstOrDefault()?.Id.VideoId
                };
            }
            catch (Exception ex) { 
                
            }

        }

        public string GetNextPageToken() {
            return this.ActiveLive.NextPageToken;
        }
        private void SetPageToken(string nextPagetoken) {
            this.ActiveLive.NextPageToken = nextPagetoken;
        }
        public async Task<List<Models.Services.YouTubeLiveChatMessageListResponse.Item>> GetChatMessages(string nextPagetoken = null)
        {
            try
            {
                var liveChatMessages = await _googleServices.GetYoutubeLiveChatMessages(this.ActiveLive.ActiveLiveChatId, 100, this.ActiveLive.NextPageToken);
                this.SetPageToken(liveChatMessages?.NextPageToken);
                var latestMessages = liveChatMessages.Items.Where(item => !this.ActiveLive.ChatHistoryId.Any(id => item.Id == id)).ToList();
                return latestMessages;
            }
            catch (Exception ex) {
                return default;
            }
        }
    }

    public class Live { 
        public string ActiveLiveChatId { get; set; }
        public string VideoId { get; set; }
        public List<string> ChatHistoryId = new List<string>();
        public string NextPageToken { get; set; }

    }
}
