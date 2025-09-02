using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// Factory for creating HttpClient-based Redmine API clients
/// </summary>
/// <remarks>
/// Initializes a new instance of the HttpClientFactory
/// </remarks>
/// <param name="options">The options for configuring the HTTP client</param>
public class HttpClientFactory(HttpClientApiOptions? options = null) : IRedmineClientFactory
{

    /// <inheritdoc />
    public IRedmineApiClient CreateClient(IRedmineAuthentication authentication, string mimeType)
    {
        return new InternalHttpClient(authentication, options, mimeType);
    }
}
