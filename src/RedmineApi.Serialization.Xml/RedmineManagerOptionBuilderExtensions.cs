using System;
using Padi.RedmineApi.Builders;

namespace Padi.RedmineApi.Serialization.Xml;

public static class RedmineManagerOptionBuilderExtensions
{
    /// <summary>
    /// Configures the Redmine client to use XML serialization
    /// </summary>
    /// <param name="builder">The builder instance</param>
    /// <param name="configure"></param>
    /// <returns>The builder instance for method chaining</returns>
    public static RedmineManagerOptionsBuilder UseXmlSerializer(this RedmineManagerOptionsBuilder builder, Action<XmlSerializerSettings>? configure = null)
    {
        var config = new XmlSerializerConfiguration();
        if (configure != null)
        {
            config.Configure(configure);
        }
        builder.WithSerializer(config);
        return builder;
    }
}