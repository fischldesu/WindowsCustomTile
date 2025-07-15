using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace WCT_WinUI3.Utility.Log
{
    public class Logger
    {
        private static readonly Logger instance = new();
        private readonly Queue<LogMessage> logMessages = [];

        public static LogMessage[] History { get { return [.. instance.logMessages]; } }
        
        public delegate void LoggedNewMessage(LogMessage msg);
        public static event LoggedNewMessage? NewMessageLogged;

        private Logger() { }

        static Logger()
        {
            Append(new LogMessage("Logger instance initialized."));
        }

        private static void Append(LogMessage message)
        {
            if (instance.logMessages.Count >= 512)
                instance.logMessages.Dequeue();

            instance.logMessages.Enqueue(message);
            NewMessageLogged?.Invoke(message);
        }

        public static LogMessage Log(string message, LogMessage.LogLevel level = LogMessage.LogLevel.INFO)
        {
            var msg = new LogMessage(message, level);

            Append(msg);
            return msg;
        }

    }
}
