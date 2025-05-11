using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public sealed class RoleTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Role()
    {
        const string input = """
        <role>
            <id>5</id>
            <name>Reporter</name>
            <assignable>true</assignable>
            <issues_visibility>default</issues_visibility>
            <time_entries_visibility>all</time_entries_visibility>
            <users_visibility>all</users_visibility>
        </role>
        """;
        var role = fixture.Serializer.Deserialize<Role>(input);

        Assert.Equal(5, role.Id);
        Assert.Equal("Reporter", role.Name);
        Assert.True(role.IsAssignable);
        Assert.Equal("default", role.IssuesVisibility);
        Assert.Equal("all", role.TimeEntriesVisibility);
        Assert.Equal("all", role.UsersVisibility);
    }

    [Fact]
    public void Should_Deserialize_Role_And_Permissions()
    {
        const string input = """
            <role>
            <id>5</id>
            <name>Reporter</name>
            <assignable>true</assignable>
            <issues_visibility>default</issues_visibility>
            <time_entries_visibility>all</time_entries_visibility>
            <users_visibility>all</users_visibility>
            <permissions type="array">
                <permission>view_issues</permission>
                <permission>add_issues</permission>
                <permission>add_issue_notes</permission>
            </permissions>
            </role>
        """;
        var role = fixture.Serializer.Deserialize<Role>(input);
        
        Assert.Equal(5, role.Id);
        Assert.Equal("Reporter", role.Name);
        Assert.True(role.IsAssignable);
        Assert.Equal("default", role.IssuesVisibility);
        Assert.Equal("all", role.TimeEntriesVisibility);
        Assert.Equal("all", role.UsersVisibility);
        Assert.Equal(3, role.Permissions.Count);
        Assert.Equal("view_issues", role.Permissions[0].Info);
        Assert.Equal("add_issues", role.Permissions[1].Info);
        Assert.Equal("add_issue_notes", role.Permissions[2].Info);
    }

    [Fact]
    public void Should_Deserialize_Roles()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <roles type="array">
            <role>
                <id>1</id>
                <name>Manager</name>
            </role>
            <role>
                <id>2</id>
                <name>Developer</name>
            </role>
        </roles>
        """;

        var output = fixture.Serializer.DeserializeToPagedResults<Role>(input);
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);
       
        var roles = output.Items.ToList();
        Assert.Equal(1, roles[0].Id);
        Assert.Equal("Manager", roles[0].Name);

        Assert.Equal(2, roles[1].Id);
        Assert.Equal("Developer", roles[1].Name);
    }
}