namespace Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;

public sealed class TestContainerOptions
{
    public RedmineOptions? Redmine { get; set; }
    public PostgresOptions? Postgres { get; set; }
    public TestContainerMode Mode { get; set; }
}