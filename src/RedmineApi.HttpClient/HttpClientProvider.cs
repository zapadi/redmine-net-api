using System;
using System.Net.Http;
using System.Net.Security;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// 
/// </summary>
internal static class HttpClientProvider
{
    private static System.Net.Http.HttpClient? _httpClient;

    /// <summary>
    /// Gets an HttpClient instance. If an existing client is provided, it is returned; otherwise, a new one is created.
    /// </summary>
    public static System.Net.Http.HttpClient GetOrCreateHttpClient(
        HttpClientApiOptions options, System.Net.Http.HttpClient? httpClient = null)
    {
        if (_httpClient != null)
        {
            return _httpClient;
        }

        _httpClient = httpClient ?? CreateClient(options);

        return _httpClient;
    }

    /// <summary>
    /// Creates a new HttpClient instance configured with the specified options.
    /// </summary>
    private static System.Net.Http.HttpClient CreateClient(HttpClientApiOptions httpClientOptions)
    {
         var handler = CreateDefaultHandler(httpClientOptions);;
        
        if (string.IsNullOrWhiteSpace(httpClientOptions.BaseAddress))
        {
            throw new ArgumentException("BaseAddress not set or empty.", nameof(RedmineApiClientOptions.BaseAddress));
        }
        
        _httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = new Uri(httpClientOptions.BaseAddress),
        };

        if (httpClientOptions.Timeout.HasValue)
        {
            _httpClient.Timeout = httpClientOptions.Timeout.Value;
        }
        
        #if NET5_0_OR_GREATER
        if (httpClientOptions.ClientOptions?.DefaultRequestVersion is not null)
        {
            _httpClient.DefaultRequestVersion = httpClientOptions.ClientOptions.DefaultRequestVersion;
        }
        
        if (httpClientOptions.ClientOptions?.DefaultVersionPolicy is not null)
        {
            _httpClient.DefaultVersionPolicy = httpClientOptions.ClientOptions.DefaultVersionPolicy.Value;
        }
        #endif
        
        SetDefaultRequestHeaders(_httpClient, httpClientOptions);
        
        return _httpClient;
    }
    
    private static void SetDefaultRequestHeaders(System.Net.Http.HttpClient httpClient, HttpClientApiOptions httpClientOptions)
    {
        httpClient.DefaultRequestHeaders.Accept.Clear();

        if (httpClientOptions.ClientOptions?.DefaultHeaders is not null)
        {
            foreach (var header in httpClientOptions.ClientOptions.DefaultHeaders)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (!string.IsNullOrEmpty(httpClientOptions.DefaultUserAgent))
        {
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(httpClientOptions.DefaultUserAgent);
        }

        if (!string.IsNullOrEmpty(httpClientOptions.DefaultImpersonateUser))
        {
            httpClient.DefaultRequestHeaders.Add((string)RedmineConstants.IMPERSONATE_HEADER_KEY, (string)httpClientOptions.DefaultImpersonateUser);
        }
    }
    
    private static HttpMessageHandler CreateDefaultHandler(HttpClientApiOptions  options)
    {
#if NET
        return CreateDefaultSocketsHandler(options);
#else
        return CreateDefaultHttpClientHandler(options);   
#endif
    }
    
#if NET40_OR_GREATER    
    private static HttpMessageHandler CreateDefaultHttpClientHandler(HttpClientApiOptions options)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = options.ClientOptions?.AutomaticDecompression ?? HttpDecompressionMethods.Default()
        };

        if (options.ClientOptions is not null)
        {
            if (options.ClientOptions.UseCookies is true && options.ClientOptions.CookieContainer is not null)
            {
                handler.UseCookies = true;
                handler.CookieContainer = options.ClientOptions.CookieContainer;
            }
            
            if (options.ClientOptions.UseProxy is true && options.ClientOptions.Proxy is not null)
            {
                handler.UseProxy = true;
                handler.Proxy = options.ClientOptions.Proxy;
            }
            
            if (options.ClientOptions.UseDefaultCredentials is true && options.ClientOptions.Credentials is not null)
            {
                handler.UseDefaultCredentials = true;
                handler.Credentials = options.ClientOptions.Credentials;
            }

            if (options.ClientOptions.AllowAutoRedirect.HasValue )
            {
                handler.AllowAutoRedirect = options.ClientOptions.AllowAutoRedirect.Value;
                if (options.ClientOptions.MaxAutomaticRedirections > 0)
                {
                    handler.MaxAutomaticRedirections = options.ClientOptions.MaxAutomaticRedirections.Value;
                }
            }
            
            if(options.ClientOptions.PreAuthenticate.HasValue)
            {
                handler.PreAuthenticate = options.ClientOptions.PreAuthenticate.Value;
            }
    #if NET471_OR_GREATER
            if (options.ClientOptions.ClientCertificates != null)
            {
                foreach (var cert in options.ClientOptions.ClientCertificates)
                {
                    handler.ClientCertificates.Add(cert);
                }
            }
            
            if (options.ClientOptions.CheckCertificateRevocationList != null)
            {
                handler.CheckCertificateRevocationList = options.ClientOptions.CheckCertificateRevocationList.Value;
            }

            if (options.ClientOptions.SslProtocols != null)
            {
                handler.SslProtocols = options.ClientOptions.SslProtocols.Value;
            }
            
            if (options.ClientOptions.VerifyServerCertificate is not true)
            {
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            }
    #endif
        }

        return handler;
    }
