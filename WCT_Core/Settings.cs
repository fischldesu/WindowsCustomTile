using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("WindowsCustomTile")]
namespace Fischldesu.WCTCore;

internal partial class Settings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public static readonly Settings Instance = new();

    public static Windows.ApplicationModel.PackageVersion Version => Windows.ApplicationModel.Package.Current.Id.Version;

    public static void ResetAll()
    {
        Instance.LaunchCommand = null;
        Instance.LaunchCommandType = null;
        Instance.LaunchAlwaysShowWindow = false;
        Instance.NoNotification = false;
        Instance.AutoUpdateNextTileIndex = 0;

        Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        Tasks.Background.BackgroundTileUpdater.UnregisterAll();
    }

    public string? LaunchCommand
    {
        get => GetStringValue(nameof(LaunchCommand));
        set
        {
            if (value != LaunchCommand)
            {
                SetStringValue(nameof(LaunchCommand), value);
                Instance.OnPropertyChanged(nameof(LaunchCommand));
            }
        }
    }
    public string? LaunchCommandType
    {
        get => GetStringValue(nameof(LaunchCommandType));
        set
        {
            if (value != LaunchCommandType)
            {
                SetStringValue(nameof(LaunchCommandType), value);
                Instance.OnPropertyChanged(nameof(LaunchCommandType));
            }
        }
    }
    public bool LaunchAlwaysShowWindow
    {
        get => GetIntValue(nameof(LaunchAlwaysShowWindow)) > 0;
        set
        {
            if (value != LaunchAlwaysShowWindow)
            {
                SetIntValue(nameof(LaunchAlwaysShowWindow), value ? 1 : 0);
                Instance.OnPropertyChanged(nameof(LaunchAlwaysShowWindow));
            }
        }
    }
    public bool NoNotification
    {
        get => GetIntValue(nameof(NoNotification)) > 0;
        set
        {
            if (value != NoNotification)
            {
                SetIntValue(nameof(NoNotification), value ? 1 : 0);
                Instance.OnPropertyChanged(nameof(NoNotification));
            }
        }
    }
    public int AutoUpdateNextTileIndex
    {
        get => GetIntValue(nameof(AutoUpdateNextTileIndex));
        set
        {
            if (value != AutoUpdateNextTileIndex)
            {
                SetIntValue(nameof(AutoUpdateNextTileIndex), value);
                Instance.OnPropertyChanged(nameof(AutoUpdateNextTileIndex));
            }
        }
    }

    public static string? GetStringValue(string key)
    {
        var settings = Storage.Settings;
        return settings.Values[key] as string;
    }
    public static void SetStringValue(string key, string? value)
    {
        var settings = Storage.Settings;
        settings.Values[key] = value;
    }
    public static int GetIntValue(string key)
    {
        var settings = Storage.Settings;
        return settings.Values[key] is int value ? value : -1;
    }
    public static void SetIntValue(string key, int value)
    {
        var settings = Storage.Settings;
        settings.Values[key] = value;
    }
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
