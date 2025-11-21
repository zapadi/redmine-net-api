using System.Globalization;
using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Npgsql;
using Padi.RedmineAPI.Integration.Tests.Extensions;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;
using Redmine.Net.Api;
using Testcontainers.PostgreSql;
using Xunit;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace Padi.RedmineAPI.Integration.Tests.Fixtures;

public class RedmineTestContainerFixture : IAsyncLifetime
{
    private readonly TestContainerOptions _testContainerOptions;
     private INetwork? Network { get; set; }
     private PostgreSqlContainer? PostgresContainer { get; set; }
     private IContainer? RedmineContainer { get; set; }
     
     private readonly PostgresOptions _postgresOptions;
     private readonly RedmineOptions _redmineOptions;
     
     public RedmineManager? RedmineManager { get; private set; }
     public string? RedmineHost { get; private set; }

     public RedmineTestContainerFixture()
     {
         _testContainerOptions = ConfigurationHelper.GetConfiguration();
         if (_testContainerOptions is null)
         {
             throw new ArgumentNullException(nameof(_testContainerOptions));
         }

         _postgresOptions = _testContainerOptions.Postgres ?? throw new ArgumentNullException(nameof(_testContainerOptions.Postgres));
         _redmineOptions = _testContainerOptions.Redmine ?? throw new ArgumentNullException(nameof(_testContainerOptions.Redmine));
     }
     
    public async ValueTask DisposeAsync()
    {
       if (_testContainerOptions.Mode == TestContainerMode.UseExisting)
       {
           return;
       }
       
       var exceptions = new List<Exception>();
         
       if (RedmineContainer is not null)
       {
           await SafeDisposeAsync(() => RedmineContainer.StopAsync());
       }
         
       if (PostgresContainer is not null)
       {
           await SafeDisposeAsync(() => PostgresContainer.StopAsync());
       }
         
       if (Network is not null)
       {
           await SafeDisposeAsync(() => Network.DisposeAsync().AsTask());
       }
         
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
    
    public async ValueTask InitializeAsync()
    {
        if (_testContainerOptions.Mode != TestContainerMode.UseExisting)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            
            Network = new NetworkBuilder()
                .WithDriver(NetworkDriver.Bridge)
                .WithName($"redmine-net-{Guid.NewGuid().ToNoDash()}")
                .Build();
            await Network.CreateAsync(cts.Token);

            var postgresNetworkAlias = $"db-postgres-{Guid.NewGuid().ToNoDash()}";
            const ushort redmineContainerInternalPort = 3000;
            const ushort postgresContainerInternalPort = 5432;

            PostgresContainer = new PostgreSqlBuilder()
                .WithImage(_postgresOptions.Image)
                .WithNetwork(Network)
                .WithNetworkAliases(postgresNetworkAlias)
                .WithEnvironment(new Dictionary<string, string>
                {
                    ["POSTGRES_DB"] = _postgresOptions.Database,
                    ["POSTGRES_USER"] = _postgresOptions.User,
                    ["POSTGRES_PASSWORD"] = _postgresOptions.Password,
                })
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilMessageIsLogged("database system is ready to accept connections"))
                .Build();

            await PostgresContainer.StartAsync(cts.Token);

            var redmineNetworkAlias = $"redmine-web-{Guid.NewGuid().ToNoDash()}";

            var redmineBuilder = new ContainerBuilder()
                .WithImage(_redmineOptions.Image)
                .WithNetwork(Network)
                .WithNetworkAliases(redmineNetworkAlias)
                .WithEnvironment(new Dictionary<string, string>
                {
                    ["REDMINE_DB_POSTGRES"] = postgresNetworkAlias,
                    ["REDMINE_DB_PORT"] = postgresContainerInternalPort.ToString(CultureInfo.InvariantCulture),
                    ["REDMINE_DB_DATABASE"] = _postgresOptions.Database,
                    ["REDMINE_DB_USERNAME"] = _postgresOptions.User,
                    ["REDMINE_DB_PASSWORD"] = _postgresOptions.Password,
                })
                .WithPortBinding(redmineContainerInternalPort, assignRandomHostPort: true)
                .DependsOn(PostgresContainer)
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(req => req
                        .ForPort(redmineContainerInternalPort)
                        .ForPath("/")
                        .ForStatusCode(HttpStatusCode.OK)));

