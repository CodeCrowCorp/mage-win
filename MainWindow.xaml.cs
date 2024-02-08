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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MageWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Windows.Networking.Sockets.MessageWebSocket webSocket;
        private DispatcherQueue _dispatcherQueue;
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 300, 600));
            GetAppWindowAndPresenter();
            _apw.IsShownInSwitchers = true;
            _presenter.SetBorderAndTitleBar(true, true);
            _presenter.IsMaximizable=false;
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
                StackPanel mainStack = new StackPanel() { Orientation = Orientation.Horizontal, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent), HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(10, 10, 0, 0) };
                TextBlock msgText = new TextBlock() { FontSize = 15, Margin = new Thickness(20, 0, 0, 0), Text = json.message, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center, TextWrapping = TextWrapping.Wrap };
                if (json.user != null)
                {
                    TextBlock userText = new TextBlock()
                    {
                        FontSize = 15,
                        Text = json.user.username,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    mainStack.Children.Add(userText);
                    mainStack.Children.Add(msgText);
                    ChatStack.Children.Add(mainStack);
                }
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
        bool IsConnected = false;
        private async void Channel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new HttpClient();
                var res = await client.GetStringAsync("https://api.mage.stream/wsinit/channelid?channelId=" + ChannelText.Text);
                SendMessageGrid.Visibility = Visibility.Collapsed;
                webSocket = new Windows.Networking.Sockets.MessageWebSocket();
                webSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
                webSocket.MessageReceived += WebSocket_MessageReceived;
                webSocket.Closed += WebSocket_Closed;
                try
                {
                    var color = GetSolidColorBrush("#554444ff").Color;
                    MainWindowUI.SystemBackdrop = new WinUIEx.TransparentTintBackdrop() { TintColor = color };
                    FileMenu.Visibility = Visibility.Collapsed;
                    _presenter.SetBorderAndTitleBar(false, false);
                    _presenter.IsResizable = false;
                    Task connectTask = webSocket.ConnectAsync(new Uri("wss://api.mage.stream/wsinit/channelid/" + res + "/connect")).AsTask();
                    IsConnected = true;
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
            }
            catch (Exception ex)
            {
                IsConnected = false;
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


        private void AppBar_Closing(object sender, object e)
        {
            ChannlePopUp.IsOpen = false;
        }

        private void OpenChannel_Click(object sender, RoutedEventArgs e)
        {
            ChannlePopUp.IsOpen = true;
        }
        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //if (TopAppBar.Visibility == Visibility.Visible)
            //{
            //    TopAppBar.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    TopAppBar.Visibility = Visibility.Visible;
            //}
        }
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MsgText.Text))
            {
                MessageData data = new MessageData() { eventName = "from app", message = MsgText.Text, timestamp = DateTime.Now.Ticks, user = new User() { userId = 1, username = "app user" } };
                var msg = JsonConvert.SerializeObject(data);
                await SendMessageUsingMessageWebSocketAsync(msg);
                MsgText.Text = "";
            }
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //if (IsConnected)
            //{
            //    var color = GetSolidColorBrush("#554444ff").Color;
            //    //MainWindowUI.SystemBackdrop = new WinUIEx.TransparentTintBackdrop() { TintColor = Colors.White };
            //    //FileMenu.Visibility = Visibility.Visible;
            //    // _presenter.SetBorderAndTitleBar(true, true);
            //}
        }
        private AppWindow _apw;
        private OverlappedPresenter _presenter;
        public void GetAppWindowAndPresenter()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
            _presenter.IsMaximizable = false;
            _presenter.SetBorderAndTitleBar(false,false);
            _presenter.IsMinimizable = false;
            
        }
        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (IsConnected)
            {
                //var color = GetSolidColorBrush("#554444ff").Color;
                ////MainWindowUI.SystemBackdrop = new WinUIEx.TransparentTintBackdrop() { TintColor = color };
                ////FileMenu.Visibility = Visibility.Collapsed;
                //_presenter.SetBorderAndTitleBar(false, false);

            }
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsConnected)
            {
                if (FileMenu.Visibility == Visibility.Visible)
                {
                    FileMenu.Visibility = Visibility.Collapsed;
                    _presenter.SetBorderAndTitleBar(false, false);
                }
                else
                {
                    FileMenu.Visibility = Visibility.Visible;
                    _presenter.SetBorderAndTitleBar(true, true);
                }
            }
        }

        private void FileMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ChannlePopUp.IsOpen = true;
        }
    }
}
