using System;
using System.Globalization;

namespace Redmine.Net.Api.Logging.Loggers;

internal sealed class DebugLogger : IRedmineLogger
{
    public void Debug(string message, params object[] args)
    {
        Log("DEBUG", message, null, args);
    }

    public void Info(string message, params object[] args)
    {
        Log("INFO", message, null, args);
    }

    public void Warning(string message, params object[] args)
    {
        Log("WARN", message, null, args);
    }

    public void Error(string message, Exception exception = null, params object[] args)
    {
        Log("ERROR", message, exception, args);
    }

    private static void Log(string level, string message, Exception exception, params object[] args)
    {
        var formattedMessage = args.Length > 0 ? string.Format(CultureInfo.InvariantCulture, message, args) : message;
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        var logMessage = $"[{timestamp}] [{level}] {formattedMessage}";
        
        if (exception != null)
        {
            logMessage = $"{logMessage} Exception: {exception.Message} Stack Trace: {exception.StackTrace}";
        }

        System.Diagnostics.Debug.WriteLine(logMessage);
    }
}