#if  NET462_OR_GREATER || NETCOREAPP
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds a RedmineLogger provider to the DI container
    /// </summary>
    public static ILoggingBuilder AddRedmineLogger(this ILoggingBuilder builder, IRedmineLogger redmineLogger)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (redmineLogger == null) throw new ArgumentNullException(nameof(redmineLogger));
        
        builder.Services.AddSingleton<IRedmineLogger>(redmineLogger);
        return builder;
    }
    
    /// <summary>
    /// Configures Redmine logging options
    /// </summary>
    public static ILoggingBuilder ConfigureRedmineLogging(this ILoggingBuilder builder, Action<RedmineLoggingOptions> configure)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        
        var options = new RedmineLoggingOptions();
        configure(options);
        
        builder.Services.AddSingleton(options);
        
        return builder;
    }
}

#endif




