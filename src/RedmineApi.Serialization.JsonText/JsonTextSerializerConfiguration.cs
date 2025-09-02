using System;

namespace Padi.RedmineApi.Serialization.JsonText;

/// <summary>
/// Configuration for JSON Text serialization
/// </summary>
public sealed class JsonTextSerializerConfiguration : ISerializerConfiguration<JsonTextSerializerSettings>
{
    private readonly JsonTextSerializerSettings _settings = new();

    /// <inheritdoc/>
    public IRedmineSerializer CreateSerializer()
    {
        return new JsonTextRedmineSerializer(_settings.WriterOptions);
    }

    /// <inheritdoc/>
    public ISerializerConfiguration<JsonTextSerializerSettings> Configure(Action<JsonTextSerializerSettings>? configure = null)
    {
        configure?.Invoke(_settings);
        return this;
    }
}