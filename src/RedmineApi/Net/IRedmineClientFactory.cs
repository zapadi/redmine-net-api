using Padi.RedmineApi.Authentication;

namespace Padi.RedmineApi.Net;

/// <summary>
/// Defines a factory for creating Redmine API clients
/// </summary>
public interface IRedmineClientFactory
{
    /// <summary>
    /// Creates a new instance of IRedmineApiClient
    /// </summary>
    /// <param name="authentication">The authentication configuration</param>
    /// <param name="mimeType">The MIME type for the client</param>
    /// <returns>A new instance of IRedmineApiClient</returns>
    IRedmineApiClient CreateClient(IRedmineAuthentication authentication, string mimeType);
}
