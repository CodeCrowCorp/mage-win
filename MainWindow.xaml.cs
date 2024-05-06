using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using WinUIEx;
using System.Collections.ObjectModel;
using MageWin.Utils;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using MageWin.Entities;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.Foundation.Metadata;
using MageWin.Enums;
using Microsoft.UI.Xaml.Controls;
using System.Net.WebSockets;
using MageWin.Models.Socket;
using MageWin.Models.Socket.MessageDataReceive;
using WinUIEx.Messaging;
using ColorCode.Compilation.Languages;
using System.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MageWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ListView _conversationList { get; set; }
        public ObservableCollection<Helpers.ChatMessage> ChatMessages { get; } = new ObservableCollection<Helpers.ChatMessage>();
        private Windows.Networking.Sockets.MessageWebSocket webSocket;
        private DispatcherTimer _youtubeTimer;
        private MageChannel _mageChannel;
        public string _channelId { get; set; }
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 300, 600));
            GetAppWindowAndPresenter();
            _apw.IsShownInSwitchers = true;
            _presenter.SetBorderAndTitleBar(true, true);
            _conversationList = ConversationList;
            ConversationList.ItemsSource = ChatMessages;
            this.CenterOnScreen();

            Title = "Mage";
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }
        private void WebSocket_Closed(Windows.Networking.Sockets.IWebSocket sender, Windows.Networking.Sockets.WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WebSocket_Closed; Code: " + args.Code + ", Reason: \"" + args.Reason + "\"");
        }

        private async void WebSocket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    Debug.WriteLine("Message received from MessageWebSocket: " + message);
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        if (IsValidJson(message))
                        {
                            var json = JsonConvert.DeserializeObject<MessageDataReceive>(message);
                            if (json != null && json.message != null)
                            {
                                AddMessageToStack(json);
                                if (IsVerticalScrollFullyDown())
                                {
                                    ConversationScrollViewer.UpdateLayout();
                                    ConversationScrollViewer.ChangeView(null, ConversationScrollViewer.ScrollableHeight, null);
                                }
                                    
                            }
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        private void AddMessageToStack(MessageDataReceive json)
        {
            if (json.user != null && json.user?.userId != null)
            {
                UserRoleType userRole = _mageChannel.GetUserRoleById(json.user.userId);
                AddMessageToChat(json.user.username, json.message, GetSolidColorBrush(_mageChannel.GetNameColorByRole(userRole)), GetSolidColorBrush(ConfigurationManager.AppSettings["default-color"]), GetSolidColorBrush(_mageChannel.GetTagColorByRole(userRole)),_mageChannel.GetTagTextByRole(userRole), json.iconUrl);
            }
        }

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ChannlePopUp.IsOpen = true;
        }

        private async void Channel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResponseProgressBar.Visibility = Visibility.Visible;
                var client = new HttpClient();
                //var res = await client.GetStringAsync("https://api.mage.stream/wsinit/channelid?channelId=" + ChannelText.Text);
                SendMessageGrid.Visibility = Visibility.Collapsed;
                webSocket = new Windows.Networking.Sockets.MessageWebSocket();
                webSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
                webSocket.MessageReceived += WebSocket_MessageReceived;
                webSocket.Closed += WebSocket_Closed;
                try
                {

                    Task connectTask = webSocket.ConnectAsync(new Uri("wss://api.mage.stream/wsinit/channelid/" + ChannelText.Text + "/connect")).AsTask();
                    var data = new
                    {
                        eventName = "channel-subscribe",
                        channelId = ChannelText.Text,
                        hostId = 0,
                        user = new { userId = 0, username = "guest" }
                    };
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(data);
                    _ = connectTask.ContinueWith(_ => this.SendMessageUsingMessageWebSocketAsync(jsonString))
                                   .ContinueWith(_ => this.InitializeChannel(data.channelId, webSocket));

                    _channelId = ChannelText.Text;
                }
                catch (Exception ex)
                {
                    Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
                    // Add additional code here to handle exceptions.
                }
                ChannlePopUp.IsOpen = false;
                ResponseProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task SendMessageUsingMessageWebSocketAsync(string message)
        {
            using (var dataWriter = new DataWriter(this.webSocket.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
            Debug.WriteLine("Sending message using MessageWebSocket: " + message);
        }


        private void OpenChannel_Click(object sender, RoutedEventArgs e)
        {
            ChannlePopUp.IsOpen = true;
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            ResponseProgressBar.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(MsgText.Text))
            {
                MessageDataRequest data = new MessageDataRequest()
                {
                    eventName = "channel-message",
                    channelId = _channelId,
                    message = new Models.Socket.Message()
                    {
                        body = MsgText.Text,
                        platform = "",
                        user = new Models.Socket.User()
                        {
                            userId = "0",
                            username = "app user"
                        },
                        isAiChatEnabled = false
                    }
                };
                var msg = JsonConvert.SerializeObject(data);
                await SendMessageUsingMessageWebSocketAsync(msg);
              
                if (IsVerticalScrollFullyDown())
                {
                    ConversationScrollViewer.UpdateLayout();
                    ConversationScrollViewer.ChangeView(null, ConversationScrollViewer.ScrollableHeight, null);
                }            
                ResponseProgressBar.Visibility = Visibility.Collapsed;
                MsgText.Text = "";
            }
            ResponseProgressBar.Visibility = Visibility.Collapsed;
        }

        private AppWindow _apw;
        private OverlappedPresenter _presenter;
        public void GetAppWindowAndPresenter()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
            _presenter.SetBorderAndTitleBar(false,false);
        }

     
        private void AddMessageToChat(string prefix, string message, SolidColorBrush prefixColor, SolidColorBrush messageColor,SolidColorBrush tagColor,string tagText,string iconPath = null)
        {
            ChatMessages.Add(new Helpers.ChatMessage($"@{prefix}", message, prefixColor,messageColor,tagColor,tagText,iconPath));
            //var lastItem = ChatMessages.Last();
            //ConversationList.ScrollIntoView(lastItem);
        }
  

        public void LockScreen() {
            var color = GetSolidColorBrush("#000000ff").Color;
            MainWindowUI.SystemBackdrop = new WinUIEx.TransparentTintBackdrop() { TintColor = color };
            FileMenu.Visibility = Visibility.Collapsed;
            _presenter.SetBorderAndTitleBar(false, false);
            _presenter.IsResizable = false;
            MainWindowUI.Show();
            MainWindowUI.Activate();
            MainWindowUI.SetIsAlwaysOnTop(true);
            SetTransparentWindowNonInteractive(true);
        }

        public void UnlockScreen() 
        {
            MainWindowUI.SystemBackdrop = new WinUIEx.TransparentTintBackdrop() { TintColor = Colors.Black };
            FileMenu.Visibility = Visibility.Visible;
            _presenter.SetBorderAndTitleBar(true, true);
            _presenter.IsResizable = true;
            MainWindowUI.SetIsAlwaysOnTop(false);
            MainWindowUI.CenterOnScreen();
            SetTransparentWindowNonInteractive(false);
        }
      
        public void SetTransparentWindowNonInteractive(bool nonInteractive)
        {

            if (nonInteractive)
            {
                this.SetWindowOpacity(255);
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
            else {
                this.SetWindowOpacity(255);
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
            }
        }

        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);


        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public bool IsVerticalScrollFullyDown()
        {
            return this.ConversationScrollViewer.VerticalOffset == ConversationScrollViewer.ScrollableHeight;
        }

        public async Task InitializeChannel(string channelId,Windows.Networking.Sockets.MessageWebSocket webSocket) {
            _mageChannel?.Dispose();
            _mageChannel = new MageChannel(channelId, webSocket);
            await _mageChannel.Initialize();
            if (_mageChannel.IsLive)
            {             
                _mageChannel.StartListenPlatformMessages();
            }

        }    
    }
}
