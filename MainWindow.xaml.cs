using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 300, 600));
        }

        private void WebSocket_Closed(Windows.Networking.Sockets.IWebSocket sender, Windows.Networking.Sockets.WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WebSocket_Closed; Code: " + args.Code + ", Reason: \"" + args.Reason + "\"");
        }

        private void WebSocket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    Debug.WriteLine("Message received from MessageWebSocket: " + message);
                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
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
                var client = new HttpClient();
                var res = await client.GetStringAsync("https://api.mage.stream/wsinit/channelid?channelId=" + ChannelText.Text);
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
    }
}
