using System.Net;

namespace Redmine.Net.Api.Authentication;

/// <summary>
/// 
/// </summary>
public sealed class RedmineApiKeyAuthentication: IRedmineAuthentication
{
    /// <inheritdoc />
    public string AuthenticationType { get; } = "X-Redmine-API-Key";

    /// <inheritdoc />
    public string Token { get; init; }

    /// <inheritdoc />
    public ICredentials Credentials { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    public RedmineApiKeyAuthentication(string apiKey)
    {
        Token = apiKey;
    }
}