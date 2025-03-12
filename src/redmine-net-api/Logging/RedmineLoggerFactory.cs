using System;
using System.Threading;
using Redmine.Net.Api.Logging.Loggers;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public static class RedmineLoggerFactory
{
    #if NET20
    private static readonly object _lock = new();
    #else
    private static readonly Lazy<IRedmineLogger> _lazyLogger = 
        new(() => new DebugLogger(), LazyThreadSafetyMode.ExecutionAndPublication);
    #endif
    static RedmineLoggerFactory()
    {
        #if NET20
        lock(_lock)
        {
            Logger =  new DebugLogger();
        }
        #else
            Logger = _lazyLogger.Value;
        #endif
    }

    /// <summary>
    /// 
    /// </summary>
    public static IRedmineLogger Logger { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetLogger(IRedmineLogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}