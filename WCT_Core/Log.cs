using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace Fischldesu.WCTCore;

public enum LogLevel
{
    INFO,
    WARNING,
    ERROR,
    FATAL
}

internal class Log
{

    public static LogMessage[] History => [.. historyMessages];

    private static readonly Queue<LogMessage> historyMessages = [];

    public static event EventHandler<LogMessage>? NewMessage;

    public static void Direct(string? message)
    {
        var path = System.IO.Path.Combine(Storage.Folder.Path, $"log.txt");
        System.IO.File.AppendAllText(path, $"{message}\r\n");
    }

    public static LogMessage Append(string message, LogLevel logLevel)
    {
        var logMessage = new LogMessage(message, logLevel);

        if (historyMessages.Count > 512)
            historyMessages.Dequeue();
        historyMessages.Enqueue(logMessage);

        try
        {
            var file = Storage.Folder.CreateFileAsync(
                $"{logMessage.Time.Month}-{logMessage.Time.Day}.log",
                CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
            FileIO.AppendTextAsync(file, $"{logMessage}\r\n").GetAwaiter().GetResult();
        }
        catch
        {
            Direct($"Failed to write: {logMessage}");
        }

        NewMessage?.Invoke(logMessage, logMessage);
        return logMessage;
    }

    public static LogMessage Info(string message) => Append(message, LogLevel.INFO);

    public static LogMessage Warning(string message) => Append(message, LogLevel.WARNING);

    public static LogMessage Error(string message) => Append(message, LogLevel.ERROR);

    public static LogMessage Fatal(string message) => Append(message, LogLevel.FATAL);

}

internal class LogMessage(string message, LogLevel logLevel = LogLevel.INFO)
{
    public readonly LogLevel Level = logLevel;
    public readonly DateTime Time = DateTime.Now;
    public readonly string Message = message;

    public override string ToString()
    {
        return $"[{Level}] {Time:HH:mm:ss} {Message}";
    }
}
