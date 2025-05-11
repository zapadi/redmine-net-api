using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public sealed class ErrorTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Errors()
    {
        const string input = """
        <errors type="array">
            <error>First name can't be blank</error>
            <error>Email is invalid</error>
        </errors>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Error>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var errors = output.Items.ToList();
        Assert.Equal("First name can't be blank", errors[0].Info);
        Assert.Equal("Email is invalid", errors[1].Info);

    }
}