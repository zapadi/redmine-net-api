using System;

namespace Padi.RedmineApi.Serialization;

/// <summary>
/// Defines configuration options for serializers
/// </summary>
/// <typeparam name="TSettings">The type of settings supported by the serializer</typeparam>
public interface ISerializerConfiguration<TSettings> : IRedmineSerializerConfiguration
{
    /// <summary>
    /// Configure the serializer settings
    /// </summary>
    /// <param name="configure">Action to configure the settings</param>
    /// <returns>The configuration instance for method chaining</returns>
    ISerializerConfiguration<TSettings> Configure(Action<TSettings> configure);
}
