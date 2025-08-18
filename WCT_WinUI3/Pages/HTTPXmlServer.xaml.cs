using Fischldesu.WCTCore.Tile;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
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
using Windows.System.Profile;
using WCT_WinUI3.Components.HTTPXmlServer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Pages
{
    public sealed partial class HTTPXmlServer : Page
    {
        public HTTPXmlServer()
        {
            this.InitializeComponent();
            updateRecurrence.ItemsSource = Enum.GetValues(typeof(Windows.UI.Notifications.PeriodicUpdateRecurrence));
            updateRecurrence.SelectedIndex = 0;
            NewItem("uri 1");
        }

        public XmlUriInput NewItem(string header = "")
        {
            if(header == string.Empty)
                header = $"uri {UriInputListView.Items.Count + 1}";
            //header = $"uri {inputStack.Children.Count + 1}";

            var item = new XmlUriInput()
            {
                Header = header,
                Margin = new Thickness(0, 0, 0, 8)
            };

            item.RequestRemove += RequestRemoveOne;

            UriInputListView.Items.Add(item);
            //inputStack.Children.Add(item);
            return item;
        }

        public async Task Submit()
        {
            var uriList = new List<Uri>();
            var invalids = new List<string>();

            foreach (var item in UriInputListView.Items)
                if (item is XmlUriInput uriInput)
                {
                    if (uriInput.Uri != null) uriList.Add(uriInput.Uri);
                    else invalids.Add(uriInput.Header);
                }

            if (UriInputListView.Items.Count < 0)
            {
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Warning"),
                    Utility.I18N.Lang.Text("Info_NoValidSrv"),
                    InfoBarSeverity.Error);
                return;
            }

            var content = new TextBlock()
            {
                Margin = new Thickness(0, 8, 0, 8),
            };

            void TextTitle(string? text)
            {
                if (text == null) return;

                content.Inlines.Add(new Run()
                {
                    Text = text,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12
                });
            }
            void TextContent(string? text)
            {
                if (text == null) return;

                content.Inlines.Add(new Run()
                {
                    Text = text
                });
            }
            void TextLineBreak() => content.Inlines.Add(new LineBreak());

            TextTitle($"{Utility.I18N.Lang.Text("Dialog_ValidUri")}\n");
            TextContent($"{Utility.I18N.Lang.Text("Dialog_Count")} {uriList.Count}\n");
            TextLineBreak();
            TextTitle($"{Utility.I18N.Lang.Text("Dialog_TimeSpan")}\n");
            TextContent($"{Utility.I18N.Lang.Text("Dialog_Every")} {updateRecurrence.SelectedItem}\n");
            if (invalids.Count > 0)
            {
                TextLineBreak();
                TextTitle($"{Utility.I18N.Lang.Text("Dialog_InvalidUri")}\n");
                TextContent($"{Utility.I18N.Lang.Text("Dialog_Count")}: {invalids.Count}\n{string.Join(Environment.NewLine, invalids)}");
            }

            var ok = await Utility.AppContentDialog.ShowAsync(
                Utility.I18N.Lang.Text("Dialog_AskApplyChanges"),
                content);
            if (ok)
            {
                if (updateRecurrence.SelectedItem is Windows.UI.Notifications.PeriodicUpdateRecurrence recurrence)
                    if (uriList.Count == 1) TileHelper.SetPeriodicUpdateHTTPServer(uriList[0], recurrence);
                    else TileHelper.SetPeriodicUpdateHTTPServer(uriList.AsReadOnly(), recurrence);

                if (uriList.Count == 0) App.mainWindow?.ShowInfoBand("None of server URI is valid",
                    string.Empty,
                    InfoBarSeverity.Error);
                else App.mainWindow?.ShowInfoBand("Success",
                    $"Applied for your {uriList.Count} uri",
                    InfoBarSeverity.Success);
            }
        }

        private void newItem_Click(object sender, RoutedEventArgs e)
        {
            NewItem();
        }

        public void RequestRemoveOne(object? from, XmlUriInput? input)
        {
            if (input != null)
            {
                //var animation = new Utility.Animation.Disappear(input)
                //{

                //};
                //animation.Start(true).Completed +=(obj, _)=>{
                    UriInputListView.Items.Remove(input);
                //};
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            UriInputListView.Items.Clear();
        }

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            await Submit();
        }

        private void randomize_Click(object sender, RoutedEventArgs e)
        {
            if (UriInputListView.Items.Count <= 1)
                return;

            var list = UriInputListView.Items.Cast<UIElement>().ToList();

            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }

            UriInputListView.Items.Clear();
            foreach (var item in list)
            {
                UriInputListView.Items.Add(item);
                var animation = new Utility.Animation.Appearance(item)
                {
                    TransitionDuration = new Duration(TimeSpan.FromMilliseconds(300)),
                    Translate = 35,
                    Opacity = 0.2
                };
                animation.Start(true);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ulong.TryParse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion, out var versionNumber))
                if( ((versionNumber & 0x00000000FFFF0000L) >> 16) >= 22000) 
                    pageTemplate.ShowInfo(Utility.I18N.Lang.Text("G_Warning"),
                        Utility.I18N.Lang.Text("Page_HTTPSrv_Info_BadForWin11"),
                        InfoBarSeverity.Warning, false);
        }
    }
}
