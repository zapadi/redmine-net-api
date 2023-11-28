using System;

namespace Redmine.Net.Api.Serialization;

/// <summary>
/// Factory for creating RedmineSerializer instances
/// </summary>
internal static class RedmineSerializerFactory
{
    public static IRedmineSerializer CreateSerializer(SerializationType type)
    {
        return type switch
        {
            SerializationType.Xml => new XmlRedmineSerializer(),
            SerializationType.Json => new JsonRedmineSerializer(),
            _ => throw new NotImplementedException($"No serializer has been implemented for the serialization type: {type}")
        };
    }
}