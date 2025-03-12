using System;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public interface IRedmineLogger
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    void Debug(string message, params object[] args);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    void Info(string message, params object[] args);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    void Warning(string message, params object[] args);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    /// <param name="args"></param>
    void Error(string message, Exception? exception = null, params object[] args);
}