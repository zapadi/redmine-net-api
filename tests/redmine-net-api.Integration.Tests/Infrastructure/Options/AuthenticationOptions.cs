namespace Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;

public sealed class AuthenticationOptions
{
    public string? ApiKey { get; set; }

    public BasicAuthenticationOptions? Basic { get; set; }
}