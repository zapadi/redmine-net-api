using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Npgsql;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Options;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;

public class RedmineTestContainerFixture : IAsyncLifetime
{
    private const int RedminePort = 3000;
    private const int PostgresPort = 5432;
    private const string PostgresImage = "postgres:17.4-alpine";
    private const string RedmineImage = "redmine:6.0.5-alpine";
    private const string PostgresDb = "postgres";
    private const string PostgresUser = "postgres";
    private const string PostgresPassword = "postgres";
    private const string RedmineSqlFilePath = "TestData/init-redmine.sql";

    private readonly string RedmineNetworkAlias = Guid.NewGuid().ToString();
    
    private readonly ITestOutputHelper _output;
    private readonly TestContainerOptions _redmineOptions;

    private INetwork Network { get; set; }
    private PostgreSqlContainer PostgresContainer { get; set; }
    private IContainer RedmineContainer { get; set; }
    public RedmineManager RedmineManager { get; private set; }
    public string RedmineHost { get; private set; }

    public RedmineTestContainerFixture()
    {
        _redmineOptions = ConfigurationHelper.GetConfiguration();
        
        if (_redmineOptions.Mode != TestContainerMode.UseExisting)
        {
            BuildContainers();
        }
    }

    /// <summary>
    /// Detects if running in a CI/CD environment
    /// </summary>
    private static bool IsRunningInCiEnvironment()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
               !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

    }
    
    private void BuildContainers()
    {
        Network = new NetworkBuilder()
            .WithDriver(NetworkDriver.Bridge)
            .Build();

        var postgresBuilder = new PostgreSqlBuilder()
            .WithImage(PostgresImage)
            .WithNetwork(Network)
            .WithNetworkAliases(RedmineNetworkAlias)
            .WithPortBinding(PostgresPort, assignRandomHostPort: true)
            .WithEnvironment(new Dictionary<string, string>
            {
                { "POSTGRES_DB", PostgresDb },
                { "POSTGRES_USER", PostgresUser },
                { "POSTGRES_PASSWORD", PostgresPassword },
            })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort));
       
        if (_redmineOptions.Mode == TestContainerMode.CreateNewWithRandomPorts)
        {
            postgresBuilder.WithPortBinding(PostgresPort, assignRandomHostPort: true);
        }
        else
        {
            postgresBuilder.WithPortBinding(PostgresPort, PostgresPort);
        }
        
        PostgresContainer = postgresBuilder.Build();
        
        var redmineBuilder = new ContainerBuilder()
            .WithImage(RedmineImage)
            .WithNetwork(Network)
            .WithPortBinding(RedminePort, assignRandomHostPort: true)
            .WithEnvironment(new Dictionary<string, string>
            {
                { "REDMINE_DB_POSTGRES", RedmineNetworkAlias },
                { "REDMINE_DB_PORT", PostgresPort.ToString() },
                { "REDMINE_DB_DATABASE", PostgresDb },
                { "REDMINE_DB_USERNAME", PostgresUser },
                { "REDMINE_DB_PASSWORD", PostgresPassword },
            })
            .DependsOn(PostgresContainer)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request.ForPort(RedminePort).ForPath("/")));
        
        if (_redmineOptions.Mode == TestContainerMode.CreateNewWithRandomPorts)
        {
            redmineBuilder.WithPortBinding(RedminePort, assignRandomHostPort: true);
        }
        else
        {
            redmineBuilder.WithPortBinding(RedminePort, RedminePort);
        }
        
        RedmineContainer = redmineBuilder.Build();
    }
    
    public async Task InitializeAsync()
    {
        var rmgBuilder = new RedmineManagerOptionsBuilder();
    
        switch (_redmineOptions.AuthenticationMode)
        {
            case AuthenticationMode.ApiKey:
                var apiKey = _redmineOptions.Authentication.ApiKey;
                    rmgBuilder.WithApiKeyAuthentication(apiKey);
                break;
            case AuthenticationMode.Basic:
                var username = _redmineOptions.Authentication.Basic.Username;
                var password = _redmineOptions.Authentication.Basic.Password;
                rmgBuilder.WithBasicAuthentication(username, password);
                break;
        }
        
        if (_redmineOptions.Mode == TestContainerMode.UseExisting)
        {
            RedmineHost = _redmineOptions.Url;
        }
        else
        {
            await Network.CreateAsync();

            await PostgresContainer.StartAsync();

            await RedmineContainer.StartAsync();

            await SeedTestDataAsync(PostgresContainer, CancellationToken.None);

            RedmineHost = $"http://{RedmineContainer.Hostname}:{RedmineContainer.GetMappedPublicPort(RedminePort)}";
        }

        rmgBuilder
            .WithHost(RedmineHost)
            .UseHttpClient()
            //.UseWebClient()
            .WithXmlSerialization();
        
        RedmineManager = new RedmineManager(rmgBuilder);
    }

    public async Task DisposeAsync()
    {
        var exceptions = new List<Exception>();
        
        if (_redmineOptions.Mode != TestContainerMode.UseExisting)
        {
            await SafeDisposeAsync(() => RedmineContainer.StopAsync());
            await SafeDisposeAsync(() => PostgresContainer.StopAsync());
            await SafeDisposeAsync(() => Network.DisposeAsync().AsTask());

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        return;

        async Task SafeDisposeAsync(Func<Task> disposeFunc)
        {
            try
            {
                await disposeFunc();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
    }

    private async Task SeedTestDataAsync(PostgreSqlContainer container, CancellationToken ct)
    {
        const int maxDbAttempts = 10;
        var dbRetryDelay = TimeSpan.FromSeconds(2);
        var connectionString = container.GetConnectionString();
        for (var attempt = 1; attempt <= maxDbAttempts; attempt++)
        {
            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync(ct);
                break;
            }
            catch
            {
                if (attempt == maxDbAttempts)
                {
                    throw;
                }
                await Task.Delay(dbRetryDelay, ct);
            }
        }
        var sql = await System.IO.File.ReadAllTextAsync(RedmineSqlFilePath, ct);
        var res = await container.ExecScriptAsync(sql, ct);
        if (!string.IsNullOrWhiteSpace(res.Stderr))
        {
            _output.WriteLine(res.Stderr);
        }
    }
} 

/// <summary>
/// Enum defining how containers should be managed
/// </summary>
public enum TestContainerMode
{
    /// <summary>Use existing running containers at specified URL</summary>
    UseExisting,
        
    /// <summary>Create new containers with random ports (CI-friendly)</summary>
    CreateNewWithRandomPorts
}
