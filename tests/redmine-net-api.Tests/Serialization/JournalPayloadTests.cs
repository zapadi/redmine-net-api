using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class JournalPayloadTests
{
    [Fact]
    public void Journal_Json_Should_Be_Wrapped_In_Journal_Root()
    {
        var journal = new Journal { Notes = "n" };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(journal);

        Assert.True(JToken.DeepEquals(JObject.Parse("""{"journal":{"notes":"n"}}"""), JObject.Parse(output)), output);
    }

    [Fact]
    public void Journal_Json_Should_Send_PrivateNotes_When_True()
    {
        var journal = new Journal { Notes = "n", PrivateNotes = true };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(journal);

        Assert.True(JToken.DeepEquals(JObject.Parse("""{"journal":{"notes":"n","private_notes":"true"}}"""), JObject.Parse(output)), output);
    }

    [Fact]
    public void Journal_Xml_Should_Send_PrivateNotes_When_True()
    {
        var journal = new Journal { Notes = "n", PrivateNotes = true };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(journal);

        Assert.True(XNode.DeepEquals(XElement.Parse("<journal><notes>n</notes><private_notes>true</private_notes></journal>"), XElement.Parse(output)), output);
    }

    [Fact]
    public void Journal_Xml_Should_Not_Send_PrivateNotes_When_False()
    {
        var journal = new Journal { Notes = "n" };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(journal);

        Assert.True(XNode.DeepEquals(XElement.Parse("<journal><notes>n</notes></journal>"), XElement.Parse(output)), output);
    }
}
