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
using MageWin.Models.Api.ChannelResponse;
using MageWin.Models.Api.YoutubeChatResponse;
using MageWin.Models.Api.PlatformInfoResponse;

namespace MageWin.Services
{
    public class MageServices : IMageServices
    {
        private readonly string _mageStreamApi;
        private readonly IBaseClientService _clientService; 
        public MageServices(IBaseClientService baseClientService)
        {          
            this._mageStreamApi =  ConfigurationManager.AppSettings["MageBaseUrlApi"];
            _clientService = baseClientService;
        }

        public async Task<ChannelResponse> GetChannelDetailsAsync(string channelId)
        {
            var parameters = new
            {
                channelId = channelId
            };
            var channelDetailsUrl = $"{this._mageStreamApi}/channel";
            return await _clientService.CallService<ChannelResponse>(channelDetailsUrl, HttpMethod.Get, parameters: parameters);
        }

        public async Task<YoutubeChatResponse> GetYoutubeLiveChatMessages(string userId, string pageToken = null)
        {
            var parameters = new
            {
                userId = userId,
                pageToken = pageToken,
            };
            var chatMessageUrl = $"{this._mageStreamApi}/youtube/messages";
            return await _clientService.CallService<YoutubeChatResponse>(chatMessageUrl, HttpMethod.Get, parameters: parameters);
        }
        public async Task<PlatformInfoResponse> GetPlatformsLiveAsync(string channelId)
        {
            var parameters = new
            {
                channelId = channelId
            };
            var platformUrl = $"{this._mageStreamApi}/platforms";
            return await _clientService.CallService<PlatformInfoResponse>(platformUrl, HttpMethod.Get, parameters: parameters);
        }

    }  
}
