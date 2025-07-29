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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Components
{
    public sealed partial class ImagePathInput : UserControl
    {

        public event EventHandler<ImagePathInput>? RequestRemove;
        public ImageSource? SourceImage 
        { 
            get
            {
                hintText.Visibility = Visibility.Collapsed;
                var converter = new Utility.Convert.StringToImageSource();
                var result = (ImageSource?)converter.Convert(SourceString, typeof(ImageSource), null, null);
                hintText.Visibility = result == null ? Visibility.Visible : Visibility.Collapsed;
                return result;
            }
        }
        public String SourceString => input.Text;
        public bool Valid => CheckValid();

        public ImagePathInput()
        {
            this.InitializeComponent();
        }

        public ImagePathInput(string path)
        {
            this.InitializeComponent();
            input.Text = path;
        }

        public bool CheckValid()
        {
            var result = SourceImage != null;
            return result;
        }

        private async void PickImageFile_Click(object sender, RoutedEventArgs e)
        {
            string[] extensions = ["*", ".jpg", ".jpeg", ".bmp", ".png"];
            var path = await FilePicker.PickSingleFile(extensions, true, Windows.Storage.Pickers.PickerLocationId.PicturesLibrary);

            if (!string.IsNullOrEmpty(path))
                input.Text = path;
        }

        private void CheckPathValid_Click(object sender, RoutedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                CheckValid();
            });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            RequestRemove?.Invoke(this, this);
        }

        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            hintText.Visibility = Visibility.Collapsed;
        }

        private async void Grid_Drop(object sender, DragEventArgs dragEvent)
        {
            dragEvent.Handled = true;
            if (dragEvent.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await dragEvent.DataView.GetStorageItemsAsync();
                input.Text = items[0]?.Path;
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
        }
    }
}
