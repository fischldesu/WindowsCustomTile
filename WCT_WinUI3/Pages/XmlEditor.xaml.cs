using Fischldesu.WCTCore.Tile;
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

namespace WCT_WinUI3.Pages
{
    public sealed partial class XmlEditor : Page
    {
        readonly XmlDocument xmlDoc = new();
        public XmlEditor()
        {
            this.InitializeComponent();
        }

        public async Task<bool> Submit(string xmlText)
        {
            var ret = false;
            try
            {
                if (xmlText.Length < 4)
                    throw new ArgumentException("Invalid Xml text");
                xmlDoc.LoadXml(xmlText);

                ret = await Utility.AppContentDialog.ShowAsync(Utility.I18N.Lang.Text("Dialog_AskApplyChanges"), xmlText);
            }
            catch (Exception)
            {
                App.mainWindow?.ShowInfoBand(string.Empty,
                    Utility.I18N.Lang.Text("Info_InvalidXmlText"),
                    InfoBarSeverity.Error);
            }
            if (ret)
            {
                TileHelper.SetTileXml(xmlDoc);
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Success"),
                    Utility.I18N.Lang.Text("Info_ChangesApplied"),
                    InfoBarSeverity.Success);
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
