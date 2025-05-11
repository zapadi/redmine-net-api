using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public sealed class MembershipTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Memberships()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <memberships type="array" limit="25" offset="0" total_count="3">
                                 <membership>
                                     <id>1</id>
                                     <project name="Redmine" id="1"/>
                                     <user name="David Robert" id="17"/>
                                     <roles type="array">
                                         <role name="Manager" id="1"/>
                                     </roles>
                                 </membership>
                                 <membership>
                                     <id>3</id>
                                     <project name="Redmine" id="1"/>
                                     <group name="Contributors" id="24"/>
                                     <roles type="array">
                                         <role name="Contributor" id="3"/>
                                     </roles>
                                 </membership>
                                 <membership>
                                     <id>4</id>
                                     <project name="Redmine" id="1"/>
                                     <user name="John Smith" id="27"/>
                                     <roles type="array">
                                         <role name="Developer" id="2" />
                                     <role name="Contributor" id="3" inherited="true" />
                                 </roles>
                                 </membership>
                             </memberships>
                             """;

        var output = fixture.Serializer.DeserializeToPagedResults<Membership>(input);
    }

    [Fact]
    public void Should_Deserialize_Membership()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <membership>
            <id>1</id>
            <project name="Redmine" id="1"/>
            <user name="David Robert" id="17"/>
        </membership>
        """;
        
        var output = fixture.Serializer.Deserialize<Membership>(input);
        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("David Robert", output.User.Name);
        Assert.Equal(17, output.User.Id);
    }
    
    [Fact]
    public void Should_Deserialize_Membership_With_Roles()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <membership>
          <id>1</id>
          <project name="Redmine" id="1"/>
          <user name="David Robert" id="17"/>
          <roles type="array">
            <role name="Manager" id="1"/>
          </roles>
        </membership>
        """;
        
        var output = fixture.Serializer.Deserialize<Membership>(input);
        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("David Robert", output.User.Name);
        Assert.Equal(17, output.User.Id);
        Assert.NotNull(output.Roles);
        Assert.Single(output.Roles);
        Assert.Equal("Manager", output.Roles[0].Name);
        Assert.Equal(1, output.Roles[0].Id);
    }
}