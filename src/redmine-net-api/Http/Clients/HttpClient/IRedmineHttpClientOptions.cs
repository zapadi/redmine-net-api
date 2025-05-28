#if NET40_OR_GREATER || NET
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
public interface IRedmineHttpClientOptions : IRedmineApiClientOptions
{
    /// <summary>
    /// 
    /// </summary>
    ClientCertificateOption ClientCertificateOptions { get; set; }
    
#if NET471_OR_GREATER || NET
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
    
#if NET
    /// <summary>
    /// 
    /// </summary>
    HttpVersionPolicy? DefaultVersionPolicy { get; set; }
#endif
}

#endif