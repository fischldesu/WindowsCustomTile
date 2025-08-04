﻿using System;
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
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public static MainWindow? mainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Log.Fatal($"App.UnhandledException {e.Exception.Message}\r\n{e.Exception.StackTrace}");
            
            var logWindow = new LogWindow();

            mainWindow?.Close();
            logWindow.Closed += (_, _) => Exit();

            logWindow.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Log.Info("Application launched (App.OnLaunched)");

            mainWindow = new MainWindow();

            var protocalLaunch = false;
            if (AppInstance.GetCurrent().GetActivatedEventArgs().Data is ProtocolActivatedEventArgs protocolArgs && protocolArgs.Uri != null)
                protocalLaunch = ProtocolLaunch(protocolArgs.Uri);

            if (protocalLaunch || !DoLaunchCommand() || Settings.Instance.LaunchAlwaysShowWindow)
                mainWindow.Activate();
            else
                Exit();
        }

        private bool ProtocolLaunch(Uri uri)
        {
            Log.Info($"Application launched with Protocol: {uri}");
            switch (uri.Host.ToLower())
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

        public static bool DoLaunchCommand()
        {
            var ret = false;
            var settings = Settings.Instance;
            try
            {
                if (settings.LaunchCommand is string command)
                {
                    var commandType = settings.LaunchCommandType;
                    switch (commandType)
                    {
                        case "uri":
                            Launcher.LaunchUriAsync(new Uri(command)).Wait();
                            ret = true;
                            break;
                        case "cmd":
                            System.Diagnostics.Process.Start(command);
                            ret = true;
                            break;
                        default:
                            Log.Warning($"Unknown launch command type:{(commandType is string t ? t : "UnknownType")}");
                            ret = false;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error while doing Launch Command {e.Message}");
            }

            return ret;
        }

        public static bool Administrator()
        {
            WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RequestRestart(string arguments)
        {
            var ret = Microsoft.Windows.AppLifecycle.AppInstance.Restart(arguments);
            if (ret != Windows.ApplicationModel.Core.AppRestartFailureReason.RestartPending)
                Log.Info("Pending restart");
            else Log.Error($"Application restart failed:{ret}");
        }

    }
}
