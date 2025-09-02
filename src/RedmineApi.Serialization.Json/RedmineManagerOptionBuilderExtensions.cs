using System;
using Newtonsoft.Json;
using Padi.RedmineApi.Builders;

namespace Padi.RedmineApi.Serialization.Json;

public static class RedmineManagerOptionBuilderExtensions
{
    /// <summary>
    /// Configures the Redmine client to use JSON serialization
    /// </summary>
    /// <param name="builder">The builder instance</param>
    /// <param name="configure"></param>
    /// <returns>The builder instance for method chaining</returns>
    public static RedmineManagerOptionsBuilder UseJsonSerializer(this RedmineManagerOptionsBuilder builder, Action<JsonSerializerSettings>? configure = null)
    {
        var config = new JsonSerializerConfiguration();
        if (configure != null)
        {
            config.Configure(configure);
        }
        builder.WithSerializer(config);
        return builder;
    }
}