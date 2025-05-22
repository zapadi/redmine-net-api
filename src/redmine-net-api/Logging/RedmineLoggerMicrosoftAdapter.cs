#if  NET462_OR_GREATER || NETCOREAPP
using System;
using System.Collections.Generic;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public class RedmineLoggerMicrosoftAdapter : Microsoft.Extensions.Logging.ILogger
{
    private readonly IRedmineLogger _redmineLogger;
    private readonly string _categoryName;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="redmineLogger"></param>
    /// <param name="categoryName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RedmineLoggerMicrosoftAdapter(IRedmineLogger redmineLogger, string categoryName = "Redmine.Net.Api")
    {
        _redmineLogger = redmineLogger ?? throw new ArgumentNullException(nameof(redmineLogger));
        _categoryName = categoryName;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public IDisposable BeginScope<TState>(TState state)
    {
        if (state is IDictionary<string, object> dict)
        {
            _redmineLogger.CreateScope("Scope", dict);
        }
        else
        {
            var scopeName = state?.ToString() ?? "Scope";
            _redmineLogger.CreateScope(scopeName);
        }
        
        return new NoOpDisposable();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        return _redmineLogger.IsEnabled(ToRedmineLogLevel(logLevel));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    /// <typeparam name="TState"></typeparam>
    public void Log<TState>(
        Microsoft.Extensions.Logging.LogLevel logLevel, 
        Microsoft.Extensions.Logging.EventId eventId, 
        TState state, 
        Exception exception, 
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
            
        var message = formatter(state, exception);
        _redmineLogger.Log(ToRedmineLogLevel(logLevel), message, exception);
    }
    
    private static LogLevel ToRedmineLogLevel(Microsoft.Extensions.Logging.LogLevel level) => level switch
    {
        Microsoft.Extensions.Logging.LogLevel.Trace => LogLevel.Trace,
        Microsoft.Extensions.Logging.LogLevel.Debug => LogLevel.Debug,
        Microsoft.Extensions.Logging.LogLevel.Information => LogLevel.Information,
        Microsoft.Extensions.Logging.LogLevel.Warning => LogLevel.Warning,
        Microsoft.Extensions.Logging.LogLevel.Error => LogLevel.Error,
        Microsoft.Extensions.Logging.LogLevel.Critical => LogLevel.Critical,
        _ => LogLevel.Information
    };
    
    private class NoOpDisposable : IDisposable
    {
        public void Dispose() { }
    }
}
#endif