using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCT_WinUI3.Utility
{
    public class Notification
    {
        public static void Text(string title, string message)
        {
            if (Fischldesu.WCTCore.Settings.Instance.NoNotification)
                return;

            AppNotification notification = new AppNotificationBuilder()
                .AddText(title)
                .AddText(message)
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
    }
}
