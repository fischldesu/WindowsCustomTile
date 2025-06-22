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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LiveTileWinUI3
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public Pages.Main main = new();
        public Pages.Interval interval = new();
        public Pages.HTTPXmlServer httpsrv = new();

        public void Navigate(Page pageInstance)
        {
            var animation = new Utility.Animation.Appearance(pageInstance)
            {
                Opacity = 0.2,
                Translate = 64,
            };
            frame.Content = pageInstance;
            animation.Start(false);
        }


        private void view_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected) frame.Navigate(typeof(Pages.Settings));
            else
            {
                var selected = args.SelectedItem as NavigationViewItem;
                if (selected != null)
                {
                    Page page = (selected.Tag as string) switch
                    {
                        "main" => main,
                        "interval" => interval,
                        "httpsrv" => httpsrv,
                        _ => main,
                    };
                    Navigate(page);
                }
            }
        }

        private void view_Loaded(object sender, RoutedEventArgs e)
        {
            view.SelectedItem = navigationDefaultItem;
        }
    }
}
