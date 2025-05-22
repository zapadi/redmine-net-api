using System;
using System.Collections.Generic;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="categoryName"></param>
/// <param name="minLevel"></param>
public class RedmineConsoleLogger(string categoryName = "Redmine", LogLevel minLevel = LogLevel.Information) : IRedmineLogger
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel level) => level >= minLevel;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public void Log(LogLevel level, string message, Exception exception = null)
    {
        if (!IsEnabled(level))
        {
            return;
        }
            
        // var originalColor = Console.ForegroundColor;
        //
        // Console.ForegroundColor = level switch
        // {
        //     LogLevel.Trace => ConsoleColor.Gray,
        //     LogLevel.Debug => ConsoleColor.Gray,
        //     LogLevel.Information => ConsoleColor.White,
        //     LogLevel.Warning => ConsoleColor.Yellow,
        //     LogLevel.Error => ConsoleColor.Red,
        //     LogLevel.Critical => ConsoleColor.Red,
        //     _ => ConsoleColor.White
        // };
        
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{categoryName}] {message}");
        
        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception}");
        }
        
        // Console.ForegroundColor = originalColor;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scopeName"></param>
    /// <param name="scopeProperties"></param>
    /// <returns></returns>
    public IRedmineLogger CreateScope(string scopeName, IDictionary<string, object> scopeProperties = null)
    {
        return new RedmineConsoleLogger($"{categoryName}.{scopeName}", minLevel);
    }
}