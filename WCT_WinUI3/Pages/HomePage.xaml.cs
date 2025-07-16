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
using WCT_WinUI3.Components;
using WCT_WinUI3.Utility;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            previewer_editBySize_combo.ItemsSource = Enum.GetValues(typeof(TileSize));
            previewer_editBySize_combo.SelectedItem = TileSize.Large;
            previewer.ImageButtonFlyoutClick += (sender, size) =>
            {
                previewer_editBySize_combo.SelectedItem = size;
                var tbox = GetPreviewerEditBySize_TextBox(size);
                if (tbox != null)
                {
                    tbox.Focus(FocusState.Pointer);
                    tbox.SelectionLength = tbox.Text.Length;
                }
            };

            var type = App.Settings.LaunchCommandType;
            if (type != null)
            {
                if (type == "uri") LaunchCmdType_uri.IsChecked = true;
                else if (type == "cmd") LaunchCmdType_cmd.IsChecked = true;
            }
        }

        public void Previewer_Clear(TileSize size)
        {
            switch (size)
            {
                case TileSize.Large:
                    previewer.Source.Large = string.Empty;
                    break;
                case TileSize.Medium:
                    previewer.Source.Medium = string.Empty;
                    break;
                case TileSize.Wide:
                    previewer.Source.Wide = string.Empty;
                    break;
                case TileSize.Small:
                    previewer.Source.Small = string.Empty;
                    break;
            }
        }

        public void Previewer_Submit()
        {
            var xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();

            var xml = TileXmlEditor.FromTemplate(
                $"<image src=\"{previewer.Source.Small}\" placement='background' />",
                $"<image src=\"{previewer.Source.Medium}\" placement='background' />",
                $"<image src=\"{previewer.Source.Wide}\" placement='background' />",
                $"<image src=\"{previewer.Source.Large}\" placement='background' />");

            try
            {
                xmlDoc.LoadXml(xml);
                if (xmlDoc != null)
                    TileHelper.SetXml(xmlDoc);
            }
            catch
            {
                App.mainWindow?.ShowInfoBand("Failed",
                    "Image tile quick set failed, check your Uri",
                    InfoBarSeverity.Error);
                return;
            }
            App.mainWindow?.ShowInfoBand("Success",
                "Image tile quick set ok",
                InfoBarSeverity.Success);
        }

        public TextBox? GetPreviewerEditBySize_TextBox(TileSize? size)
        {
            return (size) switch
            {
                TileSize.Large => previewer_editBySize_large,
                TileSize.Wide => previewer_editBySize_wide,
                TileSize.Medium => previewer_editBySize_medium,
                TileSize.Small => previewer_editBySize_small,
                _ => null
            };
        }

        private void previewer_editBySize_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in previewer_editBySize.Children)
                if (item is TextBox tbox)
                    tbox.Visibility = Visibility.Collapsed;

            var target = GetPreviewerEditBySize_TextBox((TileSize?)previewer_editBySize_combo.SelectedItem);
            if (target != null)
                target.Visibility = Visibility.Visible;

        }

        private void previewer_editBySize_apply_Click(object sender, RoutedEventArgs e)
        {
            var selected = (TileSize?)previewer_editBySize_combo.SelectedItem;
            var target = GetPreviewerEditBySize_TextBox(selected);
            if (target != null)
            {
                switch (selected)
                {
                    case TileSize.Large:
                        previewer.Source.Large = target.Text;
                        break;
                    case TileSize.Medium:
                        previewer.Source.Medium = target.Text;
                        break;
                    case TileSize.Wide:
                        previewer.Source.Wide = target.Text;
                        break;
                    case TileSize.Small:
                        previewer.Source.Small = target.Text;
                        break;
                }
            }
        }

        private async void pickImageFile_Click(object sender, RoutedEventArgs e)
        {
            string[] extensions = ["*", ".jpg", ".jpeg", ".bmp", ".png"];
            var path = await FilePicker.PickSingleFile(extensions, true, Windows.Storage.Pickers.PickerLocationId.PicturesLibrary);

            if (path != null && path != string.Empty)
            {
                if (previewer_pickImageFile == (Button)sender)
                {
                    previewer.Source.Large = path;
                    previewer.Source.Medium = path;
                    previewer.Source.Wide = path;
                    previewer.Source.Small = path;
                }
                else if (previewer_editBySize_pickFile == (Button)sender)
                {
                    var tbox = GetPreviewerEditBySize_TextBox((TileSize?)previewer_editBySize_combo.SelectedItem);
                    if (tbox != null)
                        tbox.Text = path;
                }
            }
        }

        private void previewer_submit_Click(object sender, RoutedEventArgs e)
        {
            Previewer_Submit();
        }

        private void previewer_clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (TileSize size in Enum.GetValues(typeof(TileSize)))
                Previewer_Clear(size);
        }

        private void launchCmdApply_Click(object sender, RoutedEventArgs e)
        {
            var ret = false;
            var text = launchCmdInputer.Text;

            if (text != string.Empty)
            {
                var uri = LaunchCmdType_uri.IsChecked;
                var cmd = LaunchCmdType_cmd.IsChecked;
                var asw = alwaysShowWindow.IsChecked;
                if (uri != null && cmd != null && asw != null && ((bool)uri || (bool)cmd))
                {
                    App.Settings.LaunchCommand = text;
                    App.Settings.LaunchCommandType = (bool)uri ? "uri" : "cmd";
                    App.Settings.LaunchAlwaysShowWindow = (bool)asw;

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
    }
}
