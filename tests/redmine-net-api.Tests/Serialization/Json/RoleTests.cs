using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public sealed class RoleTests(JsonSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Role_And_Permissions()
    {
        const string input = """
        {
            "role": {
                "id": 5,
                "name": "Reporter",
                "assignable": true,
                "issues_visibility": "default",
                "time_entries_visibility": "all",
                "users_visibility": "all",
                "permissions": [
                    "view_issues",
                    "add_issues",
                    "add_issue_notes",
                ]
            }
        }
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
}