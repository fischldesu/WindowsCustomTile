using ABI.System;
using CommunityToolkit.WinUI.Controls;
using LiveTileWinUI3.Utility.Log;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LiveTileWinUI3.Pages
{

    public sealed partial class Settings : Page
    {
        private LogWindow? logWindow;
        public Settings()
        {
            this.InitializeComponent();
            backdropCombo.ItemsSource = Enum.GetValues(typeof(WindowBackdropType));
            paneDisplayModeCombo.ItemsSource = Enum.GetValues(typeof(NavigationViewPaneDisplayMode));
            themeSelectorCombo.ItemsSource = Enum.GetValues(typeof(ElementTheme));

            spVersion.Text = "(beta)";
        }

        public void AskResetAll()
        {
            resetAll.Flyout?.ShowAt(resetAll);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in settingsCards.Children.Cast<FrameworkElement?>())
            {
                var itemType = item?.GetType();
                if (item != null && (itemType == typeof(SettingsCard) || itemType == typeof(SettingsExpander)))
                    item.Margin = new Thickness(0, 4, 0, 4);
            }
            languageCombo.SelectedIndex = 0;
        }

        private void showLogWindow_Click(object sender, RoutedEventArgs e)
        {
            logWindow ??= new LogWindow();
            logWindow.Activate();
            logWindow.Closed += (obj, _) =>
            {
                logWindow = null;
            };
        }

        private void resetAll_Click(object sender, RoutedEventArgs e)
        {
            resetAll.Flyout?.Hide();
            App.Settings.ResetAll();
        }

        private void aboutExpander_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (sender is Expander expander && expander.Content is UIElement content)
                if (args.NewSize.Height != args.PreviousSize.Height && expander.IsExpanded)
                    scroller.ScrollToVerticalOffset(scroller.VerticalOffset + content.ActualSize.Y + 4);
        }

        private void aboutExpander_Fold(Expander expander, ExpanderCollapsedEventArgs args)
        {
            if (expander.Content is UIElement content)
                scroller.ScrollToVerticalOffset(scroller.VerticalOffset - content.ActualSize.Y - 4);
        }

        private void languageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string langTag = languageCombo.SelectedIndex switch
            {
                1 => "en-US", // English
                2 => "zh-CN", // Simplified Chinese
                _ => string.Empty
            };

            if (langTag != Utility.I18N.Lang.Primary)
            {
                Utility.I18N.Lang.Primary = langTag;
                restart.Visibility = Visibility.Visible;
            }
        }
    }
}
