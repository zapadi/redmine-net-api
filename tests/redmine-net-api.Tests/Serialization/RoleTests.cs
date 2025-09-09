using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public sealed class RoleTests()
{
    [Theory]
    [MemberData(nameof(RoleDeserializeTheoryData))]
    public void Should_Deserialize_Role(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var role = serializer.Deserialize<Role>(input);

        Assert.Equal(5, role.Id);
        Assert.Equal("Reporter", role.Name);
        Assert.True(role.IsAssignable);
        Assert.Equal("default", role.IssuesVisibility);
        Assert.Equal("all", role.TimeEntriesVisibility);
        Assert.Equal("all", role.UsersVisibility);
    }

    [Theory]
    [MemberData(nameof(RoleWithPermissionsDeserializeTheoryData))]
    public void Should_Deserialize_Role_And_Permissions(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var role = serializer.Deserialize<Role>(input);
        
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

    [Theory]
    [MemberData(nameof(RolesDeserializeTheoryData))]
    public void Should_Deserialize_Roles(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Role>(input);
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);
       
        var roles = output.Items.ToList();
        Assert.Equal(1, roles[0].Id);
        Assert.Equal("Manager", roles[0].Name);

        Assert.Equal(2, roles[1].Id);
        Assert.Equal("Developer", roles[1].Name);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> RoleDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "role": {
                                    "id": 5,
                                    "name": "Reporter",
                                    "assignable": true,
                                    "issues_visibility": "default",
                                    "time_entries_visibility": "all",
                                    "users_visibility": "all"
                                  }
                                }
                                """;

            const string xml = """
                               <role>
                                   <id>5</id>
                                   <name>Reporter</name>
                                   <assignable>true</assignable>
                                   <issues_visibility>default</issues_visibility>
                                   <time_entries_visibility>all</time_entries_visibility>
                                   <users_visibility>all</users_visibility>
                               </role>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> RoleWithPermissionsDeserializeTheoryData
    {
        get
        {
            const string json = """
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
                                      "add_issue_notes"
                                    ]
                                  }
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> RolesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "roles": [
                                    {
                                      "id": 1,
                                      "name": "Manager"
                                    },
                                    {
                                      "id": 2,
                                      "name": "Developer"
                                    }
                                  ]
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}