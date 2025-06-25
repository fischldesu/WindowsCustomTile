using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTileWinUI3.Utility.Convert
{
    public class StringToImageSource : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                try
                {
                    return new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(str));
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Microsoft.UI.Xaml.Media.Imaging.BitmapImage bitmapImage)
                return bitmapImage.UriSource?.ToString();
            return null;
        }
    }
}
