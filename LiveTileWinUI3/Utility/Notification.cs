﻿using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTileWinUI3.Utility
{
    public class Notification
    {
        public static void Text(string title, string message)
        {
            if (Settings.NoNotification)
                return;

            AppNotification notification = new AppNotificationBuilder()
                .AddText(title)
                .AddText(message)
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
    }
}
