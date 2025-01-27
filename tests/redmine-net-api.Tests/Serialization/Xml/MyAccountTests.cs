using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class MyAccountTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_MyAccount()
    {
        const string input = """
         <?xml version="1.0" encoding="UTF-8"?>
         <user>
           <id>3</id>
           <login>dlopper</login>
           <admin>false</admin>
           <firstname>Dave</firstname>
           <lastname>Lopper</lastname>
           <mail>dlopper@somenet.foo</mail>
           <created_on>2006-07-19T17:33:19Z</created_on>
           <last_login_on>2020-06-14T13:03:34Z</last_login_on>
           <api_key>c308a59c9dea95920b13522fb3e0fb7fae4f292d</api_key>
         </user>
         """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.MyAccount>(input);
        Assert.Equal(3, output.Id);
        Assert.Equal("dlopper", output.Login);
        Assert.False(output.IsAdmin);
        Assert.Equal("Dave", output.FirstName);
        Assert.Equal("Lopper", output.LastName);
        Assert.Equal("dlopper@somenet.foo", output.Email);
        Assert.Equal("c308a59c9dea95920b13522fb3e0fb7fae4f292d", output.ApiKey);
    }
    
    [Fact]
    public void Should_Deserialize_MyAccount_With_CustomFields()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <user>
            <id>3</id>
            <login>dlopper</login>
            <admin>false</admin>
            <firstname>Dave</firstname>
            <lastname>Lopper</lastname>
            <mail>dlopper@somenet.foo</mail>
            <created_on>2006-07-19T17:33:19Z</created_on>
            <last_login_on>2020-06-14T13:03:34Z</last_login_on>
            <api_key>c308a59c9dea95920b13522fb3e0fb7fae4f292d</api_key>
            <custom_fields type="array">
             <custom_field id="4" name="Phone number">
               <value/>
             </custom_field>
             <custom_field id="5" name="Money">
               <value/>
             </custom_field>
            </custom_fields>
        </user>
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