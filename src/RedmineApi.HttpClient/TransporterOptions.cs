
#if NET40_OR_GREATER
using System;
using System.Collections.Generic;
using System.Net;
#if NET471_OR_GREATER
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
#endif

namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// 
/// </summary>
public sealed class TransporterOptions
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
    public bool? UseDefaultCredentials { get; set; }
    
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
    public bool? UseProxy { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public IWebProxy? Proxy { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool? PreAuthenticate { get; set; }
    
#if NET471_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    public X509CertificateCollection? ClientCertificates { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? CheckCertificateRevocationList { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SslProtocols? SslProtocols { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool VerifyServerCertificate { get; set; } = true;
    #endif
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Only HTTP/1.0 and HTTP/1.1 version requests are currently supported.</remarks>
    public Version? ProtocolVersion { get; set; }
}
#endif