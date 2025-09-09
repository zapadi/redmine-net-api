using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class UserTests()
{
    [Theory]
    [MemberData(nameof(UserDeserializeTheoryData))]
    public void Should_Deserialize_User(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<User>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.Id);
        Assert.Equal("jplang", output.Login);
        Assert.Equal("Jean-Philippe", output.FirstName);
        Assert.Equal("Lang", output.LastName);
        Assert.Equal("jp_lang@yahoo.fr", output.Email);
        // AssertDateTime.Equal("2007-9-28T00:16:04", output.CreatedOn);
        // AssertDateTime.Equal("2010-8-1T18:05:45", output.UpdatedOn);
        // AssertDateTime.Equal("2011-8-1T18:05:45", output.LastLoginOn);
        // AssertDateTime.Equal("2011-8-1T18:05:45", output.PasswordChangedOn);
        Assert.Equal("ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d", output.ApiKey);
        Assert.Empty(output.AvatarUrl);
        Assert.Equal(UserStatus.StatusActive, output.Status);
    }
    
    [Theory]
    [MemberData(nameof(UserWithMembershipsDeserializeTheoryData))]
    public void Should_Deserialize_User_With_Memberships(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<User>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.Id);
        Assert.Equal("jplang", output.Login);
        Assert.Equal("Jean-Philippe", output.FirstName);
        Assert.Equal("Lang", output.LastName);
        Assert.Equal("jp_lang@yahoo.fr", output.Email);
        // AssertDateTime.Equal("2007-9-28T00:16:04", output.CreatedOn);
        // AssertDateTime.Equal("2010-8-1T18:05:45", output.UpdatedOn);
        // AssertDateTime.Equal("2011-8-1T18:05:45", output.LastLoginOn);
        // AssertDateTime.Equal("2011-8-1T18:05:45", output.PasswordChangedOn);
        Assert.Equal("ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d", output.ApiKey);
        Assert.Empty(output.AvatarUrl);
        Assert.Equal(UserStatus.StatusActive, output.Status);

        var memberships = output.Memberships.ToList();
        Assert.Single(memberships);
        Assert.Equal("Redmine", memberships[0].Project.Name);
        Assert.Equal(1, memberships[0].Project.Id);

        var roles = memberships[0].Roles.ToList();
        Assert.Equal(2, roles.Count);
        Assert.Equal("Administrator", roles[0].Name);
        Assert.Equal(3, roles[0].Id);
        Assert.Equal("Contributor", roles[1].Name);
        Assert.Equal(4, roles[1].Id);
    }
    
    [Theory]
    [MemberData(nameof(UserWithGroupsDeserializeTheoryData))]
    public void Should_Deserialize_User_With_Groups(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<User>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.Id);
        Assert.Equal("jplang", output.Login);
        Assert.Equal("Jean-Philippe", output.FirstName);
        Assert.Equal("Lang", output.LastName);
        Assert.Equal("jp_lang@yahoo.fr", output.Email);
        // AssertDateTime.Equal("2007-9-28T00:16:04", output.CreatedOn);
        // AssertDateTime.Equal("2010-8-1T18:05:45", output.UpdatedOn);
        // AssertDateTime.Equal("2011-8-1T18:05:45", output.LastLoginOn);
        // AssertDateTime.Equal("2011-8-1T18:05:45", output.PasswordChangedOn);
        Assert.Equal("ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d", output.ApiKey);
        Assert.Empty(output.AvatarUrl);
        Assert.Equal(UserStatus.StatusActive, output.Status);

        var groups = output.Groups.ToList();
        Assert.Single(groups);
        Assert.Equal("Developers", groups[0].Name);
        Assert.Equal(20, groups[0].Id);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UserDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "user": {
                                    "id": 3,
                                    "login": "jplang",
                                    "firstname": "Jean-Philippe",
                                    "lastname": "Lang",
                                    "mail": "jp_lang@yahoo.fr",
                                    "created_on": "2007-09-28T00:16:04+02:00",
                                    "updated_on": "2010-08-01T18:05:45+02:00",
                                    "last_login_on": "2011-08-01T18:05:45+02:00",
                                    "passwd_changed_on": "2011-08-01T18:05:45+02:00",
                                    "api_key": "ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d",
                                    "avatar_url": null,
                                    "status": 1
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <user>
                                   <id>3</id>
                                   <login>jplang</login>
                                   <firstname>Jean-Philippe</firstname>
                                   <lastname>Lang</lastname>
                                   <mail>jp_lang@yahoo.fr</mail>
                                   <created_on>2007-09-28T00:16:04+02:00</created_on>
                                   <updated_on>2010-08-01T18:05:45+02:00</updated_on>
                                   <last_login_on>2011-08-01T18:05:45+02:00</last_login_on>
                                   <passwd_changed_on>2011-08-01T18:05:45+02:00</passwd_changed_on>
                                   <api_key>ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d</api_key>
                                   <avatar_url></avatar_url>
                                   <status>1</status>
                               </user>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UserWithMembershipsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "user": {
                                    "id": 3,
                                    "login": "jplang",
                                    "firstname": "Jean-Philippe",
                                    "lastname": "Lang",
                                    "mail": "jp_lang@yahoo.fr",
                                    "created_on": "2007-09-28T00:16:04+02:00",
                                    "updated_on": "2010-08-01T18:05:45+02:00",
                                    "last_login_on": "2011-08-01T18:05:45+02:00",
                                    "passwd_changed_on": "2011-08-01T18:05:45+02:00",
                                    "api_key": "ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d",
                                    "avatar_url": null,
                                    "status": 1,
                                    "custom_fields": [],
                                    "memberships": [
                                      {
                                        "project": {
                                          "id": 1,
                                          "name": "Redmine"
                                        },
                                        "roles": [
                                          {
                                            "id": 3,
                                            "name": "Administrator"
                                          },
                                          {
                                            "id": 4,
                                            "name": "Contributor"
                                          }
                                        ]
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <user>
                                <id>3</id>
                                <login>jplang</login>
                                <firstname>Jean-Philippe</firstname>
                                <lastname>Lang</lastname>
                                <mail>jp_lang@yahoo.fr</mail>
                                <created_on>2007-09-28T00:16:04+02:00</created_on>
                                <updated_on>2010-08-01T18:05:45+02:00</updated_on>
                                <last_login_on>2011-08-01T18:05:45+02:00</last_login_on>
                                <passwd_changed_on>2011-08-01T18:05:45+02:00</passwd_changed_on>
                                <api_key>ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d</api_key>
                                <avatar_url></avatar_url>
                                <status>1</status>
                                <custom_fields type="array" />
                                <memberships type="array">
                                    <membership>
                                        <project name="Redmine" id="1"/>
                                        <roles type="array">
                                            <role name="Administrator" id="3"/>
                                            <role name="Contributor" id="4"/>
                                        </roles>
                                    </membership>
                                </memberships>
                               </user>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UserWithGroupsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "user": {
                                    "id": 3,
                                    "login": "jplang",
                                    "firstname": "Jean-Philippe",
                                    "lastname": "Lang",
                                    "mail": "jp_lang@yahoo.fr",
                                    "created_on": "2007-09-28T00:16:04+02:00",
                                    "updated_on": "2010-08-01T18:05:45+02:00",
                                    "last_login_on": "2011-08-01T18:05:45+02:00",
                                    "passwd_changed_on": "2011-08-01T18:05:45+02:00",
                                    "api_key": "ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d",
                                    "avatar_url": null,
                                    "status": 1,
                                    "custom_fields": [],
                                    "groups": [
                                      {
                                        "id": 20,
                                        "name": "Developers"
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <user>
                                <id>3</id>
                                <login>jplang</login>
                                <firstname>Jean-Philippe</firstname>
                                <lastname>Lang</lastname>
                                <mail>jp_lang@yahoo.fr</mail>
                                <created_on>2007-09-28T00:16:04+02:00</created_on>
                                <updated_on>2010-08-01T18:05:45+02:00</updated_on>
                                <last_login_on>2011-08-01T18:05:45+02:00</last_login_on>
                                <passwd_changed_on>2011-08-01T18:05:45+02:00</passwd_changed_on>
                                <api_key>ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d</api_key>
                                <avatar_url></avatar_url>
                                <status>1</status>
                                <custom_fields type="array" />
                                <groups type="array">
                                    <group id="20" name="Developers"/>
                                </groups>
                               </user>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

