
#if NET5_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;

namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// 
/// </summary>
public sealed partial class TransporterOptions
{
    /// <summary>
    /// 
    /// </summary>
    public bool? AllowAutoRedirect { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int? MaxAutomaticRedirections { get; set; }
    

    /// <summary>
    /// 
    /// </summary>
    public bool? UseCookies { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public CookieContainer? CookieContainer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DecompressionMethods? AutomaticDecompression { get; set; } = HttpDecompressionMethods.Default();

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string>? DefaultHeaders { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? UseProxy { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public IWebProxy? Proxy { get; set; }
    
    /// <summary>
    /// Gets or sets the connection pooling lifetime in minutes
    /// </summary>
    public int? ConnectionPoolingLifetimeMinutes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? EnableMultipleHttp2Connections { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxConnectionsPerServer { get; set; }
    
    /// <summary>
    /// Set when you need HTTP/1.1
    /// </summary>
    /// <remarks>Only HTTP/1.0 and HTTP/1.1 version requests are currently supported.</remarks>
    public Version? ProtocolVersion { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Version? DefaultRequestVersion { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  HttpVersionPolicy? DefaultVersionPolicy { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }
}
#endif