#if NET45_OR_GREATER || NETCOREAPP

using System;
using Redmine.Net.Api.Http;

namespace Redmine.Net.Api.Net.HttpClient;

internal static class HttpClientProvider
{
    public static System.Net.Http.HttpClient Client(RedmineManagerOptions redmineManagerOptions)
    {
        var handler = NonDisposableHttpClientHandler.Instance;

        var options = redmineManagerOptions.ClientOptions as IRedmineHttpClientOptions;
        
        if (options != null)
        {
            handler.CookieContainer = options.CookieContainer;
            handler.Credentials = options.Credentials;
            handler.Proxy = options.Proxy;
                
            if (options.AutoRedirect.HasValue)
            {
                handler.AllowAutoRedirect = options.AutoRedirect.Value;
            }
                
            if (options.DecompressionFormat.HasValue)
            {
                handler.AutomaticDecompression = options.DecompressionFormat.Value;
            }
                
            if (options.PreAuthenticate.HasValue)
            {
                handler.PreAuthenticate = options.PreAuthenticate.Value;
            }
                
            if (options.UseCookies.HasValue)
            {
                handler.UseCookies = options.UseCookies.Value;
            }

            if (options.UseProxy.HasValue)
            {
                handler.UseProxy = options.UseProxy.Value;
            }
               
            if (options.MaxAutomaticRedirections.HasValue)
            {
                handler.MaxAutomaticRedirections = options.MaxAutomaticRedirections.Value;
            }
                
            if (options.UseDefaultCredentials.HasValue)
            {
                handler.UseDefaultCredentials = options.UseDefaultCredentials.Value;
            }

            if (options.MaxRequestContentBufferSize.HasValue)
            {
                handler.MaxRequestContentBufferSize = options.MaxRequestContentBufferSize.Value;
            }
                
#if NET471_OR_GREATER || NETCOREAPP
            handler.CheckCertificateRevocationList = options.CheckCertificateRevocationList;
            handler.DefaultProxyCredentials = options.DefaultProxyCredentials;
            handler.ServerCertificateCustomValidationCallback = options.ServerCertificateCustomValidationCallback;
            handler.SslProtocols = options.SslProtocols;

            if (options.MaxConnectionsPerServer.HasValue)
            {
                handler.MaxConnectionsPerServer = options.MaxConnectionsPerServer.Value;
            }
            if (options.MaxConnectionsPerServer.HasValue)
            {
                handler.MaxConnectionsPerServer = options.MaxConnectionsPerServer.Value;
            }
            if (options.MaxResponseHeadersLength.HasValue)
            {
                handler.MaxResponseHeadersLength = options.MaxResponseHeadersLength.Value;
            }
#endif
                
#if NET8_0_OR_GREATER
                handler.MeterFactory = options.MeterFactory;
#endif
        }

        var client = new System.Net.Http.HttpClient(handler);
        if (options != null)
        {
            if (options.Timeout.HasValue)
            {
                client.Timeout = options.Timeout.Value;
            }

            if (options.MaxResponseContentBufferSize.HasValue)
            {
                client.MaxResponseContentBufferSize = options.MaxResponseContentBufferSize.Value;
            }
               
#if NET5_0_OR_GREATER
                client.DefaultRequestVersion = options.DefaultRequestVersion;
                client.DefaultVersionPolicy = options.DefaultVersionPolicy;
#endif
        }

        client.BaseAddress = redmineManagerOptions.BaseAddress;

        return client;
    }
}
#endif