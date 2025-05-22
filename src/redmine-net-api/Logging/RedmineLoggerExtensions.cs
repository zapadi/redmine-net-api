using System;
using System.Diagnostics;
#if !(NET20 || NET40)
using System.Threading.Tasks;
#endif
namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public static class RedmineLoggerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void Trace(this IRedmineLogger logger, string message, Exception exception = null)
        => logger.Log(LogLevel.Trace, message, exception);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void Debug(this IRedmineLogger logger, string message, Exception exception = null)
        => logger.Log(LogLevel.Debug, message, exception);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void Info(this IRedmineLogger logger, string message, Exception exception = null)
        => logger.Log(LogLevel.Information, message, exception);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void Warn(this IRedmineLogger logger, string message, Exception exception = null)
        => logger.Log(LogLevel.Warning, message, exception);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void Error(this IRedmineLogger logger, string message, Exception exception = null)
        => logger.Log(LogLevel.Error, message, exception);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void Critical(this IRedmineLogger logger, string message, Exception exception = null)
        => logger.Log(LogLevel.Critical, message, exception);
    
#if !(NET20 || NET40)
    /// <summary>
    /// Creates and logs timing information for an operation
    /// </summary>
    public static async Task<T> TimeOperationAsync<T>(this IRedmineLogger logger, string operationName, Func<Task<T>> operation)
    {
        if (!logger.IsEnabled(LogLevel.Debug))
            return await operation().ConfigureAwait(false);
            
        var sw = Stopwatch.StartNew();
        try
        {
            return await operation().ConfigureAwait(false);
        }
        finally
        {
            sw.Stop();
            logger.Debug($"Operation '{operationName}' completed in {sw.ElapsedMilliseconds}ms");
        }
    }
    #endif
    
    /// <summary>
    /// Creates and logs timing information for an operation
    /// </summary>
    public static T TimeOperationAsync<T>(this IRedmineLogger logger, string operationName, Func<T> operation)
    {
        if (!logger.IsEnabled(LogLevel.Debug))
            return operation();
            
        var sw = Stopwatch.StartNew();
        try
        {
            return operation();
        }
        finally
        {
            sw.Stop();
            logger.Debug($"Operation '{operationName}' completed in {sw.ElapsedMilliseconds}ms");
        }
    }
}