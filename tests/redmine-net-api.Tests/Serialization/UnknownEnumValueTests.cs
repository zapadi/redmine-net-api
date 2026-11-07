using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class UnknownEnumValueTests
{
    [Theory]
    [InlineData("archived", "galaxy")]
    [InlineData("", "")]
    public void Should_Deserialize_Json_Version_With_Unknown_Status_And_Sharing(string status, string sharing)
    {
        var input = $$"""
                      {
                          "version": {
                              "id": 1,
                              "status": "{{status}}",
                              "sharing": "{{sharing}}",
                              "name": "1.0"
                          }
                      }
                      """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Deserialize<Version>(input);

        Assert.Equal(1, output.Id);
        Assert.Equal(VersionStatus.None, output.Status);
        Assert.Equal(VersionSharing.Unknown, output.Sharing);
        Assert.Equal("1.0", output.Name);
    }

    [Fact]
    public void Should_Deserialize_Xml_Version_With_Unknown_Status_And_Sharing()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <version>
                                 <id>1</id>
                                 <status>archived</status>
                                 <sharing>galaxy</sharing>
                                 <name>1.0</name>
                             </version>
                             """;

        var output = SerializerFactory.Create(SerializerKind.Xml).Deserialize<Version>(input);

        Assert.Equal(1, output.Id);
        Assert.Equal(VersionStatus.None, output.Status);
        Assert.Equal(VersionSharing.Unknown, output.Sharing);
        Assert.Equal("1.0", output.Name);
    }

    [Fact]
    public void Should_Deserialize_Json_Relation_With_Unknown_Type()
    {
        const string input = """
                             {
                                 "relation": {
                                     "id": 1,
                                     "issue_id": 2,
                                     "issue_to_id": 3,
                                     "relation_type": "blocks_partially"
                                 }
                             }
                             """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Deserialize<IssueRelation>(input);

        Assert.Equal(1, output.Id);
        Assert.Equal(3, output.IssueToId);
        Assert.Equal(IssueRelationType.Undefined, output.Type);
    }

    [Fact]
    public void Should_Deserialize_Xml_Relation_With_Unknown_Type()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <relation>
                                 <id>1</id>
                                 <issue_id>2</issue_id>
                                 <issue_to_id>3</issue_to_id>
                                 <relation_type>blocks_partially</relation_type>
                             </relation>
                             """;

        var output = SerializerFactory.Create(SerializerKind.Xml).Deserialize<IssueRelation>(input);

        Assert.Equal(1, output.Id);
        Assert.Equal(3, output.IssueToId);
        Assert.Equal(IssueRelationType.Undefined, output.Type);
    }
}
