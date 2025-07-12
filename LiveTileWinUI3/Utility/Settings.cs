using LiveTileWinUI3.Utility.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTileWinUI3.Utility
{

    public class Settings : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public Windows.ApplicationModel.PackageVersion Version => Windows.ApplicationModel.Package.Current.Id.Version;

        public void ResetAll()
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
        public string? LaunchCommand
        {
            get => GetStringValue("launch-cmd");
            set
            {
                if (value != LaunchCommand)
                {
                    SetStringValue("launch-cmd", value);
                    OnPropertyChanged(nameof(LaunchCommand));
                }
            }
        }

        // type of command, a uri or a system-cmd...
        public string? LaunchCommandType
        {
            get => GetStringValue("launch-type");
            set
            {
                if (value != LaunchCommandType)
                {
                    SetStringValue("launch-type", value);
                    OnPropertyChanged(nameof(LaunchCommandType));
                }
            }
        }

        // even if command is executed, should the window still be shown
        public bool LaunchAlwaysShowWindow
        {
            get => GetIntValue("launch-show") > 0;
            set
            {
                if (value != LaunchAlwaysShowWindow)
                {
                    SetIntValue("launch-show", value ? 1 : 0);
                    OnPropertyChanged(nameof(LaunchAlwaysShowWindow));
                }
            }
        }

        // dont show any system notification
        public bool NoNotification
        {
            get => GetIntValue("show-no-notification") > 0;
            set
            {
                if (value != NoNotification)
                {
                    SetIntValue("show-no-notification", value ? 1 : 0);
                    OnPropertyChanged(nameof(NoNotification));
                }
            }
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