#endif
    
#if NET
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static HttpMessageHandler CreateDefaultSocketsHandler(HttpClientApiOptions options)
    {
        var handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            
        };

        if (options.ClientOptions is not null)
        {
            if (options.ClientOptions.MaxConnectionsPerServer.HasValue)
            {
                handler.MaxConnectionsPerServer = Math.Max(1, options.ClientOptions.MaxConnectionsPerServer.Value);
            }

            handler.AutomaticDecompression = options.ClientOptions.AutomaticDecompression ?? HttpDecompressionMethods.Default();

            if (options.ClientOptions.ConnectionPoolingLifetimeMinutes > 0)
            {
                handler.PooledConnectionLifetime = TimeSpan.FromMinutes(options.ClientOptions.ConnectionPoolingLifetimeMinutes.Value);
            }
            
            if (options.ClientOptions.UseCookies is true && options.ClientOptions.CookieContainer is not null)
            {
                handler.UseCookies = true;
                handler.CookieContainer = options.ClientOptions.CookieContainer;
            }
            
            if (options.ClientOptions.UseProxy is true && options.ClientOptions.Proxy is not null)
            {
                handler.UseProxy = true;
                handler.Proxy = options.ClientOptions.Proxy;
            }

            if (options.ClientOptions.AllowAutoRedirect.HasValue )
            {
                handler.AllowAutoRedirect = options.ClientOptions.AllowAutoRedirect.Value;
                if (options.ClientOptions.MaxAutomaticRedirections > 0)
                {
                    handler.MaxAutomaticRedirections = options.ClientOptions.MaxAutomaticRedirections.Value;
                }
            }

            if (options.ClientOptions.EnableMultipleHttp2Connections != null)
            {
                handler.EnableMultipleHttp2Connections = options.ClientOptions.EnableMultipleHttp2Connections.Value;
            }
        }
        
        var sslOptions = handler.SslOptions;
        sslOptions.ApplicationProtocols =
        [
            SslApplicationProtocol.Http2,
            SslApplicationProtocol.Http11
        ];
        #if NET7_0_OR_GREATER
        if (options.EnableHttp3)
        {
            sslOptions.ApplicationProtocols.Insert(0, SslApplicationProtocol.Http3);
        }
        #endif
        
        handler.SslOptions = sslOptions;

        if (options.ClientOptions?.ServerCertificateValidationCallback is null)
        {
            handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        }
        else
        {
            handler.SslOptions.RemoteCertificateValidationCallback = options.ClientOptions.ServerCertificateValidationCallback;
        }

        return handler;
    }
#endif
}