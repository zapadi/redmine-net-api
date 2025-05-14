using Microsoft.Extensions.Configuration;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Tests
{
    internal static class TestHelper
    {
        private static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            var environment = Environment.GetEnvironmentVariable("Environment");

            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddJsonFile($"appsettings-local.json", optional: true)
                .AddUserSecrets("f8b9e946-b547-42f1-861c-f719dca00a84")
                .Build();
        }

        public static RedmineCredentials GetApplicationConfiguration(string outputPath = "")
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = Directory.GetCurrentDirectory();
            }

            var credentials = new RedmineCredentials();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig
                .GetSection("Credentials")
                .Bind(credentials);
                
            return credentials;
        }
    }
}