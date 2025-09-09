using System.Collections.Generic;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public sealed class MembershipTests()
{
    [Theory]
    [MemberData(nameof(MembershipsDeserializeTheoryData))]
    public void Should_Deserialize_Memberships(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.DeserializeToPagedResults<Membership>(input);
    }

    [Theory]
    [MemberData(nameof(MembershipDeserializeTheoryData))]
    public void Should_Deserialize_Membership(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Membership>(input);
        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("David Robert", output.User.Name);
        Assert.Equal(17, output.User.Id);
    }
    
    [Theory]
    [MemberData(nameof(MembershipWithRolesDeserializeTheoryData))]
    public void Should_Deserialize_Membership_With_Roles(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Membership>(input);
        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("David Robert", output.User.Name);
        Assert.Equal(17, output.User.Id);
        Assert.NotNull(output.Roles);
        Assert.Single(output.Roles);
        Assert.Equal("Manager", output.Roles[0].Name);
        Assert.Equal(1, output.Roles[0].Id);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> MembershipDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "membership": {
                                    "id": 1,
                                    "project": {
                                      "id": 1,
                                      "name": "Redmine"
                                    },
                                    "user": {
                                      "id": 17,
                                      "name": "David Robert"
                                    }
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <membership>
                                   <id>1</id>
                                   <project name="Redmine" id="1"/>
                                   <user name="David Robert" id="17"/>
                               </membership>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> MembershipsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "memberships": [
                                    {
                                      "id": 1,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "user": {
                                        "id": 17,
                                        "name": "David Robert"
                                      },
                                      "roles": [
                                        {
                                          "id": 1,
                                          "name": "Manager"
                                        }
                                      ]
                                    },
                                    {
                                      "id": 3,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "group": {
                                        "id": 24,
                                        "name": "Contributors"
                                      },
                                      "roles": [
                                        {
                                          "id": 3,
                                          "name": "Contributor"
                                        }
                                      ]
                                    },
                                    {
                                      "id": 4,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "user": {
                                        "id": 27,
                                        "name": "John Smith"
                                      },
                                      "roles": [
                                        {
                                          "id": 2,
                                          "name": "Developer"
                                        },
                                        {
                                          "id": 3,
                                          "name": "Contributor",
                                          "inherited": true
                                        }
                                      ]
                                    }
                                  ],
                                  "limit": 25,
                                  "offset": 0,
                                  "total_count": 3
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <memberships type="array" limit="25" offset="0" total_count="3">
                                   <membership>
                                       <id>1</id>
                                       <project name="Redmine" id="1"/>
                                       <user name="David Robert" id="17"/>
                                       <roles type="array">
                                           <role name="Manager" id="1"/>
                                       </roles>
                                   </membership>
                                   <membership>
                                       <id>3</id>
                                       <project name="Redmine" id="1"/>
                                       <group name="Contributors" id="24"/>
                                       <roles type="array">
                                           <role name="Contributor" id="3"/>
                                       </roles>
                                   </membership>
                                   <membership>
                                       <id>4</id>
                                       <project name="Redmine" id="1"/>
                                       <user name="John Smith" id="27"/>
                                       <roles type="array">
                                           <role name="Developer" id="2" />
                                       <role name="Contributor" id="3" inherited="true" />
                                   </roles>
                                   </membership>
                               </memberships>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> MembershipWithRolesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "membership": {
                                    "id": 1,
                                    "project": {
                                      "id": 1,
                                      "name": "Redmine"
                                    },
                                    "user": {
                                      "id": 17,
                                      "name": "David Robert"
                                    },
                                    "roles": [
                                      {
                                        "id": 1,
                                        "name": "Manager"
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <membership>
                                 <id>1</id>
                                 <project name="Redmine" id="1"/>
                                 <user name="David Robert" id="17"/>
                                 <roles type="array">
                                   <role name="Manager" id="1"/>
                                 </roles>
                               </membership>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}