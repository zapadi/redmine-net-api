using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public class ErrorTests(JsonSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Errors()
    {
        const string input = """
                             {
                               "errors":[
                                 "First name can't be blank",
                                 "Email is invalid" 
                               ],
                               "total_count":2
                             }
                             """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Error>(input);

        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);
        
        var errors = output.Items.ToList();
        Assert.Equal("First name can't be blank", errors[0].Info);
        Assert.Equal("Email is invalid", errors[1].Info);
    }
}