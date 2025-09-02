using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.WebClient;

/// <summary>
/// 
/// </summary>
public sealed class WebClientApiOptions : RedmineApiClientOptions
{
    /// <summary>
    /// 
    /// </summary>
    public TransporterOptions? ClientOptions { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TransporterOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? AutoRedirect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CookieContainer? CookieContainer { get; set; }

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
        public ICredentials? Credentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string>? DefaultHeaders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IWebProxy? Proxy { get; set; }

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
        public RequestCachePolicy? RequestCachePolicy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? UseCookies { get; set; }

#if !NET
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
        public Version? ProtocolVersion { get; set; }

#if NET40_OR_GREATER || NET
        /// <summary>
        /// 
        /// </summary>
        public X509CertificateCollection? ClientCertificates { get; set; }
#endif

        /// <summary>
        /// 
        /// </summary>
        public bool? KeepAlive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? UnsafeAuthenticatedConnectionSharing { get; set; }

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

#if(NET46_OR_GREATER || NET)
        /// <summary>
        /// 
        /// </summary>
        public bool? ReusePort { get; set; }
#endif

        /// <summary>
        /// 
        /// </summary>
        public SecurityProtocolType? SecurityProtocolType { get; set; }

#if (NET45_OR_GREATER || NET)
        /// <summary>
        /// 
        /// </summary>
        public RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }
#endif
    }
}