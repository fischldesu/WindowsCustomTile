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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LiveTileWinUI3.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HTTPXmlServer : Page
    {
        public HTTPXmlServer()
        {
            this.InitializeComponent();
            updateRecurrence.ItemsSource = Enum.GetValues(typeof(Windows.UI.Notifications.PeriodicUpdateRecurrence));
            updateRecurrence.SelectedIndex = 0;
            NewItem("uri 1");
        }

        public Components.XmlUriInput NewItem(string header = "")
        {
            if(header == string.Empty)
                header = $"uri {inputStack.Children.Count + 1}";

            var item = new Components.XmlUriInput()
            {
                Header = header,
                Margin = new Thickness(0, 0, 0, 8)
            };

            item.RequestRemove += RequestRemoveOne;

            inputStack.Children.Add(item);

            var animation = new Utility.Animation.Appearance(item)
            {
                TransitionDuration = new Duration(TimeSpan.FromMilliseconds(300)),
                Translate = 35,
                Opacity = 0.2
            };
            animation.Start(true);
            return item;
        }

        public async Task Submit()
        {
            var uriList = new List<Uri>();
            var invalids = new List<string>();

            foreach (var item in inputStack.Children)
                if (item is Components.XmlUriInput uriInput)
                    if (uriInput.Uri != null) uriList.Add(uriInput.Uri);
                    else invalids.Add(uriInput.Header);

            if (inputStack.Children.Count == 0)
            {
                info.Title = "Warning";
                info.Message = "No available server URI.";
                info.Severity = InfoBarSeverity.Error;
                info.IsOpen = true;
                return;
            }

            var dialogContent = new TextBlock()
            {
                Margin = new Thickness(0, 8, 0, 8),
            };
            dialogContent.Inlines.Add(new Run()
            {
                Text = "Valid Uri",
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                FontSize = 12
            });
            dialogContent.Inlines.Add(new Run()
            {
                Text = $" Count: {uriList.Count}",
            });
            dialogContent.Inlines.Add(new LineBreak());
            dialogContent.Inlines.Add(new LineBreak());
            if (invalids.Count > 0)
            {
                dialogContent.Inlines.Add(new Run()
                {
                    Text = "Invalid Uri",
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    FontSize = 12
                });
                dialogContent.Inlines.Add(new LineBreak());
                foreach (var item in invalids)
                {
                    dialogContent.Inlines.Add(new Run() { Text = item });
                    dialogContent.Inlines.Add(new LineBreak());
                }
                dialogContent.Inlines.Add(new LineBreak());
            }
            dialogContent.Inlines.Add(new Run()
            {
                Text = "Update Recurrence",
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                FontSize = 12
            });
            dialogContent.Inlines.Add(new LineBreak());
            dialogContent.Inlines.Add(new Run()
            {
                Text = $"Time span: {updateRecurrence.SelectedItem}",
            });

            var ok = await Utility.AppContentDialog.ShowAsync(
                "Apply changes?",
                dialogContent);
            if (ok)
            {
                if (updateRecurrence.SelectedItem is Windows.UI.Notifications.PeriodicUpdateRecurrence recurrence)
                    if (uriList.Count == 1) Utility.TileHelper.SetHTTPServer(uriList[0], recurrence);
                    else Utility.TileHelper.SetHTTPServer([.. uriList], recurrence);

                if (uriList.Count != 0)
                {
                    info.Title = "Warning";
                    info.Message = "None of server URI is valid.";
                    info.Severity = InfoBarSeverity.Warning;
                }
                else
                {
                    info.Title = "Success";
                    info.Message = "All server URI changes has applied.";
                    info.Severity = InfoBarSeverity.Success;
                }

                info.IsOpen = true;
            }
        }

        private void newItem_Click(object sender, RoutedEventArgs e)
        {
            NewItem();
        }

        public void RequestRemoveOne(object? from, Components.XmlUriInput? input)
        {
            if (input != null)
            {
                //var animation = new Utility.Animation.Disappear(input)
                //{

                //};
                //animation.Start(true).Completed +=(obj, _)=>{
                    inputStack.Children.Remove(input);
                //};
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            inputStack.Children.Clear();
        }

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            await Submit();
        }

        private void randomize_Click(object sender, RoutedEventArgs e)
        {
            if (inputStack.Children.Count <= 1)
                return;

            var list = inputStack.Children.Cast<UIElement>().ToList();

            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }

            inputStack.Children.Clear();
            foreach (var item in list)
            {
                inputStack.Children.Add(item);
                var animation = new Utility.Animation.Appearance(item)
                {
                    TransitionDuration = new Duration(TimeSpan.FromMilliseconds(300)),
                    Translate = 35,
                    Opacity = 0.2
                };
                animation.Start(true);
            }
        }
    }
}
