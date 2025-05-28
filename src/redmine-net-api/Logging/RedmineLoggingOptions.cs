namespace Redmine.Net.Api.Logging;

/// <summary>
/// Options for configuring Redmine logging
/// </summary>
public sealed class RedmineLoggingOptions
{
    /// <summary>
    /// Gets or sets the minimum log level. The default value is LogLevel.Information
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Gets or sets whether to include HTTP request/response details in logs
    /// </summary>
    public bool IncludeHttpDetails { get; set; }

    /// <summary>
    /// Gets or sets whether performance metrics should be logged
    /// </summary>
    public bool LogPerformanceMetrics { get; set; }
}