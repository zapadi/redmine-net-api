using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class IssueStatusTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Issue_Statuses()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <issue_statuses type="array">
            <issue_status>
                <id>1</id>
                <name>New</name>
                <is_closed>false</is_closed>
            </issue_status>
            <issue_status>
                <id>2</id>
                <name>Closed</name>
                <is_closed>true</is_closed>
            </issue_status>
        </issue_statuses>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.IssueStatus>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var issueStatuses = output.Items.ToList();
        Assert.Equal(2, issueStatuses.Count);

        Assert.Equal(1, issueStatuses[0].Id);
        Assert.Equal("New", issueStatuses[0].Name);
        Assert.False(issueStatuses[0].IsClosed);

        Assert.Equal(2, issueStatuses[1].Id);
        Assert.Equal("Closed", issueStatuses[1].Name);
        Assert.True(issueStatuses[1].IsClosed);
    }
}

