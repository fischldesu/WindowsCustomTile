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
using WCT_WinUI3.Utility;
using Fischldesu.WCTCore;
using WCT_WinUI3.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3
{
    namespace Pages
    {
        public enum PageType
        {
            Main,
            XmlEditor,
            Scheduled,
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
        public ElementTheme Theme
        {
            get => root.RequestedTheme;
            set
            {
                if (root.RequestedTheme != value)
                {
                    root.RequestedTheme = value;
                    if (value != ElementTheme.Default)
                    {
                        AppWindow.TitleBar.ButtonForegroundColor = value == ElementTheme.Dark ?
                            Microsoft.UI.Colors.White : Microsoft.UI.Colors.Black;
                    }
                }
            }
        }

        public string WindowTitle
        {
            get => appTitleText.Text;
            private set => appTitleText.Text = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            SetTitleBar(appTitleContainer);

            WindowTitle = Title;
            ExtendsContentIntoTitleBar = true;

            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;
            AppWindow.SetIcon("Assets/WindowIcon.ico");
        }

        public readonly Pages.HomePage Main = new();
        public readonly Pages.XmlEditor XmlEditor = new();
        public readonly Pages.Scheduled Scheduled = new();
        public readonly Pages.HTTPXmlServer HTTPXmlServer = new();

        private Action? infoBandCloseCancelToken;

        public void ShowInfoBand(string? title, string? message, InfoBarSeverity severity, int closeTimeSecond = 6)
        {
            infoBand.Title = title;
            infoBand.Message = message ?? string.Empty;
            infoBand.Severity = severity;
            infoBand.IsOpen = true;
            if (closeTimeSecond > 0)
            {
                infoBandCloseCancelToken?.Invoke();
                infoBandCloseCancelToken = Timer.SetTimeout(() =>
                {
                    infoBandCloseCancelToken = null;
                    infoBand.IsOpen = false;
                }, TimeSpan.FromSeconds(closeTimeSecond));
            }
            var logLevel = severity switch
            {
                InfoBarSeverity.Warning => LogLevel.WARNING,
                InfoBarSeverity.Error => LogLevel.ERROR,
                _ => LogLevel.INFO,
            };
            Log.Append($"{Title} {message}", logLevel);
        }

        public void NavigateTo(PageType page)
        {
            if (page == PageType.Settings)
            {
                view.SelectedItem = view.SettingsItem;
                return;
            }

            var viewItems = view.MenuItems.OfType<NavigationViewItem>();
            var viewItem = viewItems.FirstOrDefault(item => item.Tag.ToString() == page.ToString());
            if (viewItem != null)
                view.SelectedItem = viewItem;
        }

        private void NavigationViewDisplayContent(Page pageInstance)
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
            if (args.IsSettingsSelected) NavigationViewDisplayContent(new Pages.Settings());
            else
            {
                var selected = args.SelectedItem as NavigationViewItem;
                if (selected != null)
                {
                    Page page = (selected.Tag as string) switch
                    {
                        nameof(Main) => Main,
                        nameof(XmlEditor) => XmlEditor,
                        nameof(Scheduled) => Scheduled,
                        nameof(HTTPXmlServer) => HTTPXmlServer,
                        _ => Main,
                    };
                    NavigationViewDisplayContent(page);
                }
            }
        }

        private void view_Loaded(object sender, RoutedEventArgs e)
        {
            view.SelectedItem = navigationDefaultItem;
        }

        private void infoBand_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            infoBandCloseCancelToken = null;
        }
    }
}
