using Microsoft.UI.Windowing;
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
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LiveTileWinUI3.Pages
{
    public sealed partial class XmlEditor : Page
    {
        public readonly InfoBar InfoBar;
        readonly XmlDocument xmlDoc = new();
        public XmlEditor()
        {
            this.InitializeComponent();
            InfoBar = info;
        }

        public async Task<bool> Submit(string xmlText)
        {
            var ret = false;
            try
            {
                if (xmlText.Length < 4)
                    throw new ArgumentException("Invalid Xml text");
                xmlDoc.LoadXml(xmlText);

                ret = await Utility.AppContentDialog.ShowAsync("Apply Changes?", xmlText);
            }
            catch (Exception)
            {
                InfoBar.Severity = InfoBarSeverity.Error;
                InfoBar.Message = "Invalid Xml text content, changes will not be applied.";
                InfoBar.Title = "Parse Error";
                InfoBar.IsOpen = true;
            }
            if (ret)
            {
                Utility.TileHelper.SetXml(xmlDoc);
                InfoBar.Severity = InfoBarSeverity.Success;
                InfoBar.Message = "Xml parsed ok, tile updated.";
                InfoBar.Title = "Success";
                InfoBar.IsOpen = true;
            }
            return ret;
        }

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            await Submit(editor.GetXml());
        }

        private async void selectFile_Click(object sender, object e)
        {
            var xmlText = await Utility.FilePicker.ReadText([".xml", ".txt", "*"]);
            if (xmlText != null)
                await Submit(xmlText);
        }
        
    }
}
