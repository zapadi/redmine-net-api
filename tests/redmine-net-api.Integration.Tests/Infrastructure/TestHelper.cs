using Microsoft.Extensions.Configuration;
using Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;

namespace Padi.RedmineAPI.Integration.Tests.Infrastructure
{
    internal static class ConfigurationHelper
    {
        private static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.local.json", optional: true)
                .Build();
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