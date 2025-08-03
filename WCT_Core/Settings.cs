using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("WCT_WinUI3")]
namespace Fischldesu.WCTCore;

internal partial class Settings : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    public Windows.ApplicationModel.PackageVersion Version => Windows.ApplicationModel.Package.Current.Id.Version;

    public void ResetAll()
    {
        LaunchCommand = null;
        LaunchCommandType = null;
        LaunchAlwaysShowWindow = false;
        NoNotification = false;
        AutoUpdateNextTileIndex = 0;
    }

    public string? LaunchCommand
    {
        get => GetStringValue(nameof(LaunchCommand));
        set
        {
            if (value != LaunchCommand)
            {
                SetStringValue(nameof(LaunchCommand), value);
                OnPropertyChanged(nameof(LaunchCommand));
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
                OnPropertyChanged(nameof(LaunchCommandType));
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
                OnPropertyChanged(nameof(LaunchAlwaysShowWindow));
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
                OnPropertyChanged(nameof(NoNotification));
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
                OnPropertyChanged(nameof(AutoUpdateNextTileIndex));
            }
        }
    }

    public string? GetStringValue(string key)
    {
        var settings = Storage.Settings;
        return settings.Values[key] as string;
    }
    public void SetStringValue(string key, string? value)
    {
        var settings = Storage.Settings;
        settings.Values[key] = value;
    }
    public int GetIntValue(string key)
    {
        var settings = Storage.Settings;
        return settings.Values[key] is int value ? value : -1;
    }
    public void SetIntValue(string key, int value)
    {
        var settings = Storage.Settings;
        settings.Values[key] = value;
    }
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
