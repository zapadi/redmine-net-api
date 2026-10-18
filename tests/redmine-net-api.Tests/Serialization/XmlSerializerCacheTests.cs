using System;
using System.Xml.Serialization;
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
}
