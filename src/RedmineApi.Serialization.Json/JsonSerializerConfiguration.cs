using System;
using Newtonsoft.Json;

namespace Padi.RedmineApi.Serialization.Json;

/// <summary>
/// Configuration for JSON serialization
/// </summary>
public class JsonSerializerConfiguration : ISerializerConfiguration<JsonSerializerSettings>
{
    private readonly JsonSerializerSettings _settings = new();

    /// <inheritdoc/>
    public IRedmineSerializer CreateSerializer()
    {
        return new JsonRedmineSerializer
        {
            
        };
    }

    /// <inheritdoc/>
    public ISerializerConfiguration<JsonSerializerSettings> Configure(Action<JsonSerializerSettings> configure)
    {
        configure?.Invoke(_settings);
        return this;
    }
}
