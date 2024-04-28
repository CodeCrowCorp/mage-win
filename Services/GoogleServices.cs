using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Net.Http;
using MageWin.Interfaces;
using System.Configuration;

namespace MageWin.Services
{
    public class GoogleServices : IGoogleServices
    {
        private readonly string _googleApiKey;
        private readonly string _googleBaseUrl;
        private readonly IBaseClientService _clientService; 
        public GoogleServices(IBaseClientService baseClientService)
        {
            this._googleApiKey = ConfigurationManager.AppSettings["GoogleApiKey"];
            this._googleBaseUrl =  ConfigurationManager.AppSettings["GoogleApiBaseUrl"];
            _clientService = baseClientService;
        }

        public async  Task<Models.Services.YouTubeSearchResponse.YouTubeSearchResponse> GetYoutubeVideoLiveAsync(string channelId) {
            var parameters = new {
                part = "id",
                channelId = channelId,
                eventType = "live",
                type = "video",
                key = this._googleApiKey
            };
            var searchUrl = $"{this._googleBaseUrl}/youtube/v3/search";
            return  await _clientService.CallService<Models.Services.YouTubeSearchResponse.YouTubeSearchResponse>(searchUrl, HttpMethod.Get, parameters: parameters);
        }

        public async Task<Models.Services.YouTubeVideoResponse.YouTubeVideoResponse> GetYoutubeLiveDetailsAsync(string videoId)
        {
            var parameters = new
            {
                part = "snippet,contentDetails,statistics,liveStreamingDetails",
                id = videoId,
                key = this._googleApiKey
            };
            var videoUrl = $"{this._googleBaseUrl}/youtube/v3/videos";
            return await _clientService.CallService<Models.Services.YouTubeVideoResponse.YouTubeVideoResponse>(videoUrl, HttpMethod.Get, parameters: parameters);
        }

        public async Task<Models.Services.YouTubeLiveChatMessageListResponse.YouTubeLiveChatMessageListResponse> GetYoutubeLiveChatMessages(string liveChatId,int maxResults,string pageToken = null)
        {
            var parameters = new
            {
                part = "snippet,authorDetails",
                liveChatId = liveChatId,
                key = this._googleApiKey,
                maxResults = maxResults,
                pageToken = pageToken,
            };
            var chatMessageUrl = $"{this._googleBaseUrl}/youtube/v3/liveChat/messages";
            return await _clientService.CallService<Models.Services.YouTubeLiveChatMessageListResponse.YouTubeLiveChatMessageListResponse>(chatMessageUrl, HttpMethod.Get, parameters: parameters);
        }

    }  
}
