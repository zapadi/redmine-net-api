using System;
using System.Collections.Generic;
using Redmine.Net.Api.Serialization;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

public static class SerializerFactory
{
    private static readonly Dictionary<SerializerKind, IRedmineSerializer> _serializers = new Dictionary<SerializerKind, IRedmineSerializer>();
    
    public static IRedmineSerializer Create(SerializerKind kind)
    {
        if (_serializers.TryGetValue(kind, out var serializer))
        {
            return serializer;
        }
        
        IRedmineSerializer ser = kind switch
        {
            SerializerKind.Xml => new XmlRedmineSerializer(),
            SerializerKind.NewtonsoftJson => new JsonRedmineSerializer(),
            // SerializerKind.SystemTextJson => new JsonTextRedmineSerializer(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
        _serializers.Add(kind, ser);
        return ser;
    }
}