using MageWin.Models;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using WinUIEx;
using Microsoft.UI.Xaml.Documents;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Chat;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MageWin.Utils;
using System.Runtime.InteropServices;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MageWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<Helpers.ChatMessage> ChatMessages { get; } = new ObservableCollection<Helpers.ChatMessage>();
        private Windows.Networking.Sockets.MessageWebSocket webSocket;
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 300, 600));
            GetAppWindowAndPresenter();
            _apw.IsShownInSwitchers = true;
            _presenter.SetBorderAndTitleBar(true, true);
            ConversationList.ItemsSource = ChatMessages;
            this.CenterOnScreen();
            //this.SetWindowStyle(WindowStyle.Disabled);
            
            Title = "Mage";
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
                            var json = JsonConvert.DeserializeObject<MessageData>(message);
                            if (json != null && json.message != null)
                            {
                                addMessageToStack(json);
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

        private void addMessageToStack(MessageData json)
        {
            if (json.user != null && json.user?.userId != null && json.user.userId != 0)
            {
                AddMessageToChat(json.user.username, json.message, GetSolidColorBrush(Util.GenerateRandomARGBHex()), new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White));
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
                var res = await client.GetStringAsync("https://api.mage.stream/wsinit/channelid?channelId=" + ChannelText.Text);
                SendMessageGrid.Visibility = Visibility.Collapsed;
                webSocket = new Windows.Networking.Sockets.MessageWebSocket();
                webSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
                webSocket.MessageReceived += WebSocket_MessageReceived;
                webSocket.Closed += WebSocket_Closed;
                try
                {

                    Task connectTask = webSocket.ConnectAsync(new Uri("wss://api.mage.stream/wsinit/channelid/" + res + "/connect")).AsTask();
                    var data = new
                    {
                        eventName = "channel-subscribe",
                        channelId = ChannelText.Text,
                        hostId = 0,
                        user = new { userId = 0, username = "guest" }
                    };
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(data);
                    _ = connectTask.ContinueWith(_ => this.SendMessageUsingMessageWebSocketAsync(jsonString));
                  
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
                MessageData data = new MessageData() { eventName = "from app", message = MsgText.Text, timestamp = DateTime.Now.Ticks, user = new User() { userId = 1, username = "app user" } };
                var msg = JsonConvert.SerializeObject(data);
                await SendMessageUsingMessageWebSocketAsync(msg);
              
                AddMessageToChat(data.user.username, data.message, GetSolidColorBrush(Util.GenerateRandomARGBHex()), new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White));
                ConversationScrollViewer.UpdateLayout();
                ConversationScrollViewer.ChangeView(null, ConversationScrollViewer.ScrollableHeight, null);
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

     
        private void AddMessageToChat(string prefix, string message, SolidColorBrush prefixColor, SolidColorBrush messageColor)
        {
            ChatMessages.Add(new Helpers.ChatMessage($"@{prefix}", message, prefixColor, messageColor));
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

    }
}
