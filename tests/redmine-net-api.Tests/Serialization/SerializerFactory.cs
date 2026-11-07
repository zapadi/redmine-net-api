using System;
using Redmine.Net.Api.Serialization;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

internal static class SerializerFactory
{
    public static IRedmineSerializer Create(SerializerKind kind)
    {
        return kind switch
        {
            SerializerKind.Xml => new XmlRedmineSerializer(),
            SerializerKind.NewtonsoftJson => new JsonRedmineSerializer(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
}
