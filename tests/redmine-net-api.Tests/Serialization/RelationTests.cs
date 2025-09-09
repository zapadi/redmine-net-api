using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class RelationTests()
{
    [Theory]
    [MemberData(nameof(RelationDeserializeTheoryData))]
    public void Should_Deserialize_Relation(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<IssueRelation>(input);
        
        Assert.NotNull(output);
        Assert.Equal(1819, output.Id);
        Assert.Equal(8470, output.IssueId);
        Assert.Equal(8469, output.IssueToId);
        Assert.Equal(IssueRelationType.Relates, output.Type);
        Assert.Null(output.Delay);
    }
    
    [Theory]
    [MemberData(nameof(RelationsDeserializeTheoryData))]
    public void Should_Deserialize_Relations(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var output = serializer.DeserializeToPagedResults<IssueRelation>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var relations = output.Items.ToList();
        Assert.Equal(2, relations.Count);

        Assert.Equal(1819, relations[0].Id);
        Assert.Equal(8470, relations[0].IssueId);
        Assert.Equal(8469, relations[0].IssueToId);
        Assert.Equal(IssueRelationType.Relates, relations[0].Type);
        Assert.Null(relations[0].Delay);

        Assert.Equal(1820, relations[1].Id);
        Assert.Equal(8470, relations[1].IssueId);
        Assert.Equal(8467, relations[1].IssueToId);
        Assert.Equal(IssueRelationType.Relates, relations[1].Type);
        Assert.Null(relations[1].Delay);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> RelationDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "relation": {
                                    "id": 1819,
                                    "issue_id": 8470,
                                    "issue_to_id": 8469,
                                    "relation_type": "relates",
                                    "delay": null
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <relation>
                                   <id>1819</id>
                                   <issue_id>8470</issue_id>
                                   <issue_to_id>8469</issue_to_id>
                                   <relation_type>relates</relation_type>
                                   <delay/>
                               </relation>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> RelationsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "relations": [
                                    {
                                      "id": 1819,
                                      "issue_id": 8470,
                                      "issue_to_id": 8469,
                                      "relation_type": "relates",
                                      "delay": null
                                    },
                                    {
                                      "id": 1820,
                                      "issue_id": 8470,
                                      "issue_to_id": 8467,
                                      "relation_type": "relates",
                                      "delay": null
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <relations type="array">
                                   <relation>
                                       <id>1819</id>
                                       <issue_id>8470</issue_id>
                                       <issue_to_id>8469</issue_to_id>
                                       <relation_type>relates</relation_type>
                                       <delay/>
                                   </relation>
                                   <relation>
                                       <id>1820</id>
                                       <issue_id>8470</issue_id>
                                       <issue_to_id>8467</issue_to_id>
                                       <relation_type>relates</relation_type>
                                       <delay/>
                                   </relation>
                               </relations>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

  
