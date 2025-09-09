using System.Collections.Generic;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class MyAccountTests()
{
    [Theory]
    [MemberData(nameof(MyAccountDeserializeTheoryData))]
    public void Should_Deserialize_MyAccount(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Redmine.Net.Api.Types.MyAccount>(input);
        Assert.Equal(3, output.Id);
        Assert.Equal("dlopper", output.Login);
        Assert.False(output.IsAdmin);
        Assert.Equal("Dave", output.FirstName);
        Assert.Equal("Lopper", output.LastName);
        Assert.Equal("dlopper@somenet.foo", output.Email);
        Assert.Equal("c308a59c9dea95920b13522fb3e0fb7fae4f292d", output.ApiKey);
        AssertDateTime.Equal("2006-07-19T17:33:19", output.CreatedOn);
        AssertDateTime.Equal("2020-06-14T13:03:34", output.LastLoginOn);
    }
    
    [Theory]
    [MemberData(nameof(MyAccountWithCustomFieldsDeserializeTheoryData))]
    public void Should_Deserialize_MyAccount_With_CustomFields(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
      
        var output = serializer.Deserialize<Redmine.Net.Api.Types.MyAccount>(input);
        Assert.Equal(3, output.Id);
        Assert.Equal("dlopper", output.Login);
        Assert.False(output.IsAdmin);
        Assert.Equal("Dave", output.FirstName);
        Assert.Equal("Lopper", output.LastName);
        Assert.Equal("dlopper@somenet.foo", output.Email);
        Assert.Equal("c308a59c9dea95920b13522fb3e0fb7fae4f292d", output.ApiKey);
        AssertDateTime.Equal("2006-07-19T17:33:19", output.CreatedOn);
        AssertDateTime.Equal("2020-06-14T13:03:34", output.LastLoginOn);
        
        Assert.NotNull(output.CustomFields);
        Assert.Equal(2, output.CustomFields.Count);
        Assert.Equal("Phone number", output.CustomFields[0].Name);
        Assert.Equal(4, output.CustomFields[0].Id);
        Assert.Equal("Money", output.CustomFields[1].Name);
        Assert.Equal(5, output.CustomFields[1].Id);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> MyAccountDeserializeTheoryData
    {
        get
        {
            const string json = """
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
                                    "api_key": "c308a59c9dea95920b13522fb3e0fb7fae4f292d"
                                  }
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> MyAccountWithCustomFieldsDeserializeTheoryData
    {
        get
        {
            const string json = """
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

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}