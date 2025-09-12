using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Windows.AppLifecycle;
using Windows.System;
using Fischldesu.WCTCore;

namespace WCT_WinUI3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow? mainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Log.Fatal($"{e.Exception}: {e.Exception.Message}\r\n{e.Exception.StackTrace}");
            Crash(e);
        }

        public static void Crash(Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var crashWindow = new Window()
            {
                Title = "Windows Custom Tile - Fatal Error",
            };
            var crashContent = new StackPanel()
            {
                Padding = new Thickness(32),
            };
            var title = new TextBlock()
            {
                Text = "Fatal Error",
                FontSize = 24,
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            var message = new TextBlock()
            {
                Text = $"{e.Message}\r\n{e.Exception.StackTrace}",
                FontSize = 16,
            };
            var button = new Button()
            {
                Content = Utility.I18N.Lang.Text("G_Detail"),
            };

            button.Click += async (_, _) =>
            {
                var time = DateTime.Now;
                var logFile = $"{time.Month}-{time.Day}.log";
                try
                {
                    var file = await Storage.Folder.GetFileAsync(logFile);
                    var log = await Launcher.LaunchFileAsync(file);
                }
                catch
                {
                    Log.Error($"Failed to open log file: {logFile}");
                    if(!await Launcher.LaunchFolderAsync(Storage.Folder))
                        Log.Error($"Failed to open AppData folder: {Storage.Folder.Path}");
                }
            };
            crashContent.Spacing = 16;
            crashContent.Children.Add(title);
            crashContent.Children.Add(message);
            crashContent.Children.Add(button);

            crashWindow.Content = crashContent;
            crashWindow.Activate();

            if (mainWindow != null)
            {
                mainWindow.Content = null;
                mainWindow.Close();
                mainWindow = null;
            }

        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Log.Info("Application launched (App.OnLaunched)");

            mainWindow = new MainWindow();

            var appActivateData = AppInstance.GetCurrent().GetActivatedEventArgs().Data;
            if (appActivateData is ProtocolActivatedEventArgs protocolArgs)
            {
                if(ProtocolLaunch(protocolArgs.Uri))
                    mainWindow.Activate();
                return;
            }

            if (NeedShowWindow())
                mainWindow.Activate();
            
        }

        /// <returns>Display MainWindow or not</returns>
        private bool ProtocolLaunch(Uri? uri)
        {
            Log.Info($"Application launched with Protocol: {uri}");
            switch (uri?.Host.ToLower())
            {
                case "start":
                    return true;
                case "reset":
                    Settings.ResetAll();
                    return false;
                default:
                    return false;
            }
        }

        /// <returns>Display MainWindow or not</returns>
        private bool NeedShowWindow()
        {
            if (!ExcuteLaunchCommand())
                return true;
            return Settings.Instance.LaunchAlwaysShowWindow;
        }

        /// <returns>Excute launch command failed or success</returns>
        public static bool ExcuteLaunchCommand()
        {
            var command = Settings.Instance.LaunchCommand;
            if (string.IsNullOrWhiteSpace(command))
                return false;
            
            try
            {
                switch (Settings.Instance.LaunchCommandType)
                {
                    case "uri":
                        Launcher.LaunchUriAsync(new Uri(command)).Wait();
                        return true;
                    case "cmd":
                        System.Diagnostics.Process.Start(command);
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error while excuting [{command}] Exception: {e.Message}");
            }
            return false;
        }

        public static bool Administrator()
        {
            WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RequestRestart(string arguments)
        {
            var ret = AppInstance.Restart(arguments);
            if (ret != Windows.ApplicationModel.Core.AppRestartFailureReason.RestartPending)
                Log.Info("Pending restart");
            else Log.Error($"Application restart failed: {ret}");
        }

    }
}
