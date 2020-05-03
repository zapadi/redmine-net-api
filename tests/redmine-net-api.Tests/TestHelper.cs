using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Padi.RedmineApi.Tests
{
    internal static class TestHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            var environment = Environment.GetEnvironmentVariable("Environment");

            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddUserSecrets("f8b9e946-b547-42f1-861c-f719dca00a84")
                .AddEnvironmentVariables()
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
                .GetSection("Credentials-Local")
                .Bind(credentials);
                
            return credentials;
        }
    }
}