            RedmineContainer = redmineBuilder.Build();
            await RedmineContainer.StartAsync(cts.Token);

            await LoadDefaultDataAsync();
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
            await ApplySqlSeedAsync();
            
            var redminePublicPort = RedmineContainer.GetMappedPublicPort(redmineContainerInternalPort);
            RedmineHost = $"http://localhost:{redminePublicPort}";
        }
        else
        {
            RedmineHost = _redmineOptions.Url ?? throw new ArgumentNullException(nameof(RedmineOptions.Url));
        }
        
        RedmineManager = CreateRedmineManager(_redmineOptions, RedmineHost);
        return;

        async Task LoadDefaultDataAsync()
        {
            var result = await RedmineContainer.ExecAsync([
                "bash", "-lc",
                """
                 cd /usr/src/redmine &&
                        RAILS_ENV=production REDMINE_LANG=en bundle exec rake redmine:load_default_data &&
                        RAILS_ENV=production bundle exec rails jobs:work &
                 """
            ]);

            if (result.ExitCode != 0)
            {
                throw new Exception("load_default_data failed:\n" + result.Stderr);
            }
        }
        
        async Task ApplySqlSeedAsync()
        {
           const string host = "localhost";
           var port = PostgresContainer.GetMappedPublicPort(_postgresOptions.Port);

           var connectionString = $"Host={host};Port={port};Database={_postgresOptions.Database};Username={_postgresOptions.User};Password={_postgresOptions.Password}";

            var sql = File.ReadAllText(_redmineOptions.SqlFilePath);

            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }
    
    private static RedmineManager CreateRedmineManager(RedmineOptions redmineOptions, string redmineHost)
     {
         Ensure.NotNull(redmineOptions, nameof(redmineOptions));

         Ensure.NotNull(redmineOptions.Authentication, nameof(redmineOptions.Authentication));
         
         var rmgBuilder = new RedmineManagerOptionsBuilder();
     
         switch (redmineOptions.AuthenticationMode)
         {
             case AuthenticationMode.ApiKey:
                 var apiKey = redmineOptions.Authentication.ApiKey;
                 rmgBuilder.WithApiKeyAuthentication(apiKey);
                 break;
             case AuthenticationMode.Basic:
                 if (redmineOptions.Authentication.Basic is null)
                 {
                     throw new ArgumentNullException(nameof(redmineOptions.Authentication.Basic));
                 }
                 var username = redmineOptions.Authentication.Basic.Username;
                 var password = redmineOptions.Authentication.Basic.Password;
                 rmgBuilder.WithBasicAuthentication(username, password);
                 break;
         }
         
         switch (redmineOptions.Client)
         {
             case ClientType.Http:
             case ClientType.Web:
                 rmgBuilder.WithHost(redmineHost);
                 break;
             default:
                 throw new ArgumentOutOfRangeException(nameof(redmineOptions.Client));
         }

         switch (redmineOptions.Serializer)
         {
             case SerializationType.Xml:
                 rmgBuilder.WithSerializationType(Redmine.Net.Api.Serialization.SerializationType.Xml);
                 break;
             case SerializationType.Json:
             case SerializationType.JsonText:
                 rmgBuilder.WithSerializationType(Redmine.Net.Api.Serialization.SerializationType.Json);
                 break;
             default:
                 throw new ArgumentOutOfRangeException(nameof(redmineOptions.Serializer));
         }

         return new RedmineManager(rmgBuilder);
     }
}