#nullable enable
using System;

namespace Padi.RedmineApi.Net;

/// <summary>
/// 
/// </summary>
public abstract class RedmineApiClientOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Redmine instance.
    /// </summary>
    public string? BaseAddress { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Scheme { get; set; } = "https";

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan? Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Gets or sets the default impersonated Redmine login.
    /// If set, requests will include X-Redmine-Switch-User unless overridden per request.
    /// </summary>
    public string? DefaultImpersonateUser { get; set; }
    
    /// <summary>
    /// Gets or sets the default User-Agent header value unless overridden per request.
    /// </summary>
    public string? DefaultUserAgent { get; set; }
}
