using System.Net;

namespace Redmine.Net.Api.Authentication;

/// <summary>
/// 
/// </summary>
public sealed class RedmineNoAuthentication: IRedmineAuthentication
{
    /// <inheritdoc />
    public string AuthenticationType { get; } = "NoAuth";

    /// <inheritdoc />
    public string Token { get; init; }

    /// <inheritdoc />
    public ICredentials Credentials { get; init; }
}