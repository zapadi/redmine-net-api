using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class GroupTests
{
    [Fact]
    public void Should_Serialize_Json_Group_User_Ids_As_Array()
    {
        var group = new Group
        {
            Name = "Developers",
            Users = [new GroupUser { Id = 5 }, new GroupUser { Id = 8 }]
        };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(group);

        var userIds = JObject.Parse(output)["group"]!["user_ids"];
        Assert.Equal(JTokenType.Array, userIds!.Type);
        Assert.Equal([5, 8], userIds.Values<int>());
    }

    [Fact]
    public void Should_Omit_Json_Group_User_Ids_When_Empty()
    {
        var group = new Group { Name = "Developers", Users = [] };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(group);

        Assert.Null(JObject.Parse(output)["group"]!["user_ids"]);
    }

    [Theory]
    [MemberData(nameof(GroupWithCustomFieldsSerializeTheoryData))]
    public void Should_Serialize_Group_With_Custom_Fields(string expected, SerializerKind kind)
    {
        var group = new Group
        {
            Name = "Developers",
            CustomFields = [IssueCustomField.CreateSingle(3, "Team", "core")]
        };

        var output = SerializerFactory.Create(kind).Serialize(group);

        Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
    }

    [Theory]
    [MemberData(nameof(GroupWithEmptyCustomFieldsSerializeTheoryData))]
    public void Should_Serialize_Group_Without_Custom_Fields_When_Empty(string expected, SerializerKind kind)
    {
        var group = new Group { Name = "Developers", CustomFields = [] };

        var output = SerializerFactory.Create(kind).Serialize(group);

        Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
    }

    [Theory]
    [MemberData(nameof(GroupDeserializeTheoryData))]
    public void Should_Deserialize_Group(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Group>(input);
        Assert.NotNull(output);
        Assert.Equal(20, output.Id);
        Assert.Equal("Developers", output.Name);
        Assert.NotNull(output.Users);
        Assert.Equal(2, output.Users.Count);
        Assert.Equal("John Smith", output.Users[0].Name);
        Assert.Equal("Dave Loper", output.Users[1].Name);
        Assert.Equal(5, output.Users[0].Id);
        Assert.Equal(8, output.Users[1].Id);
    }
    
    [Theory]
    [MemberData(nameof(GroupsDeserializeTheoryData))]
    public void Should_Deserialize_Groups(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Group>(input);
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var groups = output.Items.ToList();
        Assert.Equal(53, groups[0].Id);
        Assert.Equal("Managers", groups[0].Name);

        Assert.Equal(55, groups[1].Id);
        Assert.Equal("Developers", groups[1].Name);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> GroupsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "groups": [
                                    {
                                      "id": 53,
                                      "name": "Managers"
                                    },
                                    {
                                      "id": 55,
                                      "name": "Developers"
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <groups type="array">
                                   <group>
                                       <id>53</id>
                                       <name>Managers</name>
                                   </group>
                                   <group>
                                       <id>55</id>
                                       <name>Developers</name>
                                   </group>
                               </groups>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> GroupDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "group": {
                                    "id": 20,
                                    "name": "Developers",
                                    "users": [
                                      {
                                        "id": 5,
                                        "name": "John Smith"
                                      },
                                      {
                                        "id": 8,
                                        "name": "Dave Loper"
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <group>
                                   <id>20</id>
                                   <name>Developers</name>
                                   <users type="array">
                                       <user id="5" name="John Smith"/>
                                       <user id="8" name="Dave Loper"/>
                                   </users>
                               </group>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> GroupWithCustomFieldsSerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "group": {
                                    "name": "Developers",
                                    "custom_fields": [
                                      {
                                        "id": 3,
                                        "name": "Team",
                                        "multiple": "false",
                                        "value": "core"
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <group>
                               <name>Developers</name>
                               <custom_fields type="array">
                               <custom_field id="3">
                               <multiple>false</multiple>
                               <value>core</value>
                               </custom_field>
                               </custom_fields>
                               </group>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> GroupWithEmptyCustomFieldsSerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "group": {
                                    "name": "Developers"
                                  }
                                }
                                """;

            const string xml = """
                               <group>
                               <name>Developers</name>
                               </group>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
