#if NET462_OR_GREATER || NETCOREAPP
using System;
using System.Collections.Generic;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// Adapter that converts Microsoft.Extensions.Logging.ILogger to IRedmineLogger
/// </summary>
public class MicrosoftLoggerRedmineAdapter : IRedmineLogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _microsoftLogger;
    
    /// <summary>
    /// Creates a new adapter for Microsoft.Extensions.Logging.ILogger
    /// </summary>
    /// <param name="microsoftLogger">The Microsoft logger to adapt</param>
    /// <exception cref="ArgumentNullException">Thrown if microsoftLogger is null</exception>
    public MicrosoftLoggerRedmineAdapter(Microsoft.Extensions.Logging.ILogger microsoftLogger)
    {
        _microsoftLogger = microsoftLogger ?? throw new ArgumentNullException(nameof(microsoftLogger));
    }
    
    /// <summary>
    /// Checks if logging is enabled for the specified level
    /// </summary>
    public bool IsEnabled(LogLevel level)
    {
        return _microsoftLogger.IsEnabled(ToMicrosoftLogLevel(level));
    }
    
    /// <summary>
    /// Logs a message with the specified level
    /// </summary>
    public void Log(LogLevel level, string message, Exception exception = null)
    {
        _microsoftLogger.Log(
            ToMicrosoftLogLevel(level),
            0, // eventId
            message,
            exception,
            (s, e) => s);
    }
    
    /// <summary>
    /// Creates a scoped logger with additional context
    /// </summary>
    public IRedmineLogger CreateScope(string scopeName, IDictionary<string, object> scopeProperties = null)
    {
        var scopeData = new Dictionary<string, object>
        {
            ["ScopeName"] = scopeName
        };
    
        // Add additional properties if provided
        if (scopeProperties != null)
        {
            foreach (var prop in scopeProperties)
            {
                scopeData[prop.Key] = prop.Value;
            }
        }
    
        // Create a single scope with all properties
        var disposableScope = _microsoftLogger.BeginScope(scopeData);
    
        // Return a new adapter that will close the scope when disposed
        return new ScopedMicrosoftLoggerAdapter(_microsoftLogger, disposableScope);
    }
    
    private class ScopedMicrosoftLoggerAdapter(Microsoft.Extensions.Logging.ILogger logger, IDisposable scope)
        : MicrosoftLoggerRedmineAdapter(logger), IDisposable
    {
        public void Dispose()
        {
            scope?.Dispose();
        }
    }

    
    private static Microsoft.Extensions.Logging.LogLevel ToMicrosoftLogLevel(LogLevel level) => level switch
    {
        LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
        LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
        LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
        LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
        LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
        LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
        _ => Microsoft.Extensions.Logging.LogLevel.Information
    };
}
#endif
