using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class GroupTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Group()
    {
        const string input = """
        <group>
            <id>20</id>
            <name>Developers</name>
            <users type="array">
                <user id="5" name="John Smith"/>
                <user id="8" name="Dave Loper"/>
            </users>
        </group>
        """;
        
        var output = fixture.Serializer.Deserialize<Group>(input);
        Assert.NotNull(output);
        Assert.Equal(20, output.Id);
        Assert.Equal("Developers", output.Name);
        Assert.NotNull(output.Users);
        Assert.Equal(2, output.Users.Count);
        Assert.Equal("John Smith", output.Users[0].Name);
        Assert.Equal("Dave Loper", output.Users[1].Name);
        Assert.Equal(5, output.Users[0].Id);
        Assert.Equal(8, output.Users[1].Id);
    }
    
    [Fact]
    public void Should_Deserialize_Groups()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <groups type="array">
            <group>
                <id>53</id>
                <name>Managers</name>
            </group>
            <group>
                <id>55</id>
                <name>Developers</name>
            </group>
        </groups>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Group>(input);
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var groups = output.Items.ToList();
        Assert.Equal(53, groups[0].Id);
        Assert.Equal("Managers", groups[0].Name);

        Assert.Equal(55, groups[1].Id);
        Assert.Equal("Developers", groups[1].Name);
    }
}