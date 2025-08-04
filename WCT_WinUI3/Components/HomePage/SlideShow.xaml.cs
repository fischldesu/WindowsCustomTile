using Fischldesu.WCTCore.Tile;
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
using WCT_WinUI3.Utility;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Components.HomePage
{
    public sealed partial class SlideShow : UserControl
    {
        public SlideShowTilePreviewer? CurrentPreviewer
        {
            get
            {
                return PreviewRadioButtonSelector.SelectedIndex switch
                {
                    0 => SlideShowLarge,
                    1 => SlideShowWide,
                    2 => SlideShowMedium,
                    _ => null
                };
            }
        }

        public string[] SourceStrings
        {
            get
            {
                string[] sourceStrings = [];
                foreach (var item in SourcePathInputs.Items)
                    if (item is ImagePathInput imagePathInput)
                        sourceStrings.Append(imagePathInput.SourceString);
                
                return sourceStrings;
            }
        }

        public List<ImageSource> ImageSources
        {
            get
            {
                List<ImageSource> sourceImages = [];
                foreach (var item in SourcePathInputs.Items)
                    if (item is ImagePathInput imagePathInput)
                    {
                        var sourceImage = imagePathInput.SourceImage;

                        if (sourceImage != null)
                            sourceImages.Add(sourceImage);
                        if (sourceImages.Count >= 12)
                            return sourceImages;
                    }

                return sourceImages;
            }
        }

        public ImagePathInput NewItem(string? path = null)
        {
            var sourceInput = path == null ? new ImagePathInput() : new ImagePathInput(path);

            SourcePathInputs.Items.Add(sourceInput);
            sourceInput.RequestRemove += (_, _) => SourcePathInputs.Items.Remove(sourceInput);

            return sourceInput;
        }

        public SlideShow()
        {
            this.InitializeComponent();
            NewItem();
        }

        private void Submit_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            var sources = ImageSources;
            
            if (sources.Count <= 0)
            {
                App.mainWindow?.ShowInfoBand(null,
                    "No valid images provided", InfoBarSeverity.Warning);
                return;
            }

            XmlDocument xmlDocument = new();
            String slideshow = string.Empty;

            foreach (var item in SourcePathInputs.Items)
                if (item is ImagePathInput imagePathInput && imagePathInput.Valid)
                    slideshow += $"<image src='{imagePathInput.SourceString}' />";

            var xmlString =
                "<tile><visual>" +
                $"<binding template='{CurrentPreviewer?.Tag}' hint-presentation='photos' >" +
                $"{slideshow}</binding>" +
                "</visual></tile>";

            try
            {
                xmlDocument.LoadXml(xmlString);
                TileHelper.SetTileXml(xmlDocument);
            }
            catch
            {
                
                return;
            }
            App.mainWindow?.ShowInfoBand("Success",
                "Slide Show tile set",
                InfoBarSeverity.Success);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (SourcePathInputs.Items.Count >= 12)
                App.mainWindow?.ShowInfoBand(null, "Max limited to 12", InfoBarSeverity.Warning);
            else NewItem();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SourcePathInputs.Items.Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SourcePathInputsContainer.MaxHeight = ActualSize.Y;
            if (sender is UserControl userControl)
                userControl.Loaded -= UserControl_Loaded;
        }

        private void SlideShow_RequestPreview(object sender, EventArgs e)
        {
            if (sender is not SlideShowTilePreviewer previewer) return;
            
            if (previewer.IsPreviewing) previewer.StopPreview();
            else
            {
                previewer.SetSources(ImageSources);
                previewer.StartPreview();
            }

        }

        private async void Grid_Drop(object sender, DragEventArgs dragEvent)
        {
            if (!dragEvent.DataView.Contains(StandardDataFormats.StorageItems)) return;
            
            var items = await dragEvent.DataView.GetStorageItemsAsync();
            foreach (var item in items)
            {
                NewItem(item.Path);
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
        }

    }
}
