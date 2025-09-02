using System;

namespace Padi.RedmineApi.Serialization.Xml;

/// <summary>
/// Configuration for XML serialization
/// </summary>
public sealed class XmlSerializerConfiguration : ISerializerConfiguration<XmlSerializerSettings>
{
    private readonly XmlSerializerSettings _settings = new();

    /// <inheritdoc/>
    public IRedmineSerializer CreateSerializer()
    {
        return new XmlRedmineSerializer();
    }

    /// <inheritdoc/>
    public ISerializerConfiguration<XmlSerializerSettings> Configure(Action<XmlSerializerSettings> configure)
    {
        configure.Invoke(_settings);
        return this;
    }
}
