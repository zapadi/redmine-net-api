using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public class IssueCustomFieldsTests(JsonSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Issue_With_CustomFields_With_Multiple_Values()
    {
        const string input = """
                             {
                                  "custom_fields":[
                                      {"value":["1.0.1","1.0.2"],"multiple":true,"name":"Affected version","id":1},
                                      {"value":"Fixed","name":"Resolution","id":2}
                                    ],
                                    "total_count":2
                             }
                             """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<IssueCustomField>(input);

        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);
        
        var customFields = output.Items.ToList();
        
        Assert.Equal(1, customFields[0].Id);
        Assert.Equal("Affected version", customFields[0].Name);
        Assert.True(customFields[0].Multiple);
        Assert.Equal(2, customFields[0].Values.Count);
        Assert.Equal("1.0.1", customFields[0].Values[0].Info);
        Assert.Equal("1.0.2", customFields[0].Values[1].Info);
        
        Assert.Equal(2, customFields[1].Id);
        Assert.Equal("Resolution", customFields[1].Name);
        Assert.False(customFields[1].Multiple);
        Assert.Equal("Fixed", customFields[1].Values[0].Info);
    }
}