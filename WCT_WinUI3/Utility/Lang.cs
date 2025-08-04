using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.Resources;
using Microsoft.Windows.Globalization;
using Fischldesu.WCTCore;

namespace WCT_WinUI3.Utility.I18N
{
    public class Lang
    {
        private static readonly ResourceLoader loader = new();
        public static string Primary
        {
            set
            {
                string? PLO = value;
                if (string.IsNullOrEmpty(value))
                    PLO = Windows.Globalization.ApplicationLanguages.ManifestLanguages[0];
                ApplicationLanguages.PrimaryLanguageOverride = PLO;
                Settings.SetStringValue("language", value);
            }
            get
            {
                var lang = Settings.GetStringValue("language");
                if (string.IsNullOrEmpty(lang))
                    return string.Empty;
                return lang;
            }
        }

        public static string? Text(string stringTag)
        {
            try
            {
                return loader.GetString(stringTag);
            }
            catch
            {
                Log.Error($"Cannot load string: {stringTag}");
                return stringTag;
            }
        }

    }
}
