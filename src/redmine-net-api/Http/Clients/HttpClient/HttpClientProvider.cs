#if !NET20
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Redmine.Net.Api.Common;
using Redmine.Net.Api.Options;

namespace Redmine.Net.Api.Http.Clients.HttpClient;

internal static class HttpClientProvider
{
    private static System.Net.Http.HttpClient _client;

    /// <summary>
    /// Gets an HttpClient instance. If an existing client is provided, it is returned; otherwise, a new one is created.
    /// </summary>
    public static System.Net.Http.HttpClient GetOrCreateHttpClient(System.Net.Http.HttpClient httpClient,
        RedmineManagerOptions options)
    {
        if (_client != null)
        {
            return _client;
        }

        _client = httpClient ?? CreateClient(options);
        
        return _client;
    }

    /// <summary>
    /// Creates a new HttpClient instance configured with the specified options.
    /// </summary>
    private static System.Net.Http.HttpClient CreateClient(RedmineManagerOptions redmineManagerOptions)
    {
        ArgumentVerifier.ThrowIfNull(redmineManagerOptions, nameof(redmineManagerOptions));

        var handler = 
            #if NET
            CreateSocketHandler(redmineManagerOptions);
            #elif NETFRAMEWORK
            CreateHandler(redmineManagerOptions);
            #endif
        
        var client = new System.Net.Http.HttpClient(handler, disposeHandler: true);
        
        if (redmineManagerOptions.BaseAddress != null)
        {
            client.BaseAddress = redmineManagerOptions.BaseAddress;
        }

        if (redmineManagerOptions.ApiClientOptions is not RedmineHttpClientOptions options)
        {
            return client;
        }
        
        if (options.Timeout.HasValue)
        {
            client.Timeout = options.Timeout.Value;
        }

        if (options.MaxResponseContentBufferSize.HasValue)
        {
            client.MaxResponseContentBufferSize = options.MaxResponseContentBufferSize.Value;
        }

#if NET5_0_OR_GREATER
        if (options.DefaultRequestVersion != null)
        {
            client.DefaultRequestVersion = options.DefaultRequestVersion;
        }

        if (options.DefaultVersionPolicy != null)
        {
            client.DefaultVersionPolicy = options.DefaultVersionPolicy.Value;
        }
#endif

        return client;
    }

#if NET
    private static SocketsHttpHandler CreateSocketHandler(RedmineManagerOptions redmineManagerOptions)
    {
        var handler = new SocketsHttpHandler()
        {
            // Limit the lifetime of connections to better respect any DNS changes
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),

            // Check cert revocation
            SslOptions = new SslClientAuthenticationOptions()
            {
                CertificateRevocationCheckMode = X509RevocationMode.Online,
            },
        };
        
        if (redmineManagerOptions.ApiClientOptions is not RedmineHttpClientOptions options)
        {
            return handler;
        }

        if (options.CookieContainer != null)
        {
            handler.CookieContainer = options.CookieContainer;
        }

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
     
        handler.DefaultProxyCredentials = options.DefaultProxyCredentials;

        if (options.MaxConnectionsPerServer.HasValue)
        {
          handler.MaxConnectionsPerServer = options.MaxConnectionsPerServer.Value;
        }

        if (options.MaxResponseHeadersLength.HasValue)
        {
          handler.MaxResponseHeadersLength = options.MaxResponseHeadersLength.Value;
        }

#if NET8_0_OR_GREATER
        handler.MeterFactory = options.MeterFactory;
#endif
        
        return handler;
    }
#elif NETFRAMEWORK
        private static HttpClientHandler CreateHandler(RedmineManagerOptions redmineManagerOptions)
        {
            var handler = new HttpClientHandler();
            return ConfigureHandler(handler, redmineManagerOptions);
        }
    
    private static HttpClientHandler ConfigureHandler(HttpClientHandler handler, RedmineManagerOptions redmineManagerOptions)
    {
        if (redmineManagerOptions.ApiClientOptions is not RedmineHttpClientOptions options)
        {
            return handler;
        }

        if (options.UseDefaultCredentials.HasValue)
        {
            handler.UseDefaultCredentials = options.UseDefaultCredentials.Value;
        }

        if (options.CookieContainer != null)
        {
            handler.CookieContainer = options.CookieContainer;
        }

        if (handler.SupportsAutomaticDecompression && options.DecompressionFormat.HasValue)
        {
            handler.AutomaticDecompression = options.DecompressionFormat.Value;
        }

        if (handler.SupportsRedirectConfiguration)
        {
            if (options.AutoRedirect.HasValue)
            {
                handler.AllowAutoRedirect = options.AutoRedirect.Value;
            }

            if (options.MaxAutomaticRedirections.HasValue)
            {
                handler.MaxAutomaticRedirections = options.MaxAutomaticRedirections.Value;
            }
        }

        if (options.ClientCertificateOptions != default)
        {
            handler.ClientCertificateOptions = options.ClientCertificateOptions;
        }

        handler.Credentials = options.Credentials;
        
        if (options.UseProxy != null)
        {
            handler.UseProxy = options.UseProxy.Value;
            if (handler.UseProxy && options.Proxy != null)
            {
                handler.Proxy = options.Proxy;
            }
        }

        if (options.PreAuthenticate.HasValue)
        {
            handler.PreAuthenticate = options.PreAuthenticate.Value;
        }

        if (options.UseCookies.HasValue)
        {
            handler.UseCookies = options.UseCookies.Value;
        }

        if (options.MaxRequestContentBufferSize.HasValue)
        {
            handler.MaxRequestContentBufferSize = options.MaxRequestContentBufferSize.Value;
        }

#if NET471_OR_GREATER
        handler.CheckCertificateRevocationList = options.CheckCertificateRevocationList;

        if (options.DefaultProxyCredentials != null)
            handler.DefaultProxyCredentials = options.DefaultProxyCredentials;

        if (options.ServerCertificateCustomValidationCallback != null)
            handler.ServerCertificateCustomValidationCallback = options.ServerCertificateCustomValidationCallback;

        if (options.ServerCertificateValidationCallback != null)
            handler.ServerCertificateCustomValidationCallback = options.ServerCertificateValidationCallback;

        handler.SslProtocols = options.SslProtocols;

        if (options.MaxConnectionsPerServer.HasValue)
            handler.MaxConnectionsPerServer = options.MaxConnectionsPerServer.Value;

        if (options.MaxResponseHeadersLength.HasValue)
            handler.MaxResponseHeadersLength = options.MaxResponseHeadersLength.Value;
#endif

        return handler;
    }
#endif
}

#endif