using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Net.WebClient;

/// <summary>
/// 
/// </summary>
public interface IRedmineWebClientOptions : IRedmineApiClientOptions
{
#if NET40_OR_GREATER || NETCOREAPP
    /// <summary>
    /// 
    /// </summary>
    public X509CertificateCollection ClientCertificates { get;  set; }
#endif
        
    /// <summary>
    /// 
    /// </summary>
    int? DefaultConnectionLimit { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    Dictionary<string, string> DefaultHeaders { get; set; }
        
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
    bool? KeepAlive { get; set; }

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
    RequestCachePolicy RequestCachePolicy { get; set; }
        
#if(NET46_OR_GREATER || NETCOREAPP)
    /// <summary>
    /// 
    /// </summary>
    public bool? ReusePort { get; set; }
#endif
        
    /// <summary>
    /// 
    /// </summary>
    RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    bool? UnsafeAuthenticatedConnectionSharing { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Only HTTP/1.0 and HTTP/1.1 version requests are currently supported.</remarks>
    Version ProtocolVersion { get; set; }
}