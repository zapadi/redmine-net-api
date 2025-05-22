#if  NET462_OR_GREATER || NETCOREAPP
namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public static class RedmineLoggerFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="redmineLogger"></param>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public static Microsoft.Extensions.Logging.ILogger CreateMicrosoftLoggerAdapter(IRedmineLogger redmineLogger, 
        string categoryName = "Redmine")
    {
        if (redmineLogger == null || redmineLogger == RedmineNullLogger.Instance)
        {
            return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        }
        
        return new RedmineLoggerMicrosoftAdapter(redmineLogger, categoryName);
    }
    
    /// <summary>
    /// Creates an adapter that exposes a Microsoft.Extensions.Logging.ILogger as IRedmineLogger
    /// </summary>
    /// <param name="microsoftLogger">The Microsoft logger to adapt</param>
    /// <returns>A Redmine logger implementation</returns>
    public static IRedmineLogger CreateMicrosoftLogger(Microsoft.Extensions.Logging.ILogger microsoftLogger)
    {
        return microsoftLogger != null 
            ? new MicrosoftLoggerRedmineAdapter(microsoftLogger) 
            : RedmineNullLogger.Instance;
    }

    /// <summary>
    /// Creates a logger that writes to the console
    /// </summary>
    public static IRedmineLogger CreateConsoleLogger(LogLevel minLevel = LogLevel.Information)
    {
        return new RedmineConsoleLogger(minLevel: minLevel);
    }
    
    // /// <summary>
    // /// Creates an adapter for Serilog
    // /// </summary>
    // public static IRedmineLogger CreateSerilogAdapter(Serilog.ILogger logger)
    // {
    //     if (logger == null) return NullRedmineLogger.Instance;
    //     return new SerilogAdapter(logger);
    // }
    //
    // /// <summary>
    // /// Creates an adapter for NLog
    // /// </summary>
    // public static IRedmineLogger CreateNLogAdapter(NLog.ILogger logger)
    // {
    //     if (logger == null) return NullRedmineLogger.Instance;
    //     return new NLogAdapter(logger);
    // }
    //
    // /// <summary>
    // /// Creates an adapter for log4net
    // /// </summary>
    // public static IRedmineLogger CreateLog4NetAdapter(log4net.ILog logger)
    // {
    //     if (logger == null) return NullRedmineLogger.Instance;
    //     return new Log4NetAdapter(logger);
    // }
}


#endif