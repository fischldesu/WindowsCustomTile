using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.Resources;
using Microsoft.Windows.Globalization;

namespace LiveTileWinUI3.Utility.I18N
{
    public class Lang
    {
        private static readonly ResourceLoader loader = new();
        public static string Primary
        {
            set
            {
                ApplicationLanguages.PrimaryLanguageOverride = value;
            }
            get
            {
                var lang = ApplicationLanguages.PrimaryLanguageOverride;
                if (string.IsNullOrEmpty(lang))
                    return string.Empty;
                return lang;
            }
        }

        public static string? Text(string langTag)
        {
            return loader.GetString(langTag);
        }

    }
}
