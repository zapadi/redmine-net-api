using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class MyAccountCustomFieldTests
{
    [Theory]
    [MemberData(nameof(MyAccountCustomFieldsTheoryData))]
    public void Should_Deserialize_Single_Multi_And_Empty_Custom_Field_Values(string input, SerializerKind kind)
    {
        var output = SerializerFactory.Create(kind).Deserialize<MyAccount>(input);

        Assert.Equal(3, output.Id);
        Assert.Equal("Dave", output.FirstName);
        Assert.Equal("dlopper@somenet.foo", output.Email);
        Assert.Equal(4, output.CustomFields.Count);

        var single = output.CustomFields[0];
        Assert.Equal(4, single.Id);
        Assert.Equal("Phone number", single.Name);
        Assert.False(single.Multiple);
        Assert.Equal("555-1234", single.Value);
        Assert.Null(single.Values);

        var multi = output.CustomFields[1];
        Assert.Equal(5, multi.Id);
        Assert.Equal("Skills", multi.Name);
        Assert.True(multi.Multiple);
        Assert.Equal("a", multi.Value);
        Assert.Equal(["a", "b"], multi.Values);

        var empty = output.CustomFields[2];
        Assert.Equal(6, empty.Id);
        Assert.Equal("Tags", empty.Name);
        Assert.True(empty.Multiple);
        Assert.Null(empty.Value);
        Assert.NotNull(empty.Values);
        Assert.Empty(empty.Values);

        var date = output.CustomFields[3];
        Assert.Equal(7, date.Id);
        Assert.Equal("2024-01-02", date.Value);

        Assert.Equal("c308a59c9dea95920b13522fb3e0fb7fae4f292d", output.ApiKey);
    }

    [Fact]
    public void Xml_Should_Write_Single_Value()
    {
        var field = new MyAccountCustomField { Id = 4, Value = "123" };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(field);

        Assert.True(XNode.DeepEquals(XElement.Parse("""<custom_field id="4"><value>123</value></custom_field>"""), XElement.Parse(output)));
    }

    [Fact]
    public void Xml_Should_Write_Multiple_Values()
    {
        var field = new MyAccountCustomField { Id = 5, Value = "a", Values = ["a", "b"] };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(field);

        Assert.True(XNode.DeepEquals(XElement.Parse("""<custom_field id="5"><value type="array"><value>a</value><value>b</value></value></custom_field>"""), XElement.Parse(output)));
    }

    [Fact]
    public void Json_Should_Write_Single_Value()
    {
        var field = new MyAccountCustomField { Id = 4, Value = "123" };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(field);

        Assert.True(JToken.DeepEquals(JObject.Parse("""{"id":4,"value":"123"}"""), JObject.Parse(output)));
    }

    [Fact]
    public void Json_Should_Write_Multiple_Values()
    {
        var field = new MyAccountCustomField { Id = 5, Value = "a", Values = ["a", "b"] };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(field);

        Assert.True(JToken.DeepEquals(JObject.Parse("""{"id":5,"value":["a","b"]}"""), JObject.Parse(output)));
    }

    [Fact]
    public void Json_Should_Write_Empty_Array_For_Multiple_Field_Without_Values()
    {
        var field = new MyAccountCustomField { Id = 6, Multiple = true, Values = [] };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(field);

        Assert.True(JToken.DeepEquals(JObject.Parse("""{"id":6,"value":[]}"""), JObject.Parse(output)));
    }

    [Fact]
    public void Json_Should_Keep_Date_Time_Like_Values_As_Sent()
    {
        const string json = """
                            {
                              "user": {
                                "id": 3,
                                "custom_fields": [
                                  { "id": 4, "name": "Since", "value": "2024-01-02T10:11:12Z" },
                                  { "id": 5, "name": "Dates", "multiple": true, "value": ["2024-01-02T10:11:12Z"] }
                                ],
                                "mail": "dlopper@somenet.foo"
                              }
                            }
                            """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Deserialize<MyAccount>(json);

        Assert.Equal("2024-01-02T10:11:12Z", output.CustomFields[0].Value);
        Assert.Equal(["2024-01-02T10:11:12Z"], output.CustomFields[1].Values);
        Assert.Equal("dlopper@somenet.foo", output.Email);
    }

    [Fact]
    public void Xml_Should_Read_Value_With_Non_Array_Type_As_Single_Value()
    {
        const string xml = """
                           <user>
                             <id>3</id>
                             <custom_fields type="array">
                               <custom_field id="4" name="Phone number">
                                 <value type="string">555-1234</value>
                               </custom_field>
                             </custom_fields>
                             <mail>dlopper@somenet.foo</mail>
                           </user>
                           """;

        var output = SerializerFactory.Create(SerializerKind.Xml).Deserialize<MyAccount>(xml);

        Assert.Equal("555-1234", output.CustomFields[0].Value);
        Assert.Null(output.CustomFields[0].Values);
        Assert.Equal("dlopper@somenet.foo", output.Email);
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> MyAccountCustomFieldsTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "user": {
                                    "id": 3,
                                    "firstname": "Dave",
                                    "custom_fields": [
                                      { "id": 4, "name": "Phone number", "value": "555-1234" },
                                      { "id": 5, "name": "Skills", "multiple": true, "value": ["a", "b"] },
                                      { "id": 6, "name": "Tags", "multiple": true, "value": [] },
                                      { "id": 7, "name": "Birthday", "value": "2024-01-02" }
                                    ],
                                    "mail": "dlopper@somenet.foo",
                                    "api_key": "c308a59c9dea95920b13522fb3e0fb7fae4f292d"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <user>
                                 <id>3</id>
                                 <firstname>Dave</firstname>
                                 <custom_fields type="array">
                                   <custom_field id="4" name="Phone number">
                                     <value>555-1234</value>
                                   </custom_field>
                                   <custom_field id="5" name="Skills" multiple="true">
                                     <value type="array">
                                       <value>a</value>
                                       <value>b</value>
                                     </value>
                                   </custom_field>
                                   <custom_field id="6" name="Tags" multiple="true">
                                     <value type="array"/>
                                   </custom_field>
                                   <custom_field id="7" name="Birthday">
                                     <value>2024-01-02</value>
                                   </custom_field>
                                 </custom_fields>
                                 <mail>dlopper@somenet.foo</mail>
                                 <api_key>c308a59c9dea95920b13522fb3e0fb7fae4f292d</api_key>
                               </user>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
