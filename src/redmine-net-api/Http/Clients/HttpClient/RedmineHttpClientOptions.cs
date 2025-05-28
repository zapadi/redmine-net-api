#if !NET20
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
#if NET8_0_OR_GREATER
using System.Diagnostics.Metrics;
#endif

namespace Redmine.Net.Api.Http.Clients.HttpClient;

/// <summary>
/// 
/// </summary>
public sealed class RedmineHttpClientOptions: RedmineApiClientOptions
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    public IMeterFactory MeterFactory { get; set; }
#endif
   
    /// <summary>
    /// 
    /// </summary>
    public Version DefaultRequestVersion { get; set; }
    
#if NET
    /// <summary>
    /// 
    /// </summary>
    public HttpVersionPolicy? DefaultVersionPolicy { get; set; }
#endif
    /// <summary>
    /// 
    /// </summary>
    public ICredentials DefaultProxyCredentials { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ClientCertificateOption ClientCertificateOptions { get; set; }
    
  
    
#if NETFRAMEWORK
 /// <summary>
    /// 
    /// </summary>
    public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
    {
        get;
        set;
    }

    /// <summary>
    /// 
    /// </summary>
    public SslProtocols SslProtocols { get; set; }
    #endif
    /// <summary>
    /// 
    /// </summary>
    public
#if NET || NET471_OR_GREATER
        Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> 
#else
     RemoteCertificateValidationCallback 
#endif
        ServerCertificateValidationCallback { get; set; }
}
#endif