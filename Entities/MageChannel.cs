using AutoMapper;
using CommunityToolkit.Common;
using Google.Apis.Services;
using MageWin.Enums;
using MageWin.Interfaces;
using MageWin.Models;
using MageWin.Models.Socket;
using MageWin.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.Storage.Streams;

namespace MageWin.Entities
{
    public class MageChannel
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Windows.Networking.Sockets.MessageWebSocket _websocket;
        private readonly IMageServices _mageServices;
        private readonly IMapper _mapper;
        private ChannelModel _channel;
        private ChatModel _chat;
        public string _channelId { get; set; }
        public bool IsLive => _channel?.IsLive == 1;

        public MageChannel(string channelId, Windows.Networking.Sockets.MessageWebSocket webSocket)
        {
            _channelId = channelId;
            _mageServices = App.ServiceProvider.GetRequiredService<IMageServices>();
            _mapper = App.ServiceProvider.GetRequiredService<IMapper>();
            _websocket = webSocket;
            _chat = new ChatModel() {
                YoutubeChat = new YoutubeChat()
            };
            _channel = new ChannelModel();
        }

        public async Task Initialize() {
            var channelDetails = await _mageServices.GetChannelDetailsAsync(this._channelId);
            this._channel = _mapper.Map<ChannelModel>(channelDetails);
        }

        public void StartListenPlatformMessages()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {                  
                        var chatMessages = await this.GetYoutubeChatMessages(this._channel.UserId.ToString(),this.GetNextPageToken());
                        await SendYoutubeMessagesToSocket(chatMessages);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    
                }
            });
        }

        public void StopListenPlatformMessages()
        {
            _cancellationTokenSource?.Cancel();
        }
        public void Dispose() 
        {
            StopListenPlatformMessages();
        }

        public string GetNextPageToken() {
            return this._chat?.YoutubeChat?.NextPageToken ?? string.Empty;
        }
        private void SetPageToken(string nextPagetoken) {
            this._chat.YoutubeChat.NextPageToken = nextPagetoken;
        }
        public async Task<List<YoutubeModel>> GetYoutubeChatMessages(string userId, string nextPagetoken = null)
        {
            try
            {
                var liveChatMessages = await _mageServices.GetYoutubeLiveChatMessages(userId, nextPagetoken);
                if (liveChatMessages != null && liveChatMessages.Messages.Count > 0) {
                    var latestMessages = liveChatMessages.Messages?.Select(item => _mapper.Map<YoutubeModel>(item))
                                                                   .Where(item => !_chat.YoutubeChat.HistoryMessage.Any(history => history.Id == item.Id))
                                                                   .ToList();
                    this.SetPageToken(liveChatMessages?.NextPageToken);
                    this.SetMessageHistoryList(latestMessages);
                    return latestMessages;
                }
                return default;
            }
            catch (Exception ex) {
                return default;
            }
        }

        public async Task SendYoutubeMessagesToSocket(List<YoutubeModel> youtubeMessages)
        {
            if (youtubeMessages != null)
            {                
                foreach (var message in youtubeMessages)
                {
                    Models.Socket.MessageDataRequest data = new Models.Socket.MessageDataRequest()
                    {
                        eventName = "channel-message",
                        channelId = _channelId,
                        message = new Message()
                        {
                            body = message.Message,
                            platform = "youtube",
                            user = new Models.Socket.User()
                            {
                                userId = "youtube",
                                username = message.User.Username
                            },
                            isAiChatEnabled = false
                        }
                    };

                    var msg = JsonConvert.SerializeObject(data);
                    await SendMessageUsingMessageWebSocketAsync(msg);
                }
                
            }
        }

        private async Task SendMessageUsingMessageWebSocketAsync(string message)
        {
            using (var dataWriter = new DataWriter(_websocket.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
            Debug.WriteLine("Sending message using MessageWebSocket: " + message);
        }
        private void SetMessageHistoryList(List<YoutubeModel> youtubeMessages) 
        {
            if (youtubeMessages.Count > 0)
            {
                _chat.YoutubeChat.HistoryMessage.AddRange(youtubeMessages);
            }
        }

        public string GetNameColorByRole(UserRoleType userRole) 
        {
            switch (userRole) {
                case UserRoleType.Host:
                    return ConfigurationManager.AppSettings["text-pink-500"];
                case UserRoleType.Moderator:
                    return ConfigurationManager.AppSettings["text-green-600"];
                case UserRoleType.Random:
                    return ConfigurationManager.AppSettings["text-info"];
                default:
                    return "#FFFFFFFF";
            };
        }
        public string GetTagColorByRole(UserRoleType userRole)
        {
            switch (userRole)
            {
                case UserRoleType.Host:
                    return ConfigurationManager.AppSettings["bg-pink-600"];
                   
                case UserRoleType.Moderator:
                    return ConfigurationManager.AppSettings["bg-green-700"];

                case UserRoleType.Random:
                    return ConfigurationManager.AppSettings["text-info"];                  
                default:
                    return "#FFFFFFFF";
            }
            
        }
        public string GetTagTextByRole(UserRoleType userRole)
        {
            switch (userRole)
            {
                case UserRoleType.Host:
                    return Util.GetDescription(UserRoleType.Host);

                case UserRoleType.Moderator:
                    return Util.GetDescription(UserRoleType.Moderator);

                case UserRoleType.Random:
                    return Util.GetDescription(UserRoleType.Random);
                default:
                    return "";
            }

        }
        public UserRoleType GetUserRoleById(string userId) 
        {
            if (userId == _channel.UserId.ToString())
                return UserRoleType.Host;
            if (_channel.Mods.Any(modId => modId == userId))
                return UserRoleType.Moderator;

            return UserRoleType.Random;
        }
    }


}
