using LiveTileWinUI3.Utility.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTileWinUI3.Utility
{
    public class Settings
    {
        private static readonly Settings instance = new();

        public static Windows.ApplicationModel.PackageVersion Version { get => Windows.ApplicationModel.Package.Current.Id.Version; }

        public static void ResetAll()
        {
            LaunchCommand = null;
            LaunchCommandType = null;
            LaunchAlwaysShowWindow = false;
            NoNotification = false;

            TileHelper.Reset();

            Logger.Log("Settings.ResetAll");
            Notification.Text("Settings have been reset", "All Settings have been restore to default");
        }
        // Command before Window activated
        public static string? LaunchCommand
        {
            get => instance.GetStringValue("launch-cmd");
            set => instance.SetStringValue("launch-cmd", value);
        }

        // type of command, a uri or a system-cmd...
        public static string? LaunchCommandType
        {
            get => instance.GetStringValue("launch-type");
            set => instance.SetStringValue("launch-type", value);
        }

        // even if command is executed, should the window still be shown
        public static bool LaunchAlwaysShowWindow
        {
            get => instance.GetIntValue("launch-show") > 0;
            set => instance.SetIntValue("launch-show", value ? 1 : 0);
        }

        // dont show any system notification
        public static bool NoNotification
        {
            get => instance.GetIntValue("show-no-notification") > 0;
            set => instance.SetIntValue("launch-show", value ? 1 : 0);
        }

        public string? GetStringValue(string key)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return settings.Values[key] as string;
        }

        public void SetStringValue(string key, string? value)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[key] = value;
        }

        public int GetIntValue(string key)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return settings.Values[key] is int value ? value : -1;
        }

        public void SetIntValue(string key, int value)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[key] = value;
        }

    }
}
