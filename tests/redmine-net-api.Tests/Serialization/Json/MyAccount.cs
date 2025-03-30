using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public class MyAccount(JsonSerializerFixture fixture)
{
    [Fact]
    public void Test_Xml_Serialization()
    {
        const string input = """
                             {
                               "user": {
                                 "id": 3,
                                 "login": "dlopper",
                                 "admin": false,
                                 "firstname": "Dave",
                                 "lastname": "Lopper",
                                 "mail": "dlopper@somenet.foo",
                                 "created_on": "2006-07-19T17:33:19Z",
                                 "last_login_on": "2020-06-14T13:03:34Z",
                                 "api_key": "c308a59c9dea95920b13522fb3e0fb7fae4f292d",
                                 "custom_fields": [
                                   {
                                     "id": 4,
                                     "name": "Phone number",
                                     "value": null
                                   },
                                   {
                                     "id": 5,
                                     "name": "Money",
                                     "value": null
                                   }
                                 ]
                               }
                             }
                             """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.MyAccount>(input);
        Assert.Equal(3, output.Id);
        Assert.Equal("dlopper", output.Login);
        Assert.False(output.IsAdmin);
        Assert.Equal("Dave", output.FirstName);
        Assert.Equal("Lopper", output.LastName);
        Assert.Equal("dlopper@somenet.foo", output.Email);
        Assert.Equal("c308a59c9dea95920b13522fb3e0fb7fae4f292d", output.ApiKey);
        Assert.NotNull(output.CustomFields);
        Assert.Equal(2, output.CustomFields.Count);
        Assert.Equal("Phone number", output.CustomFields[0].Name);
        Assert.Equal(4, output.CustomFields[0].Id);
        Assert.Equal("Money", output.CustomFields[1].Name);
        Assert.Equal(5, output.CustomFields[1].Id);
    }
}