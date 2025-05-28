using System.Text;
using Redmine.Net.Api.Options;

namespace Redmine.Net.Api.Http.Clients.WebClient;

internal static class WebClientProvider
{
    /// <summary>
    /// Creates a new WebClient instance with the specified options.
    /// </summary>
    /// <param name="options">The options for the Redmine manager.</param>
    /// <returns>A new WebClient instance.</returns>
    public static System.Net.WebClient CreateWebClient(RedmineManagerOptions options)
    {
        var webClient = new InternalWebClient(options);
        
        if (options?.ApiClientOptions is RedmineWebClientOptions webClientOptions)
        {
            ConfigureWebClient(webClient, webClientOptions);
        }
        
        return webClient;
    }
    
    /// <summary>
    /// Configures a WebClient instance with the specified options.
    /// </summary>
    /// <param name="webClient">The WebClient instance to configure.</param>
    /// <param name="options">The options to apply.</param>
    private static void ConfigureWebClient(System.Net.WebClient webClient, RedmineWebClientOptions options)
    {
        if (options == null) return;
        
        webClient.Proxy = options.Proxy;
        webClient.Headers = null;
        webClient.BaseAddress = null;
        webClient.CachePolicy = null;
        webClient.Credentials = null;
        webClient.Encoding = Encoding.UTF8;
        webClient.UseDefaultCredentials = false;
        
        // if (options.Timeout.HasValue && options.Timeout.Value != TimeSpan.Zero)
        // {
        //     webClient.Timeout = options.Timeout;
        // }
        //
        // if (options.KeepAlive.HasValue)
        // {
        //     webClient.KeepAlive = options.KeepAlive.Value;
        // }
        //
        // if (options.UnsafeAuthenticatedConnectionSharing.HasValue)
        // {
        //     webClient.UnsafeAuthenticatedConnectionSharing = options.UnsafeAuthenticatedConnectionSharing.Value;
        // }
        //
        // #if NET40_OR_GREATER || NET
        // if (options.ClientCertificates != null)
        // {
        //     webClient.ClientCertificates = options.ClientCertificates;
        // }
        // #endif
        
    }
}