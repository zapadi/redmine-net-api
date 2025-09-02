using System;
using Padi.RedmineApi.Builders;

namespace Padi.RedmineApi.Serialization.JsonText;

public static class RedmineManagerOptionBuilderExtensions
{
    /// <summary>
    /// Configures the Redmine client to use JSON Text serialization
    /// </summary>
    /// <param name="builder">The builder instance</param>
    /// <param name="configure"></param>
    /// <returns>The builder instance for method chaining</returns>
    public static RedmineManagerOptionsBuilder UseJsonTextSerializer(this RedmineManagerOptionsBuilder builder, Action<JsonTextSerializerSettings>? configure = null)
    {
        var config = new JsonTextSerializerConfiguration();
        if (configure != null)
        {
            config.Configure(configure);
        }
        builder.WithSerializer(config);
        return builder;
    }
}