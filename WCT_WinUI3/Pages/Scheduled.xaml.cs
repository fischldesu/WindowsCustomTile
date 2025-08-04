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
using System.Collections.Generic;
using WinRT;

namespace WCT_WinUI3.Pages
{
    public sealed partial class Scheduled : Page
    {
        public class TileXmlTabs(XmlDocument[] xmlDocuments, string[] invalid)
        {
            public readonly XmlDocument[] Available = xmlDocuments;
            public readonly string[] Invalid = invalid;
        }

        public Scheduled()
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

        public TileXmlTabs? GetTileXmls()
        {
            if (items.TabItems.Count == 0) return null;

            List<XmlDocument> xmlDocuments = [];
            List<string> invalid = [];

            foreach (var tab in items.TabItems)
            {
                if (tab is TabViewItem tabItem && tabItem.Content is TileXmlEditor item)
                {
                    try
                    {
                        var xmlDocument = new XmlDocument();
                        var xmlString = item.GetXml();

                        xmlDocument.LoadXml(xmlString);
                        xmlDocuments.Add(xmlDocument);
                    }
                    catch
                    {
                        var itemHeader = tabItem.Header.ToString();
                        if (itemHeader != null)
                            invalid.Add(itemHeader);
                    }
                }
            }

            if (xmlDocuments.Count < 1)
                return null;

            return new([.. xmlDocuments], [.. invalid]);
        }

        public async Task<bool> SubmitXmlsTimer(TileXmlTabs? tabs)
        {
            //


            if (tabs == null || tabs.Available.Length < 1)
            {
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Warning"),
                    Utility.I18N.Lang.Text("Info_NoValidXml"),
                    InfoBarSeverity.Warning);
                return false;
            }

            TextBlock content = new()
            {
                Margin = new Thickness(0, 16, 0, 0)
            };

            void TextTitle (string? text)
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

            TextTitle($"{Utility.I18N.Lang.Text("Dialog_TotalValidXml")}\n");
            TextContent($"{Utility.I18N.Lang.Text("Dialog_Count")}: {tabs.Available.Length}\n");
            TextLineBreak();
            TextTitle($"{Utility.I18N.Lang.Text("Dialog_TimeSpan")}\n");
            TextContent($"{Utility.I18N.Lang.Text("Dialog_Every")} {timeInput.Value} {timeFormat.SelectedItem}\n");
            if (tabs.Invalid.Length > 0)
            {
                TextLineBreak();
                TextTitle($"{Utility.I18N.Lang.Text("Dialog_TotalInvalidXml")}\n");
                TextContent($"{Utility.I18N.Lang.Text("Dialog_Count")}: {tabs.Invalid.Length}\n{string.Join(Environment.NewLine, tabs.Invalid)}");
            }

            if (await AppContentDialog.ShowAsync(Utility.I18N.Lang.Text("Dialog_AskApplyChanges"), content))
            {
                var timeSpan = timeFormat.SelectedIndex switch
                {
                    0 => TimeSpan.FromHours(timeInput.Value),
                    1 => TimeSpan.FromMinutes(timeInput.Value),
                    2 => TimeSpan.FromSeconds(timeInput.Value),
                    _ => TimeSpan.FromMinutes(timeInput.Value),
                };
                await Fischldesu.WCTCore.Tile.TileHelper.SetAutoUpdateTileXmls(tabs.Available.AsReadOnly());
                await Fischldesu.WCTCore.Tasks.Background.BackgroundTileUpdater.Register("interval", new Windows.ApplicationModel.Background.TimeTrigger((uint)timeSpan.Minutes, false));
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Success"),
                    Utility.I18N.Lang.Text("Info_ChangesApplied"),
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
            var tabs = GetTileXmls();
            await SubmitXmlsTimer(tabs);
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
