using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCT_WinUI3.Utility
{
    public static class Toolsets
    {
        public static string AddPrefixForEachLine(string input, string prefix)
        {
            // 支持 \r\n、\n、\r
            var lines = input.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = prefix + lines[i];
            }
            return string.Join("\n", lines);
        }
    }
}
