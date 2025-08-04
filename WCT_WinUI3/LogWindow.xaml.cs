using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Fischldesu.WCTCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogWindow : Window
    {
        public LogWindow()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1024, 768));
            this.AppWindow.SetIcon("Assets/WindowIcon.ico");

            Initilaize();

            Log.NewMessage += (_, msg) => { if (autoUpdate.IsChecked is bool u && u) Append(msg); };
        }

        private void Initilaize()
        {
            foreach (var item in Log.History)
                Append(item);
        }

        private void Append(LogMessage msg)
        {
            var textBlock = new TextBlock() { IsTextSelectionEnabled = true };

            var danger = msg.Level >= LogLevel.WARNING;
            var levelRun = new Run() 
            { 
                Text = $"[{msg.Level}]".PadRight(8),
                FontFamily = new FontFamily("Cascadia Mono"),
            };
            if (danger)
                levelRun.Foreground = new SolidColorBrush(Colors.Red);

            textBlock.Inlines.Add(levelRun);
            textBlock.Inlines.Add(new Run() { Text = $" [{msg.Time:HH:mm:ss}] ", FontFamily = new FontFamily("Cascadia Mono") });
            textBlock.Inlines.Add(new Run() { Text = msg.Message });

            logMessageStack.Children.Add(textBlock);
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            logMessageStack.Children.Clear();
            Initilaize();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            var text = string.Empty;
            foreach (var item in Log.History.TakeLast(128))
                text += item.ToString() + Environment.NewLine;
            
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
    }
}

