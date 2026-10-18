using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Redmine.Net.Api;
using Redmine.Net.Api.Net.WebClient;
using Xunit;

#pragma warning disable CS0618
namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class RedmineManagerInitializationTests
{
    private sealed class CustomWebClientOptions : IRedmineWebClientOptions
    {
        public bool? AutoRedirect { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public DecompressionMethods? DecompressionFormat { get; set; }
        public ICredentials Credentials { get; set; }
        public Dictionary<string, string> DefaultHeaders { get; set; }
        public IWebProxy Proxy { get; set; }
        public bool? KeepAlive { get; set; }
        public int? MaxAutomaticRedirections { get; set; }
        public long? MaxRequestContentBufferSize { get; set; }
        public long? MaxResponseContentBufferSize { get; set; }
        public int? MaxConnectionsPerServer { get; set; }
        public int? MaxResponseHeadersLength { get; set; }
        public bool? PreAuthenticate { get; set; }
        public RequestCachePolicy RequestCachePolicy { get; set; }
        public string Scheme { get; set; }
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
        public TimeSpan? Timeout { get; set; }
        public bool? UnsafeAuthenticatedConnectionSharing { get; set; }
        public string UserAgent { get; set; }
        public bool? UseCookies { get; set; }
        public bool? UseDefaultCredentials { get; set; }
        public bool? UseProxy { get; set; }
        public Version ProtocolVersion { get; set; }
        public bool CheckCertificateRevocationList { get; set; }
        public int? DefaultConnectionLimit { get; set; }
        public int? DnsRefreshTimeout { get; set; }
        public bool? EnableDnsRoundRobin { get; set; }
        public int? MaxServicePoints { get; set; }
        public int? MaxServicePointIdleTime { get; set; }
        public SecurityProtocolType? SecurityProtocolType { get; set; }
        public X509CertificateCollection ClientCertificates { get; set; }
        public bool? ReusePort { get; set; }
    }

    [Fact]
    public void Caller_Web_Client_Options_Are_Not_Mutated()
    {
        var options = new RedmineWebClientOptions();

        _ = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithWebClientOptions(options));

        Assert.Null(options.SecurityProtocolType);
        Assert.Null(options.ServerCertificateValidationCallback);
    }

    [Fact]
    public void Obsolete_Properties_Are_Read_From_Custom_Web_Client_Options()
    {
        var proxy = new WebProxy("http://proxy:8080");
        var options = new CustomWebClientOptions
        {
            Proxy = proxy,
            Timeout = TimeSpan.FromSeconds(42),
            SecurityProtocolType = SecurityProtocolType.Tls12,
        };

        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithWebClientOptions(options));

        Assert.Same(proxy, manager.Proxy);
        Assert.Equal(TimeSpan.FromSeconds(42), manager.Timeout);
        Assert.Equal(SecurityProtocolType.Tls12, manager.SecurityProtocolType);
    }

    [Fact]
    public void Verify_Server_Cert_Keeps_User_Callback()
    {
        RemoteCertificateValidationCallback callback = (_, _, _, _) => true;
        var options = new RedmineWebClientOptions { ServerCertificateValidationCallback = callback };

        _ = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithVerifyServerCert(true).WithWebClientOptions(options));

        Assert.Same(callback, options.ServerCertificateValidationCallback);
    }

    [Fact]
    public void Verify_Server_Cert_Does_Not_Mutate_Caller_Options()
    {
        var options = new RedmineWebClientOptions();

        _ = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithVerifyServerCert(true).WithWebClientOptions(options));

        Assert.Null(options.ServerCertificateValidationCallback);
    }
}
