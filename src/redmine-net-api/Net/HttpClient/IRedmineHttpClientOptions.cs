#if NET45_OR_GREATER || NETCOREAPP
using System;
#if NET8_0_OR_GREATER
using System.Diagnostics.Metrics;
#endif
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Net.HttpClient;

/// <summary>
/// 
/// </summary>
public interface IRedmineHttpClientOptions: IRedmineApiClientOptions
{
    /// <summary>
    /// 
    /// </summary>
    ClientCertificateOption ClientCertificateOptions { get; set; }
    
#if NET471_OR_GREATER || NETCOREAPP
    /// <summary>
    /// 
    /// </summary>
    ICredentials DefaultProxyCredentials { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    SslProtocols SslProtocols { get; set; }
#endif
    
#if NET8_0_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    public IMeterFactory MeterFactory { get; set; }
#endif
    
    /// <summary>
    /// 
    /// </summary>
    bool SupportsAutomaticDecompression { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool SupportsProxy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool SupportsRedirectConfiguration { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    Version DefaultRequestVersion { get; set; }
    
    #if NETCOREAPP
    /// <summary>
    /// 
    /// </summary>
    HttpVersionPolicy DefaultVersionPolicy { get; set; }
#endif
}
#endif