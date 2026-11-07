using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

public sealed class XmlSerializerCacheTests
{
    private static readonly XmlAttributeOverrides Overrides = new();

    [Fact]
    public void GetSerializer_Should_Return_Same_Instance_For_Same_Type()
    {
        Assert.Same(XmlSerializerCache.Instance.GetSerializer(typeof(Issue)), XmlSerializerCache.Instance.GetSerializer(typeof(Issue)));
    }

    [Fact]
    public void GetSerializer_Should_Return_Same_Instance_For_Equal_Root_Attributes()
    {
        var first = XmlSerializerCache.Instance.GetSerializer(typeof(int), Overrides, Type.EmptyTypes, new XmlRootAttribute("id"), null);
        var second = XmlSerializerCache.Instance.GetSerializer(typeof(int), Overrides, Type.EmptyTypes, new XmlRootAttribute("id"), null);

        Assert.Same(first, second);
    }

    [Fact]
    public void GetSerializer_Should_Return_Different_Instances_For_Different_Roots()
    {
        var first = XmlSerializerCache.Instance.GetSerializer(typeof(int), Overrides, Type.EmptyTypes, new XmlRootAttribute("id"), null);
        var second = XmlSerializerCache.Instance.GetSerializer(typeof(int), Overrides, Type.EmptyTypes, new XmlRootAttribute("user_id"), null);

        Assert.NotSame(first, second);
    }

    [Fact]
    public void XmlRedmineSerializer_Serialize_Should_Use_Cache()
    {
        new XmlRedmineSerializer().Serialize(new SerializeProbe { Value = 1 });

        Assert.True(IsCached(typeof(SerializeProbe)));
    }

    [Fact]
    public void XmlRedmineSerializer_Deserialize_Should_Use_Cache()
    {
        var probe = new XmlRedmineSerializer().Deserialize<DeserializeProbe>("<DeserializeProbe><Value>2</Value></DeserializeProbe>");

        Assert.Equal(2, probe.Value);
        Assert.True(IsCached(typeof(DeserializeProbe)));
    }

    [Fact]
    public void XmlWriter_WriteArray_Should_Use_Cache()
    {
        Write(writer => writer.WriteArray("items", new List<WriteArrayProbe> { new() { Value = 3 } }));

        Assert.True(IsCached(typeof(WriteArrayProbe)));
    }

    [Fact]
    public void XmlWriter_WriteArray_With_Root_Should_Use_Cache()
    {
        Write(writer => writer.WriteArray("items", new List<int> { 4 }, "item_id"));

        Assert.True(IsCached(typeof(int), new XmlRootAttribute("item_id")));
    }

    private static void Write(Action<XmlWriter> write)
    {
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment });
        write(xmlWriter);
    }

    private static bool IsCached(Type type, XmlRootAttribute root = null)
    {
        var serializers = (Dictionary<string, XmlSerializer>)GetField("serializers");

        lock (GetField("syncRoot"))
        {
            foreach (var key in serializers.Keys)
            {
                if (key.StartsWith(type.AssemblyQualifiedName + "??", StringComparison.Ordinal) && (root == null || key.Contains("??" + root.ElementName + "??")))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static object GetField(string name)
    {
        return typeof(XmlSerializerCache).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(XmlSerializerCache.Instance)!;
    }

    public sealed class SerializeProbe
    {
        public int Value { get; set; }
    }

    public sealed class DeserializeProbe
    {
        public int Value { get; set; }
    }

    public sealed class WriteArrayProbe
    {
        public int Value { get; set; }
    }
}
