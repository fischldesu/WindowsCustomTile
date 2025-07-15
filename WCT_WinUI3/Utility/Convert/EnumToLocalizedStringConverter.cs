using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCT_WinUI3.Utility.Convert
{
    public partial class EnumToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string? ret = null;
            if (value is Enum enumValue)
                ret = I18N.Lang.Text($"Enum_{enumValue.GetType().Name}_{enumValue}");
            if (string.IsNullOrEmpty(ret))
                return value?.ToString() ?? string.Empty;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
