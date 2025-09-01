using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Serialization;

namespace Padi.RedmineApi;

/// <summary>
/// Defines a configuration mechanism for Redmine clients
/// </summary>
public interface IRedmineClientConfiguration
{
    /// <summary>
    /// Creates a client with the specified authentication and serializer
    /// </summary>
    /// <param name="authentication">The authentication configuration</param>
    /// <param name="serializer">The serializer to use</param>
    /// <returns>A configured Redmine API client</returns>
    IRedmineApiClient CreateClient(IRedmineAuthentication authentication, IRedmineSerializer serializer);
}
