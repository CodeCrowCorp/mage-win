using AutoMapper;
using Google.Apis.YouTube.v3;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using MageWin.Interfaces;
using MageWin.Models;
using MageWin.Models.Api.ChannelResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using WinUIEx;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MageWin
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public bool _displayPlatformIcons = true;
        public bool _lockScreen;
        public TaskbarIcon? TrayIcon { get; private set; }
        public MainWindow? Window { get; set; }
        public static IServiceProvider ServiceProvider { get; private set; }


        public bool HandleClosedEvents { get; set; } = true;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var services = new ServiceCollection();
            DependencyConfiguration.Configure(services);
            ServiceProvider = services.BuildServiceProvider();
            InitializeTrayIcon();

        }

        private void InitializeTrayIcon()
        {
            var OpenWindowCommand = (XamlUICommand)Resources["OpenWindowCommand"];
            OpenWindowCommand.ExecuteRequested += OpenWindowCommand_ExecuteRequested;

            var lockUnlockWindowCommand = (XamlUICommand)Resources["LockUnlockWindowCommand"];
            lockUnlockWindowCommand.ExecuteRequested += LockUnlockWindowCommand_ExecuteRequested;

            var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
            exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

            var githubCommand = (XamlUICommand)Resources["GitHubCommand"];
            githubCommand.ExecuteRequested += GitHubCommand_ExecuteRequested;

            var discordCommand = (XamlUICommand)Resources["DiscordCommand"];
            discordCommand.ExecuteRequested += DiscordCommand_ExecuteRequested;

            var showHidePlatformIconCommand = (XamlUICommand)Resources["ShowHidePlatformIconCommand"];
            showHidePlatformIconCommand.ExecuteRequested += ShowHidePlatformIconCommand_ExecuteRequested;

            TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
            Image img = new Image();
            TrayIcon.IconSource = new BitmapImage(new Uri("ms-appx:///Assets/Mage16x16.ico"));
            TrayIcon.ForceCreate();

            OpenWindowCommand.Execute(this);
        }

        private void OpenWindowCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {           
            if (Window == null)
            {
              
                Window = new MainWindow();
                Window.Closed += (sender, args) =>
                {
                    if (HandleClosedEvents)
                    {
                        args.Handled = true;
                        Window.Hide();
                    }
                };
               
                Window.SetIcon("Assets/Mage16x16.ico");
                Window.Show();
                return;
            }           
            else
            {
                Window.Show();
                Window.Activate();
            }
        }

        private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            HandleClosedEvents = false;
            TrayIcon?.Dispose();
            Window?.Close();

         
            if (Window == null)
            {
                Environment.Exit(0);
            }
        }

        private void LockUnlockWindowCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {

            if (_lockScreen)
            {
                
                Window.UnlockScreen();
                _lockScreen = false;
            }
            else {
                Window.LockScreen();
                _lockScreen = true;
            }
   
        }

        async private void GitHubCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            _ = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/CodeCrowCorp/mage-win"));
        }

        async private void DiscordCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            _ = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.mage.stream"));
        }

        private void ShowHidePlatformIconCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {            
            if (_displayPlatformIcons)
            {

                Window.ChatMessages.Where(message => message.SvgImage != null).ToList().ForEach(item =>
                {                    
                    item.ImageVisibility = Visibility.Collapsed;
                    item.ReloadMessage(item.Message);
                    
                });
                _displayPlatformIcons = false;
            }
            else
            {
                Window.ChatMessages.Where(message => message.SvgImage != null).ToList().ForEach(item =>
                {                   
                    item.ImageVisibility = Visibility.Visible;
                    item.ReloadMessage(item.Message);
                });
                _displayPlatformIcons = true;
            }
            Window._conversationList.UpdateLayout();
        }

    }
}
