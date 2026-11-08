using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
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
}