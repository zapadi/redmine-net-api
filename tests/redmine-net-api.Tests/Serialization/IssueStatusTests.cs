using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class IssueStatusTests()
{
    [Theory]
    [MemberData(nameof(IssueStatusDeserializeTheoryData))]
    public void Should_Deserialize_Issue_Statuses(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.IssueStatus>(input);
        
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
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueStatusDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue_statuses": [
                                    {
                                      "id": 1,
                                      "name": "New",
                                      "is_closed": false
                                    },
                                    {
                                      "id": 2,
                                      "name": "Closed",
                                      "is_closed": true
                                    }
                                  ]
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

