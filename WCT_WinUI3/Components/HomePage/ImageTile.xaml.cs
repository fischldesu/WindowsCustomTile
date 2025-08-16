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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WCT_WinUI3.Utility;
using Fischldesu.WCTCore.Tile;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Components.HomePage
{
    public sealed partial class ImageTile : UserControl
    {
        public TileSize? SelectedSize
        {
            get
            {
                if (TileSizeLargeRadio.IsChecked)
                    return TileSize.Large;
                else if (TileSizeWideRadio.IsChecked)
                    return TileSize.Wide;
                else if (TileSizeMediumRadio.IsChecked)
                    return TileSize.Medium;
                else if (TileSizeSmallRadio.IsChecked)
                    return TileSize.Small;
                return null;
            }
            set
            {
                RadioMenuFlyoutItem? item = value switch
                {
                    TileSize.Small => TileSizeSmallRadio,
                    TileSize.Medium => TileSizeMediumRadio,
                    TileSize.Wide => TileSizeWideRadio,
                    TileSize.Large => TileSizeLargeRadio,
                    _ => null
                };

                if (item == null) return;
                item.IsChecked = true;
                item.UpdateLayout();
            }
        }

        public ImageTile()
        {
            this.InitializeComponent(); 
            previewer.ImageButtonFlyoutClick += (sender, size) =>
            {
                SelectedSize = size;
                var textBox = GetPreviewerEditBySize_TextBox(size);
                if (textBox == null) return;
                textBox.Focus(FocusState.Pointer);
                textBox.SelectionLength = textBox.Text.Length;
            };
        }

        private void Previewer_Clear(TileSize size)
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

        private void Previewer_Submit()
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
                TileHelper.SetTileXml(xmlDoc);
            }
            catch
            {
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"),
                    Utility.I18N.Lang.Text("Page_Home_ImageTile") + Utility.I18N.Lang.Text("G_UpdateFailed"),
                    InfoBarSeverity.Error);
                return;
            }
            App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Success"),
                Utility.I18N.Lang.Text("Page_Home_ImageTile") + Utility.I18N.Lang.Text("G_UpdateSuccess"),
                InfoBarSeverity.Success);
        }

        public TextBox? GetPreviewerEditBySize_TextBox(TileSize? size)
        {
            return (size) switch
            {
                TileSize.Large => EditBySizeLarge,
                TileSize.Wide => EditBySizeWide,
                TileSize.Medium => EditBySizeMedium,
                TileSize.Small => EditBySizeSmall,
                _ => null
            };
        }

        private void EditBySizeApply_Click(object sender, RoutedEventArgs e)
        {
            var selected = SelectedSize;
            var target = GetPreviewerEditBySize_TextBox(selected);
            if (target != null)
            {
                var text = target.Text ?? string.Empty;
                switch (selected)
                {
                    case TileSize.Large:
                        previewer.Source.Large = text;
                        break;
                    case TileSize.Medium:
                        previewer.Source.Medium = text;
                        break;
                    case TileSize.Wide:
                        previewer.Source.Wide = text;
                        break;
                    case TileSize.Small:
                        previewer.Source.Small = text;
                        break;
                }
            }
        }

        private async void PickImageFile_Click(object sender, RoutedEventArgs e)
        {
            string[] extensions = ["*", ".jpg", ".jpeg", ".bmp", ".png"];
            var path = await FilePicker.PickSingleFile(extensions, true, Windows.Storage.Pickers.PickerLocationId.PicturesLibrary);

            if (string.IsNullOrEmpty(path)) return;
            
            if (PickImageFile == (Button)sender)
            {
                previewer.Source.Large = path;
                previewer.Source.Medium = path;
                previewer.Source.Wide = path;
                previewer.Source.Small = path;
            }
            else if (sender is MenuFlyoutItem)
            {
                var textBox = GetPreviewerEditBySize_TextBox(SelectedSize);
                if (textBox != null)
                    textBox.Text = path;
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            Previewer_Submit();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (TileSize size in Enum.GetValues(typeof(TileSize)))
                Previewer_Clear(size);
        }
        
    }
}
