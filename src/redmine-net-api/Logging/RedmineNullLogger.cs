using System;
using System.Collections.Generic;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public class RedmineNullLogger : IRedmineLogger
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly RedmineNullLogger Instance = new RedmineNullLogger();
    
    private RedmineNullLogger() { }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel level) => false;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public void Log(LogLevel level, string message, Exception exception = null) { }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scopeName"></param>
    /// <param name="scopeProperties"></param>
    /// <returns></returns>
    public IRedmineLogger CreateScope(string scopeName, IDictionary<string, object> scopeProperties = null) => this;
}