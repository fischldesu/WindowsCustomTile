using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Text;
using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System.Linq;
using WCT_WinUI3.Components;
using WCT_WinUI3.Utility;

namespace WCT_WinUI3.Pages
{
    public sealed partial class Interval : Page
    {

        public Interval()
        {
            this.InitializeComponent();
            timeFormat.Items.Add(Utility.I18N.Lang.Text("G_Hour"));
            timeFormat.Items.Add(Utility.I18N.Lang.Text("G_Minute"));
            timeFormat.Items.Add(Utility.I18N.Lang.Text("G_Second"));
            NewTab();
        }

        public TileXmlEditor NewTab()
        {
            var editor = new TileXmlEditor();
            var newItem = new TabViewItem()
            {
                Header = $"Tile {items.TabItems.Count + 1}",
                Content = editor
            };

            items.TabItems.Add(newItem);
            items.SelectedItem = newItem;
            items.TabCloseRequested += (sender, e) =>
            {
                sender.TabItems.Remove(e.Tab);
            };
            return editor;
        }

        public async Task<bool> SubmitXmlsTimer()
        {
            if (items.TabItems.Count == 0)
            {
                App.mainWindow?.ShowInfoBand(string.Empty,
                    "Theres no available sources",
                    InfoBarSeverity.Warning
                    );
                return false;
            }

            var xmlDocuments = new System.Collections.Generic.List<XmlDocument>();
            var invalids = new System.Collections.Generic.List<string>();
            foreach (var item_ in items.TabItems)
            {
                if (item_ is TabViewItem item && item.Content is Components.TileXmlEditor editor)
                {
                    try
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(editor.GetXml());
                        if (xmlDoc.DocumentElement != null)
                            xmlDocuments.Add(xmlDoc);
                    }
                    catch (Exception)
                    {
                        var itemHeader = item.Header.ToString();
                        if (itemHeader != null)
                            invalids.Add(itemHeader);
                    }
                }
            }

            if (xmlDocuments.Count == 0)
                return false;
            

            static Run TitleText(string text)
            {
                return new Run() { 
                    Text = text,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12 
                };
            }
            TextBlock content = new()
            {
                Margin = new Thickness(0, 16, 0, 0)
            };

            content.Inlines.Add(TitleText("Total valid Xml"));
            content.Inlines.Add(new LineBreak());
            content.Inlines.Add(new Run { Text = $"Count: {xmlDocuments.Count}" });
            content.Inlines.Add(new LineBreak());
            content.Inlines.Add(new LineBreak());
            content.Inlines.Add(TitleText("Time span"));
            content.Inlines.Add(new LineBreak());
            content.Inlines.Add(new Run { Text = $"Every {timeInput.Value} {timeFormat.SelectedItem.ToString()?.ToLower()}" });
            if (invalids.Count > 0)
            {
                content.Inlines.Add(new LineBreak());
                content.Inlines.Add(new LineBreak());
                content.Inlines.Add(TitleText("Total invalid Xml"));
                content.Inlines.Add(new Run { Text = $" Count: {invalids.Count}\n{string.Join(Environment.NewLine, invalids)}" });
            }

            if (await AppContentDialog.ShowAsync("Apply all changes?", content))
            {
                var timeSpan = timeFormat.SelectedIndex switch
                {
                    0 => TimeSpan.FromHours(timeInput.Value),
                    1 => TimeSpan.FromMinutes(timeInput.Value),
                    2 => TimeSpan.FromSeconds(timeInput.Value),
                    _ => TimeSpan.FromMinutes(timeInput.Value),
                };
                TileHelper.SetInterval([.. xmlDocuments], timeSpan);

                App.mainWindow?.ShowInfoBand("Success",
                    "All changes have applied",
                    InfoBarSeverity.Success);

                return true;
            }

            return false;
        }

        private void items_AddTabButtonClick(TabView sender, object args)
        {
            NewTab();
        }

        private void items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (items.SelectedItem is TabViewItem tab && tab.Content is UIElement content)
            {
                var animation = new Utility.Animation.Appearance(content)
                {
                    Opacity = 0,
                    Translate = 32
                };
                animation.Start(false);

            }
        }

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            await SubmitXmlsTimer();
        }

        private async void selectFile_Click(object sender, RoutedEventArgs e)
        {
            var xmlText = await Utility.FilePicker.ReadText([".xml", ".txt", "*"]);
            if (xmlText != null)
                if (items.SelectedItem is TabViewItem tabViewItem && tabViewItem.Content is TileXmlEditor editor)
                {
                    editor.Editor.Text = xmlText;
                    editor.SelectedSingleEditor = false;
                }
                else
                {
                    editor = NewTab();
                    editor.Editor.Text = xmlText;
                    editor.SelectedSingleEditor = false;
                }
        }

        private void randomize_Click(object sender, RoutedEventArgs e)
        {
            if (items.TabItems.Count <= 1)
                return;
            var random = new Random();
            var tabList = items.TabItems.ToList();
            items.TabItems.Clear();

            int n = tabList.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (tabList[n], tabList[k]) = (tabList[k], tabList[n]);
            }
            foreach (var item in tabList)
            {
                items.TabItems.Add(item);
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            items.TabItems.Clear();
        }
    }
}
