using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public class UserTests(JsonSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_User()
    {
        const string input = """ 
         {
           "user":{
            "id": 3,
             "login":"jplang",
             "firstname": "Jean-Philippe",
             "lastname":"Lang",
             "mail":"jp_lang@yahoo.fr",
             "created_on": "2007-09-28T00:16:04+02:00",
             "updated_on":"2010-08-01T18:05:45+02:00",
             "last_login_on":"2011-08-01T18:05:45+02:00",
             "passwd_changed_on": "2011-08-01T18:05:45+02:00",
             "api_key": "ebc3f6b781a6fb3f2b0a83ce0ebb80e0d585189d",
             "avatar_url": "",
             "status": 1 
           }
         }
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
    
}