using Microsoft.UI.Xaml;
using System;

namespace WCT_WinUI3.Utility
{
    public class Timer
    {
        public static Action SetInterval(Action action, TimeSpan timeSpan)
        {
            DispatcherTimer timer = new()
            {
                Interval = timeSpan
            };
            timer.Tick += (sender, e) => action();
            timer.Start();
            return timer.Stop;
        }

        public static Action SetTimeout(Action action, TimeSpan timeSpan)
        {
            DispatcherTimer timer = new()
            {
                Interval = timeSpan
            };
            timer.Tick += (sender, e) =>
            {
                action();
                timer.Stop();
            };
            timer.Start();
            return timer.Stop;
        }
    }
}
