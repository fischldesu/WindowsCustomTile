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
    namespace Pages
    {
        public enum PageType
        {
            Main,
            XmlEditor,
            Interval,
            HTTPXmlServer,
            Settings
        }
    }

    public enum WindowBackdropType
    {
        None,
        Mica,
        Acrylic,
    }
    public sealed partial class MainWindow : Window
    {
        public NavigationViewPaneDisplayMode PaneDisplayMode
        {
            get => view.PaneDisplayMode;
            set => view.PaneDisplayMode = value;
        }
        public WindowBackdropType            BackdropType
        {
            get
            {
                var currentBackdrop = this.SystemBackdrop;
                var name = string.Empty;
                if (currentBackdrop != null)
                    name = currentBackdrop.GetType().Name;

                return name switch
                {
                    nameof(MicaBackdrop) => WindowBackdropType.Mica,
                    nameof(DesktopAcrylicBackdrop) => WindowBackdropType.Acrylic,
                    _ => WindowBackdropType.None
                };
            }
            set
            {
                if (value != BackdropType)
                {
                    this.SystemBackdrop = value switch
                    {
                        WindowBackdropType.Mica => new MicaBackdrop(),
                        WindowBackdropType.Acrylic => new DesktopAcrylicBackdrop(),
                        _ => null
                    };
                }
                
            }
        }
        public ElementTheme                  Theme
        {
            get => view.RequestedTheme;
            set
            {
                if (view.RequestedTheme != value)
                    view.RequestedTheme = value;
            }
        }
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public readonly Pages.HomePage main = new();
        public readonly Pages.XmlEditor xmlEditor = new();
        public readonly Pages.Interval interval = new();
        public readonly Pages.HTTPXmlServer httpsrv = new();

        private void NavigateToContent(Page pageInstance)
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
            if (args.IsSettingsSelected) NavigateToContent(new Pages.Settings());
            else
            {
                var selected = args.SelectedItem as NavigationViewItem;
                if (selected != null)
                {
                    Page page = (selected.Tag as string) switch
                    {
                        "main" => main,
                        "xmlEditor" => xmlEditor,
                        "interval" => interval,
                        "httpsrv" => httpsrv,
                        _ => main,
                    };
                    NavigateToContent(page);
                }
            }
        }

        private void view_Loaded(object sender, RoutedEventArgs e)
        {
            view.SelectedItem = navigationDefaultItem;
        }
    }
}
