#if NET45_OR_GREATER || NETCOREAPP
using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Diagnostics.Metrics;
#endif
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Net.HttpClient;

/// <summary>
/// 
/// </summary>
public sealed class RedmineHttpClientOptions: IRedmineHttpClientOptions
{
    /// <summary>
    /// 
    /// </summary>
    public bool? AutoRedirect { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public CookieContainer CookieContainer { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DecompressionMethods? DecompressionFormat { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ICredentials Credentials { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string> DefaultHeaders { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public IWebProxy Proxy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? KeepAlive { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? MaxAutomaticRedirections { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long? MaxRequestContentBufferSize { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long? MaxResponseContentBufferSize { get; set; }
#if NET471_OR_GREATER || NETCOREAPP
    /// <summary>
    /// 
    /// </summary>
    public int? MaxConnectionsPerServer { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? MaxResponseHeadersLength { get; set; }
#endif
    /// <summary>
    /// 
    /// </summary>
    public bool? PreAuthenticate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public RequestCachePolicy RequestCachePolicy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Scheme { get; set; }
#if NET45_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
#endif
    /// <summary>
    /// 
    /// </summary>
    public TimeSpan? Timeout { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? UnsafeAuthenticatedConnectionSharing { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserAgent { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? UseCookies { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? UseDefaultCredentials { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? UseProxy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Version ProtocolVersion { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the certificate is checked against the certificate authority revocation list.
    /// </summary>
    public bool CheckCertificateRevocationList { get; set; } = true;
    /// <summary>
    /// 
    /// </summary>
    public int? DefaultConnectionLimit { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? DnsRefreshTimeout { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool? EnableDnsRoundRobin { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? MaxServicePoints { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? MaxServicePointIdleTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public SecurityProtocolType? SecurityProtocolType { get; set; }
    
    #if NET8_0_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    public IMeterFactory MeterFactory { get; set; }
    #endif
    /// <summary>
    /// 
    /// </summary>
    public bool SupportsAutomaticDecompression { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool SupportsProxy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool SupportsRedirectConfiguration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Version DefaultRequestVersion { get; set; }
    
#if NETCOREAPP
    /// <summary>
    /// 
    /// </summary>
    public HttpVersionPolicy DefaultVersionPolicy { get; set; }
#endif
    /// <summary>
    /// 
    /// </summary>
    public ICredentials DefaultProxyCredentials { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ClientCertificateOption ClientCertificateOptions { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public X509CertificateCollection ClientCertificates { get; set; }
   

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
}
#endif