using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedmineApiClientOptions
    {
        /// <summary>
    /// 
    /// </summary>
    bool? AutoRedirect { get; set; }

    /// <summary>
    /// 
    /// </summary>
    CookieContainer CookieContainer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    DecompressionMethods? DecompressionFormat { get; set; }

    /// <summary>
    /// 
    /// </summary>
    ICredentials Credentials { get; set; }

    /// <summary>
    /// 
    /// </summary>
    Dictionary<string, string> DefaultHeaders { get; set; }

    /// <summary>
    /// 
    /// </summary>
    IWebProxy Proxy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? KeepAlive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? MaxAutomaticRedirections { get; set; }

    /// <summary>
    /// 
    /// </summary>
    long? MaxRequestContentBufferSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    long? MaxResponseContentBufferSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? MaxConnectionsPerServer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? MaxResponseHeadersLength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? PreAuthenticate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    RequestCachePolicy RequestCachePolicy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string Scheme { get; set; }

    /// <summary>
    /// 
    /// </summary>
    RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

    /// <summary>
    /// 
    /// </summary>
    TimeSpan? Timeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? UnsafeAuthenticatedConnectionSharing { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string UserAgent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? UseCookies { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? UseDefaultCredentials { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? UseProxy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Only HTTP/1.0 and HTTP/1.1 version requests are currently supported.</remarks>
    Version ProtocolVersion { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool CheckCertificateRevocationList { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? DefaultConnectionLimit { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? DnsRefreshTimeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool? EnableDnsRoundRobin { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? MaxServicePoints { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int? MaxServicePointIdleTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    SecurityProtocolType? SecurityProtocolType { get; set; }
    }
}