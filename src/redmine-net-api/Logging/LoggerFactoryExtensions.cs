
#if  NET462_OR_GREATER || NETCOREAPP

using Microsoft.Extensions.Logging;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public static class LoggerFactoryExtensions
{
    /// <summary>
    /// Creates a Redmine logger from the Microsoft ILoggerFactory
    /// </summary>
    public static IRedmineLogger CreateRedmineLogger(this ILoggerFactory factory, string categoryName = "Redmine.Api")
    {
        if (factory == null)
        {
            return RedmineNullLogger.Instance;
        }
        
        var logger = factory.CreateLogger(categoryName);
        return RedmineLoggerFactory.CreateMicrosoftLogger(logger);
    }
}
#endif
