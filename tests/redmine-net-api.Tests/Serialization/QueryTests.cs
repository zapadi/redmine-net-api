using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class QueryTests()
{
    [Theory]
    [MemberData(nameof(QueriesDeserializeTheoryData))]
    public void Queries_Should_Deserialize(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Query>(input);
        
        Assert.NotNull(output);
        Assert.Equal(5, output.TotalItems);

        var queries = output.Items.ToList();
        Assert.Equal(2, queries.Count);

        Assert.Equal(84, queries[0].Id);
        Assert.Equal("Documentation issues", queries[0].Name);
        Assert.True(queries[0].IsPublic);
        Assert.Equal(1, queries[0].ProjectId);

        Assert.Equal(1, queries[1].Id);
        Assert.Equal("Open defects", queries[1].Name);
        Assert.True(queries[1].IsPublic);
        Assert.Equal(1, queries[1].ProjectId);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> QueriesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "queries": [
                                    {
                                      "id": 84,
                                      "name": "Documentation issues",
                                      "is_public": true,
                                      "project_id": 1
                                    },
                                    {
                                      "id": 1,
                                      "name": "Open defects",
                                      "is_public": true,
                                      "project_id": 1
                                    }
                                  ],
                                  "total_count": 5,
                                  "limit": 25,
                                  "offset": 0
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <queries type="array" total_count="5" limit="25" offset="0">
                                   <query>
                                       <id>84</id>
                                       <name>Documentation issues</name>
                                       <is_public>true</is_public>
                                       <project_id>1</project_id>
                                   </query>
                                   <query>
                                       <id>1</id>
                                       <name>Open defects</name>
                                       <is_public>true</is_public>
                                       <project_id>1</project_id>
                                   </query>
                               </queries>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

