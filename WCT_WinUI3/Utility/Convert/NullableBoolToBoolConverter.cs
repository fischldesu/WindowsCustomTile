using Microsoft.UI.Xaml.Data;
using System;

namespace WCT_WinUI3.Utility.Convert;

public partial class NullableBoolToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value is bool b && b;

    public object? ConvertBack(object value, Type targetType, object parameter, string language) =>
        value is bool b ? (bool?)b : null;
}
