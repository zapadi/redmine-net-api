using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class UserTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_User()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.Deserialize<User>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.Id);
        Assert.Equal("jplang", output.Login);
        Assert.Equal("Jean-Philippe", output.FirstName);
        Assert.Equal("Lang", output.LastName);
        Assert.Equal("jp_lang@yahoo.fr", output.Email);
        Assert.Equal(new DateTime(2007, 9, 28, 0, 16, 4, DateTimeKind.Local).AddHours(1), output.CreatedOn);
        Assert.Equal(new DateTime(2010, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.UpdatedOn);
        Assert.Equal(new DateTime(2011, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.LastLoginOn);
        Assert.Equal(new DateTime(2011, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.PasswordChangedOn);
        Assert.Equal("ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d", output.ApiKey);
        Assert.Empty(output.AvatarUrl);
        Assert.Equal(UserStatus.StatusActive, output.Status);
    }
    
    [Fact]
    public void Should_Deserialize_User_With_Memberships()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.Deserialize<User>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.Id);
        Assert.Equal("jplang", output.Login);
        Assert.Equal("Jean-Philippe", output.FirstName);
        Assert.Equal("Lang", output.LastName);
        Assert.Equal("jp_lang@yahoo.fr", output.Email);
        Assert.Equal(new DateTime(2007, 9, 28, 0, 16, 4, DateTimeKind.Local).AddHours(1), output.CreatedOn);
        Assert.Equal(new DateTime(2010, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.UpdatedOn);
        Assert.Equal(new DateTime(2011, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.LastLoginOn);
        Assert.Equal(new DateTime(2011, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.PasswordChangedOn);
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
    
    [Fact]
    public void Should_Deserialize_User_With_Groups()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.Deserialize<User>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.Id);
        Assert.Equal("jplang", output.Login);
        Assert.Equal("Jean-Philippe", output.FirstName);
        Assert.Equal("Lang", output.LastName);
        Assert.Equal("jp_lang@yahoo.fr", output.Email);
        Assert.Equal(new DateTime(2007, 9, 28, 0, 16, 4, DateTimeKind.Local).AddHours(1), output.CreatedOn);
        Assert.Equal(new DateTime(2010, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.UpdatedOn);
        Assert.Equal(new DateTime(2011, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.LastLoginOn);
        Assert.Equal(new DateTime(2011, 8, 1, 18, 5, 45, DateTimeKind.Local).AddHours(1), output.PasswordChangedOn);
        Assert.Equal("ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d", output.ApiKey);
        Assert.Empty(output.AvatarUrl);
        Assert.Equal(UserStatus.StatusActive, output.Status);

        var groups = output.Groups.ToList();
        Assert.Single(groups);
        Assert.Equal("Developers", groups[0].Name);
        Assert.Equal(20, groups[0].Id);
    }
}

