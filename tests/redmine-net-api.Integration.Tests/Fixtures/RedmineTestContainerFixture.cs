using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Npgsql;
using Redmine.Net.Api;
using Testcontainers.PostgreSql;

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

    public const string RedmineApiKey = "029a9d38-17e8-41ae-bc8c-fbf71e193c57";
    
    private readonly string RedmineNetworkAlias = Guid.NewGuid().ToString();
    private INetwork Network { get; set; }
    private PostgreSqlContainer PostgresContainer { get; set; }
    private IContainer RedmineContainer { get; set; }
    public RedmineManager RedmineManager { get; private set; }
    public string RedmineHost { get; private set; }

    public RedmineTestContainerFixture()
    {
        BuildContainers();
    }

    private void BuildContainers()
    {
        Network = new NetworkBuilder()
            .WithDriver(NetworkDriver.Bridge)
            .Build();
        
        PostgresContainer = new PostgreSqlBuilder()
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
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort))
            .Build();
        
        RedmineContainer = new ContainerBuilder()
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
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPort(RedminePort).ForPath("/")))
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await Network.CreateAsync();
      
        await PostgresContainer.StartAsync();
       
        await RedmineContainer.StartAsync();

        await SeedTestDataAsync(PostgresContainer, CancellationToken.None);

        RedmineHost = $"http://{RedmineContainer.Hostname}:{RedmineContainer.GetMappedPublicPort(RedminePort)}";
        
        var rmgBuilder = new RedmineManagerOptionsBuilder()
            .WithHost(RedmineHost)
            .WithBasicAuthentication("adminuser", "1qaz2wsx");
        
        RedmineManager = new RedmineManager(rmgBuilder);
    }

    public async Task DisposeAsync()
    {
        var exceptions = new List<Exception>();
        
        await SafeDisposeAsync(() => RedmineContainer.StopAsync());
        await SafeDisposeAsync(() => PostgresContainer.StopAsync());
        await SafeDisposeAsync(() => Network.DisposeAsync().AsTask());
        
        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
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

    private static async Task SeedTestDataAsync(PostgreSqlContainer container, CancellationToken ct)
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
            // Optionally log stderr
        }
    }
} 