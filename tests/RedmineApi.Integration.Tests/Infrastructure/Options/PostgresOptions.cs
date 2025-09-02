namespace Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;

public sealed class PostgresOptions
{
    public int Port { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}