using System;
using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Serialization;

namespace Padi.RedmineApi.WebClient;

/// <summary>
/// Factory for creating WebClient-based Redmine API clients
/// </summary>
public sealed class WebClientFactory : IRedmineClientFactory
{
    private readonly Action<WebClientApiOptions>? _configure;
    private readonly Func<System.Net.WebClient>? _webClientFactory;

    /// <summary>
    /// Creates a new WebClientFactory with options configuration
    /// </summary>
    /// <param name="configure">Action to configure WebClient options</param>
    public WebClientFactory(Action<WebClientApiOptions>? configure = null)
    {
        _configure = configure;
    }

    /// <summary>
    /// Creates a new WebClientFactory with a WebClient factory function
    /// </summary>
    /// <param name="webClientFactory">Factory function to create WebClient instances</param>
    public WebClientFactory(Func<System.Net.WebClient> webClientFactory)
    {
        _webClientFactory = webClientFactory ?? throw new ArgumentNullException(nameof(webClientFactory));
    }

    /// <inheritdoc />
    public IRedmineApiClient CreateClient(IRedmineAuthentication authentication, IRedmineSerializer serializer)
    {
        return CreateClient(authentication, (string)serializer.ContentType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authentication"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public IRedmineApiClient CreateClient(IRedmineAuthentication authentication, string contentType)
    {
        if (_webClientFactory != null)
        {
            return new InternalRedmineApiWebClient(_webClientFactory, authentication, contentType);
        }

        var options = new WebClientApiOptions();
        _configure?.Invoke(options);
        return new InternalRedmineApiWebClient(authentication, contentType, options);
    }
}
