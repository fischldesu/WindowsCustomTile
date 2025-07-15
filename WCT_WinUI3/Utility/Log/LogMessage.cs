using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCT_WinUI3.Utility.Log
{
    public class LogMessage(string message, LogMessage.LogLevel logLevel = LogMessage.LogLevel.INFO)
    {
        public enum LogLevel
        {
            INFO,
            WARNING,
            ERROR,
            FATAL
        }

        public readonly LogLevel Level = logLevel;
        public readonly DateTime Time = DateTime.Now;
        public readonly string Message = message;

        public override string ToString()
        {
            return $"[{Level}] {Time:HH:mm:ss} {Message}";
        }
    }
}
