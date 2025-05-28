using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
#if NET || NET471_OR_GREATER
using System.Net.Http;
#endif
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Http;

/// <summary>
/// 
/// </summary>
public abstract class RedmineApiClientOptions : IRedmineApiClientOptions
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
    public DecompressionMethods? DecompressionFormat { get; set; } = 
#if NET
    DecompressionMethods.All;
#else
        DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;
#endif

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
    public int? MaxAutomaticRedirections { get; set; }

   

    /// <summary>
    /// 
    /// </summary>
    public long? MaxResponseContentBufferSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxConnectionsPerServer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxResponseHeadersLength { get; set; }

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
    public string Scheme { get; set; } = "https";


    /// <summary>
    /// 
    /// </summary>
    public TimeSpan? Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 
    /// </summary>
    public string UserAgent { get; set; } = "RedmineDotNetAPIClient";

    /// <summary>
    /// 
    /// </summary>
    public bool? UseCookies { get; set; }

#if NETFRAMEWORK
  /// <summary>
    /// 
    /// </summary>
    public bool CheckCertificateRevocationList { get; set; }

 /// <summary>
    /// 
    /// </summary>
    public long? MaxRequestContentBufferSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? UseDefaultCredentials { get; set; }
#endif
    /// <summary>
    /// 
    /// </summary>
    public bool? UseProxy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Only HTTP/1.0 and HTTP/1.1 version requests are currently supported.</remarks>
    public Version ProtocolVersion { get; set; }

  


#if NET40_OR_GREATER || NETCOREAPP
    /// <summary>
    /// 
    /// </summary>
    public X509CertificateCollection ClientCertificates { get;  set; }
#endif

  
}