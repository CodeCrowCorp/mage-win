using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;

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
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 800, 800));
        }

        private void WebSocket_Closed(Windows.Networking.Sockets.IWebSocket sender, Windows.Networking.Sockets.WebSocketClosedEventArgs args)
        {
            throw new NotImplementedException();
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
                    this.webSocket.Dispose();
                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ChannlePopUp.IsOpen = true;
        }

        private async void Channel_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            var res = await client.GetStringAsync("https://api.mage.stream/wsinit/channelid?channelId=" + ChannelText.Text);
            webSocket = new Windows.Networking.Sockets.MessageWebSocket();
            await webSocket.ConnectAsync(new Uri("wss://api.mage.stream/wsinit/channelid/" + res + "/connect"));
            ChannlePopUp.IsOpen = false;
            webSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
            try
            {
                webSocket.MessageReceived += WebSocket_MessageReceived; ;
                webSocket.Closed += WebSocket_Closed;
            }
            catch (Exception ex)
            {
            }
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
            if (TopAppBar.Visibility == Visibility.Visible)
            {
                TopAppBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                TopAppBar.Visibility = Visibility.Visible;
            }
        }
    }
}
