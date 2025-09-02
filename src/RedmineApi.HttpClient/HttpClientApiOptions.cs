using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// 
/// </summary>
public sealed class HttpClientApiOptions : RedmineApiClientOptions
{
    /// <summary>
    /// 
    /// </summary>
    public TransporterOptions? ClientOptions { get; set; }
    
    
#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets or sets whether to enable HTTP/3 support
    /// </summary>
    public bool EnableHttp3 { get; set; } = true;
#endif
    
}