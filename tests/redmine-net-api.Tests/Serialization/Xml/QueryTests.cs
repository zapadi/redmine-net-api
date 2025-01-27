using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class QueryTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Version()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Query>(input);
        
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
}

