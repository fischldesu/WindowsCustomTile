using Fischldesu.WCTCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

using CoreSettings = Fischldesu.WCTCore.Settings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            DataContext = Fischldesu.WCTCore.Settings.Instance;
            var type = Fischldesu.WCTCore.Settings.Instance.LaunchCommandType;
            if (type != null)
            {
                if (type == "uri") LaunchCmdType_uri.IsChecked = true;
                else if (type == "cmd") LaunchCmdType_cmd.IsChecked = true;
            }
        }

        private void launchCmdApply_Click(object sender, RoutedEventArgs e)
        {
            var settings = Fischldesu.WCTCore.Settings.Instance;
            var ret = false;
            var text = launchCmdInputer.Text;

            if (text != string.Empty)
            {
                var uri = LaunchCmdType_uri.IsChecked;
                var cmd = LaunchCmdType_cmd.IsChecked;
                var asw = alwaysShowWindow.IsChecked;
                if (uri != null && cmd != null && asw != null && ((bool)uri || (bool)cmd))
                {
                    settings.LaunchCommand = text;
                    settings.LaunchCommandType = (bool)uri ? "uri" : "cmd";
                    settings.LaunchAlwaysShowWindow = (bool)asw;

                    ret = true;
                }
            }

            if (ret) App.mainWindow?.ShowInfoBand("Success",
                "Launch command applied",
                InfoBarSeverity.Success);
            else App.mainWindow?.ShowInfoBand(string.Empty,
                "Failed to apply for the launch command",
                InfoBarSeverity.Error);
        }

        private void quickFunctionPivot_Loaded(object? sender, RoutedEventArgs? e)
        {
            void foreachContent(Action<FrameworkElement> action)
            {
                foreach (var item in quickFunctionPivot.Items)
                    if (item is PivotItem pivotItem && pivotItem.Content is FrameworkElement element)
                        action(element);
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                double height = 0;
                foreachContent(ele =>
                {
                    double height_ = ele.ActualSize.Y;
                    if (height_ > height)
                        height = height_;
                });
                if (height > 0)
                    foreachContent(ele => ele.MinHeight = height);
            });
        }

        private async void LaunchCommandFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, LaunchCmd_Excute))
            {
                var uri = LaunchCmdType_uri.IsChecked ?? false;
                var cmd = LaunchCmdType_cmd.IsChecked ?? false;
                var command = launchCmdInputer.Text;

                try
                {
                    if (uri)
                    {
                        Log.Info($"Excute Uri command {command}");
                        if (!await Launcher.LaunchUriAsync(new Uri(command)))
                            throw new InvalidOperationException("Launcher.LaunchUriAsync failed");
                    }
                    else if (cmd)
                    {
                        Log.Info($"Excute Command {command}");
                        System.Diagnostics.Process.Start(command);
                        App.mainWindow?.ShowInfoBand("Info", "Command Excuted", InfoBarSeverity.Informational);
                    }
                }
                catch (Exception ex)
                {
                    App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"), $"Command Excute Failed {ex.Message}", InfoBarSeverity.Error);
                }

            }
            else if (ReferenceEquals(sender, LaunchCmd_Reset))
            {
                CoreSettings.Instance.LaunchCommand = null;
                CoreSettings.Instance.LaunchCommandType = null;
                CoreSettings.Instance.LaunchAlwaysShowWindow = false;

                launchCmdInputer.Text = string.Empty;
                LaunchCmdType_uri.IsChecked = false;
                LaunchCmdType_cmd.IsChecked = false;
                alwaysShowWindow.IsChecked = false;

                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Success"), "Reset Launch command settings", InfoBarSeverity.Success);
            }
        }
    }
}
