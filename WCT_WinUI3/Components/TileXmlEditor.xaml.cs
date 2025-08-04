using Fischldesu.WCTCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace WCT_WinUI3.Components
{
    public sealed partial class TileXmlEditor : UserControl
    {
        private static Action? copyAction;

        public TextBox Editor { get { return edit_total; } }
        public bool SelectedSingleEditor { 
            set {
                if (value)
                    pivot.SelectedIndex = 0;
                else
                    pivot.SelectedIndex = 1;
            }
            get { return pivot.SelectedIndex == 0; }
        }

        public TileXmlEditor()
        {
            this.InitializeComponent();
        }

        public static string FromTemplate(string small, string mid, string wide, string large)
        {
            return $@"<tile>
  <visual>
    <binding template='TileSmall'>
      {small}
    </binding>
    <binding template='TileMedium'>
      {mid}
    </binding>
    <binding template='TileWide'>
      {wide}
    </binding>
    <binding template='TileLarge'>
      {large}
    </binding>
  </visual>
</tile>
";
        }


        public string GetXml()
        {
            if (pivot.SelectedIndex == 0)
            {
                if (edit_single_small.Text.Length < 2 &&
                    edit_single_medium.Text.Length < 2 &&
                    edit_single_wide.Text.Length < 2 &&
                    edit_single_large.Text.Length < 2)
                return string.Empty;

                return FromTemplate(
                    edit_single_small.Text,
                    edit_single_medium.Text,
                    edit_single_wide.Text,
                    edit_single_large.Text);
            }
            return edit_total.Text;
        }

        public void SetXml(Windows.Data.Xml.Dom.XmlDocument xmlDocument)
        {
            edit_total.Text = xmlDocument.GetXml();
            pivot.SelectedIndex = 1;
        }

        private void edit_total_showTemplate_Click(object sender, RoutedEventArgs e)
        {
            var spaces = "    ";
            edit_total.Text = FromTemplate(spaces, spaces, spaces, spaces);
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            
            var dataPackage = new DataPackage();
            dataPackage.SetText(FromTemplate(
                    edit_single_small.Text,
                    edit_single_medium.Text,
                    edit_single_wide.Text,
                    edit_single_large.Text));

            Clipboard.SetContent(dataPackage);

            tip.IsOpen = true;
            copyAction?.Invoke();
            copyAction = Utility.Timer.SetTimeout(() =>
            {
                tip.IsOpen = false;
            }, TimeSpan.FromSeconds(4));
        }

        private async void pivot_Drop(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
                return;

            var storageItems = await e.DataView.GetStorageItemsAsync();
            if (storageItems.Count < 1)
                return;

            var storageItem = storageItems[0];
            if (storageItem != null && storageItem is IStorageFile file)
            {
                try
                {
                    var text = await FileIO.ReadTextAsync(file);
                    var xmlDocument = new Windows.Data.Xml.Dom.XmlDocument();

                    xmlDocument.LoadXml(text);
                    SetXml(xmlDocument);
                    e.Handled = true;
                }
                catch
                {
                    Log.Error($"Failed to load XML from file {file.Name}");
                }
            }

        }

        private void pivot_DragOver(object _, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }
}
