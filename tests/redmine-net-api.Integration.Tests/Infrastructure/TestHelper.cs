using Microsoft.Extensions.Configuration;
using Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;

namespace Padi.RedmineAPI.Integration.Tests.Infrastructure
{
    internal static class ConfigurationHelper
    {
        /// <summary>
        /// Detects if running in a CI/CD environment
        /// </summary>
        private static bool IsRunningInCiEnvironment()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

        }
        
        private static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true);

            if (!IsRunningInCiEnvironment())
            {
                config.AddJsonFile("appsettings.local.json", optional: true);
            }
            return config.Build();
        }

        public static TestContainerOptions GetConfiguration(string outputPath = "")
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = Directory.GetCurrentDirectory();
            }

            var testContainerOptions = new TestContainerOptions();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig.GetSection("TestContainer")
                .Bind(testContainerOptions);
                
            return testContainerOptions;
        }
    }
}