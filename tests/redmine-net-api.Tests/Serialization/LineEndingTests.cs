using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class LineEndingTests
{
    [Theory]
    [InlineData(SerializerKind.Xml)]
    [InlineData(SerializerKind.NewtonsoftJson)]
    public void Should_Serialize_LineEndings_Lf_On_Every_Os(SerializerKind serializerKind)
    {
        var project = new Project {Name = "TestProject", Identifier = "TestProjectIdentifier"};

        var output = SerializerFactory.Create(serializerKind).Serialize(project);

        Assert.Contains("\n", output);
        Assert.DoesNotContain("\r", output);
    }

    [Theory]
    [InlineData(SerializerKind.Xml)]
    [InlineData(SerializerKind.NewtonsoftJson)]
    public void Should_Keep_Lf_In_Text_Values_On_Every_Os(SerializerKind serializerKind)
    {
        var serializer = SerializerFactory.Create(serializerKind);

        var issue = serializer.Serialize(new Issue { Subject = "s", Description = "line1\nline2" });
        var journal = SerializationHelper.SerializeJournal("line1\nline2", null, serializer);

        Assert.DoesNotContain("\r", issue);
        Assert.DoesNotContain("\r", journal);
    }
}
