using Fischldesu.WCTCore.Tile;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCT_WinUI3.Components;
using WCT_WinUI3.Components.Scheduled;
using WCT_WinUI3.Utility;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;

namespace WCT_WinUI3.Pages
{
    public sealed partial class Scheduled : Page
    {
        public readonly string BackgroundTaskName = "WCTBackgroundTileUpdateTask";

        public readonly struct TileXmlTabs(XmlDocument[] xmlDocuments, string[] invalid)
        {
            public readonly XmlDocument[] Available = xmlDocuments;
            public readonly string[] Invalid = invalid;
        }

        public Scheduled()
        {
            this.InitializeComponent();

            CancelTaskButton.Visibility = Visibility.Collapsed;
            foreach (var item in BackgroundTaskRegistration.AllTasks)
            {
                if (item.Value.Name == BackgroundTaskName)
                {
                    CancelTaskButton.Visibility = Visibility.Visible;
                    break;
                }
            }

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

            if (tabs == null || tabs.Value.Available.Length < 1)
            {
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Warning"),
                    Utility.I18N.Lang.Text("Info_NoValidXml"),
                    InfoBarSeverity.Warning);
                return false;
            }

            var content = new SubmitDialogContent(tabs.Value);

            if (!await AppContentDialog.ShowAsync(Utility.I18N.Lang.Text("Dialog_AskApplyChanges"), content))
                return false;

            SubmitDialogContent.ScheduleSubmitInfo? submitInfo = null;
            try
            {
                submitInfo = content.GetSubmitInfo();
            }
            catch (Exception e)
            {
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"),
                    $"{e.Message} \r\n{e.StackTrace}",
                    InfoBarSeverity.Error);
            }

            if (submitInfo == null) return false;

            var type = submitInfo.Value.TriggerType;
            var infoMessage = Utility.I18N.Lang.Text("Page_Scheduled");
            switch (type)
            {
                case SubmitDialogContent.ScheduleTriggerType.System:
                case SubmitDialogContent.ScheduleTriggerType.BackgroundTimer:
                    if (submitInfo.Value.Trigger is IBackgroundTrigger trigger)
                    {
                        ProcessingRing.Visibility = Visibility.Visible;
                        ProcessingRing.IsActive = true;
                        var xmls = tabs.Value.Available.AsReadOnly();
                        await Fischldesu.WCTCore.Tile.TileHelper.SetAutoUpdateTileXmls(xmls);
                        var registration = await Fischldesu.WCTCore.Tasks.Background.BackgroundTileUpdater.Register(BackgroundTaskName, trigger);
                        ProcessingRing.IsActive = false;
                        ProcessingRing.Visibility = Visibility.Collapsed;
                        if (registration == null)
                        {
                            App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"),
                                "Background task register failed",
                                InfoBarSeverity.Error);
                            return false;
                        }
                        else
                        {
                            var textID = "Page_Scheduled_TaskTriggerType_System";
                            if (type == SubmitDialogContent.ScheduleTriggerType.BackgroundTimer)
                                textID = "Page_Scheduled_TaskTriggerType_Time";

                            infoMessage += Utility.I18N.Lang.Text(textID);
                            CancelTaskButton.Visibility = Visibility.Visible;
                            XmlFileFolderOperationGroup.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"),
                            "Invlaid Trigger",
                            InfoBarSeverity.Error);
                        return false;
                    }
                    break;
                case SubmitDialogContent.ScheduleTriggerType.TrayIcon:
                    App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"),
                        "Tray Icon task is not available for now",
                        InfoBarSeverity.Error);
                    return false;
                default:
                    App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Failed"),
                        "Schedule Submit: Invalid Result",
                        InfoBarSeverity.Error);
                    return false;
            }

            App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Success"), infoMessage, InfoBarSeverity.Success);

            return true;
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

        private void CancelTaskButton_Click(object sender, RoutedEventArgs e)
        {
            Fischldesu.WCTCore.Tasks.Background.BackgroundTileUpdater.Unregister(BackgroundTaskName);
            CancelTaskButton.Visibility = Visibility.Collapsed;
        }

        private async void LoadXmlsFromFile_Click(object sender, RoutedEventArgs e)
        {
            var docs = await TileHelper.GetAllAutoUpdateTileXmls();
            if (docs == null || docs.Count < 1)
            {
                App.mainWindow?.ShowInfoBand(Utility.I18N.Lang.Text("G_Warning"),
                    Utility.I18N.Lang.Text("Info_NoValidXml"),
                    InfoBarSeverity.Warning);
                return;
            }
            
            foreach (var xmlDocument in docs)
            {
                var tab = NewTab();
                if (xmlDocument != null)
                    tab.SetXml(xmlDocument);
            }
        }

        private async void OpenXmlsFolder_Click(object sender, RoutedEventArgs e)
        {
            var folder = await TileHelper.GetSetAutoUpdateTileXmlsFolder();
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var path = await TileHelper.GetSetAutoUpdateTileXmlsFolder();
            if (path != null)
                XmlFileFolderOperationGroup.Visibility = Visibility.Visible;
        }
    }
}
