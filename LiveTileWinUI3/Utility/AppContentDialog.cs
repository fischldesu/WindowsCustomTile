using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace LiveTileWinUI3.Utility
{
    public class AppContentDialog
    {
        private readonly XamlRoot? xamlRoot = App.mainWindow?.Content.XamlRoot;
        public static async Task<bool> ShowAsync(string title, object content)
        {
            var helper = new AppContentDialog();
            if (helper.xamlRoot != null)
            {
                ContentDialog dialog = new()
                {
                    Title = title,
                    Content = content,
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = helper.xamlRoot
                };

                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                    return true;
            }
            else
            {
                throw new ApplicationException("MainWindow of Application not exist");
            }
            return false;
        }
    }
}
