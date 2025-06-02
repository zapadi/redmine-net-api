using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Npgsql;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure.Options;
using Redmine.Net.Api;
using Redmine.Net.Api.Options;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;

public class RedmineTestContainerFixture : IAsyncLifetime
{
    private readonly RedmineConfiguration _configuration;
    private readonly string _redmineNetworkAlias = Guid.NewGuid().ToString();
    
    private readonly ITestOutputHelper _output;
    private readonly TestContainerOptions _testContainerOptions;

    private INetwork Network { get; set; }
    private PostgreSqlContainer PostgresContainer { get; set; }
    private IContainer RedmineContainer { get; set; }
    public RedmineManager RedmineManager { get; private set; }
    public string RedmineHost { get; private set; }

    public RedmineTestContainerFixture()
    {
        //_configuration = configuration;
        _testContainerOptions = ConfigurationHelper.GetConfiguration();
        
        if (_testContainerOptions.Mode != TestContainerMode.UseExisting)
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
            .WithImage(_testContainerOptions.Postgres.Image)
            .WithNetwork(Network)
            .WithNetworkAliases(_redmineNetworkAlias)
            .WithEnvironment(new Dictionary<string, string>
            {
                { "POSTGRES_DB", _testContainerOptions.Postgres.Database },
                { "POSTGRES_USER", _testContainerOptions.Postgres.User },
                { "POSTGRES_PASSWORD", _testContainerOptions.Postgres.Password },
            })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(_testContainerOptions.Postgres.Port));
       
        if (_testContainerOptions.Mode == TestContainerMode.CreateNewWithRandomPorts)
        {
            postgresBuilder.WithPortBinding(_testContainerOptions.Postgres.Port, assignRandomHostPort: true);
        }
        else
        {
            postgresBuilder.WithPortBinding(_testContainerOptions.Postgres.Port, _testContainerOptions.Postgres.Port);
        }
        
        PostgresContainer = postgresBuilder.Build();
        
        var redmineBuilder = new ContainerBuilder()
            .WithImage(_testContainerOptions.Redmine.Image)
            .WithNetwork(Network)
            .WithEnvironment(new Dictionary<string, string>
            {
                { "REDMINE_DB_POSTGRES", _redmineNetworkAlias },
                { "REDMINE_DB_PORT", _testContainerOptions.Redmine.Port.ToString() },
                { "REDMINE_DB_DATABASE", _testContainerOptions.Postgres.Database },
                { "REDMINE_DB_USERNAME", _testContainerOptions.Postgres.User },
                { "REDMINE_DB_PASSWORD", _testContainerOptions.Postgres.Password },
            })
            .DependsOn(PostgresContainer)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request.ForPort((ushort)_testContainerOptions.Redmine.Port).ForPath("/")));
        
        if (_testContainerOptions.Mode == TestContainerMode.CreateNewWithRandomPorts)
        {
            redmineBuilder.WithPortBinding(_testContainerOptions.Redmine.Port, assignRandomHostPort: true);
        }
        else
        {
            redmineBuilder.WithPortBinding(_testContainerOptions.Redmine.Port, _testContainerOptions.Redmine.Port);
        }
        
        RedmineContainer = redmineBuilder.Build();
    }
    
    public async Task InitializeAsync()
    {
        var rmgBuilder = new RedmineManagerOptionsBuilder();
    
        switch (_testContainerOptions.Redmine.AuthenticationMode)
        {
            case AuthenticationMode.ApiKey:
                var apiKey = _testContainerOptions.Redmine.Authentication.ApiKey;
                    rmgBuilder.WithApiKeyAuthentication(apiKey);
                break;
            case AuthenticationMode.Basic:
                var username = _testContainerOptions.Redmine.Authentication.Basic.Username;
                var password = _testContainerOptions.Redmine.Authentication.Basic.Password;
                rmgBuilder.WithBasicAuthentication(username, password);
                break;
        }
        
        if (_testContainerOptions.Mode == TestContainerMode.UseExisting)
        {
            RedmineHost = _testContainerOptions.Redmine.Url;
        }
        else
        {
            await Network.CreateAsync();

            await PostgresContainer.StartAsync();

            await RedmineContainer.StartAsync();

            await SeedTestDataAsync(PostgresContainer, CancellationToken.None);

            RedmineHost = $"http://{RedmineContainer.Hostname}:{RedmineContainer.GetMappedPublicPort(_testContainerOptions.Redmine.Port)}";
        }

        rmgBuilder.WithHost(RedmineHost);

        if (_configuration != null)
        {
            switch (_configuration.Client)
            {
                case ClientType.Http:
                    rmgBuilder.UseHttpClient();
                    break;
                case ClientType.Web:
                    rmgBuilder.UseWebClient();
                    break;
            }

            switch (_configuration.Serialization)
            {
                case SerializationType.Xml:
                    rmgBuilder.WithXmlSerialization();
                    break;
                case SerializationType.Json:
                    rmgBuilder.WithJsonSerialization();
                    break;
            }
        }
        else
        {
            rmgBuilder
                .UseHttpClient()
      //          .UseWebClient()
                .WithXmlSerialization();
        }

        RedmineManager = new RedmineManager(rmgBuilder);
    }

    public async Task DisposeAsync()
    {
        var exceptions = new List<Exception>();

        if (_testContainerOptions.Mode == TestContainerMode.UseExisting)
        {
            return;
        }
        
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
        var sql = await System.IO.File.ReadAllTextAsync(_testContainerOptions.Redmine.SqlFilePath, ct);
        var res = await container.ExecScriptAsync(sql, ct);
        if (!string.IsNullOrWhiteSpace(res.Stderr))
        {
            _output.WriteLine(res.Stderr);
        }
    }
}