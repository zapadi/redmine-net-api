using System.Collections.Specialized;

namespace Redmine.Net.Api.Net;

/// <summary>
/// 
/// </summary>
public sealed class RequestOptions
{
    /// <summary>
    /// 
    /// </summary>
    public NameValueCollection QueryString { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ImpersonateUser { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ContentType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Accept { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserAgent { get; set; }
}