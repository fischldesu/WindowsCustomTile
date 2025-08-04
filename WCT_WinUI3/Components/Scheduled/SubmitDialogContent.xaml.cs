using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Components.Scheduled;

public sealed partial class SubmitDialogContent : UserControl
{
    public enum ScheduleTriggerType
    {
        System,
        BackgroundTimer,
        TrayIcon
    }

    public readonly struct ScheduleSubmitInfo(ScheduleTriggerType triggerType, object? trigger)
    {
        public readonly object? Trigger = trigger;
        public readonly ScheduleTriggerType TriggerType = triggerType;
    }

    public readonly Pages.Scheduled.TileXmlTabs Tabs;

    public SubmitDialogContent(Pages.Scheduled.TileXmlTabs tabs)
    {
        Tabs = tabs;
        InitializeComponent();

        TimeTriggerComboBox.Items.Add("Background");
        TimeTriggerComboBox.Items.Add("TrayIcon");
        SystemTriggerComboBox.ItemsSource = Enum.GetValues(typeof(SystemTriggerType));

        ValidXmlCount.Text = tabs.Available.Length.ToString();
        if (tabs.Invalid.Length > 0)
        {
            InvalidXmlCount.Text = tabs.Invalid.Length.ToString();
            foreach (var name in tabs.Invalid)
                InvalidXmlList.Children.Add(new TextBlock { Text = name });
            InvalidXmlStatistics.Visibility = Visibility.Visible;
        }
        else InvalidXmlStatistics.Visibility = Visibility.Collapsed;

    }

    public ScheduleSubmitInfo? GetSubmitInfo()
    {
        if (TimeTriggerRadio.IsChecked ?? false)
        {
            uint time = Math.Max((uint)TimeTriggerTimeSpan.Value, 15);
            switch (TimeTriggerComboBox.SelectedIndex)
            {
                case 0:
                    return new(ScheduleTriggerType.BackgroundTimer, new TimeTrigger(time, false));
                case 1:
                    return new(ScheduleTriggerType.TrayIcon, time);
            }
            throw new ArgumentException($"Invalid Argument of TimeTrigger");
        }
        else if (SystemTriggerRadio.IsChecked ?? false)
        {
            if (SystemTriggerComboBox.SelectedItem is SystemTriggerType type && type != SystemTriggerType.Invalid)
                return new(ScheduleTriggerType.System, new SystemTrigger(type, false));
            
            throw new ArgumentException("Invalid System Trigger Type");
        }
        else
        {
            return null;
        }
    }

    private void TimeTriggerTimeSpan_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (TimeTriggerComboBox.SelectedIndex == 0 && args.NewValue < 15)
            sender.Value = 15;
        else if (args.NewValue < 1)
            sender.Value = 1;
    }

    private void TimeTriggerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TimeTriggerComboBox.SelectedIndex == 0 && TimeTriggerTimeSpan.Value < 15)
            TimeTriggerTimeSpan.Value = 15;
        
    }
}
