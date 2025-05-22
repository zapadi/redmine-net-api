using System;
using System.Collections.Generic;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// Provides abstraction for logging operations
/// </summary>
public interface IRedmineLogger
{
    /// <summary>
    /// Checks if the specified log level is enabled
    /// </summary>
    bool IsEnabled(LogLevel level);
    
    /// <summary>
    /// Logs a message with the specified level
    /// </summary>
    void Log(LogLevel level, string message, Exception exception = null);
    
    /// <summary>
    /// Creates a scoped logger with additional context
    /// </summary>
    IRedmineLogger CreateScope(string scopeName, IDictionary<string, object> scopeProperties = null);
}