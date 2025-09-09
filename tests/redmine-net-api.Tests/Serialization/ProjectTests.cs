using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class ProjectTests()
{
    [Theory]
    [MemberData(nameof(ProjectDeserializeTheoryData))]
    public void Should_Deserialize_Project(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Project>(input);
        
        Assert.NotNull(output);

        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Name);
        Assert.Equal("redmine", output.Identifier);
        Assert.Contains("Redmine is a flexible project management web application", output.Description);
        AssertDateTime.Equal("2007-9-29T10:03:04", output.CreatedOn);
        AssertDateTime.Equal("2009-3-15T11:35:11", output.UpdatedOn);
        Assert.True(output.IsPublic);
    }
    
    [Theory]
    [MemberData(nameof(ProjectWithIncludeDeserializeTheoryData))]
    public void Should_Deserialize_Project_With_Includes(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Project>(input);
        
        Assert.NotNull(output);

        Assert.Equal(45, output.Id);
        Assert.Equal("cpIFOkw", output.Name);
        Assert.Equal("cpifokw", output.Identifier);
        Assert.Equal("pgRgERQ", output.Description);
        AssertDateTime.Equal("2025-9-11T15:25:46", output.CreatedOn);
        AssertDateTime.Equal("2025-9-11T15:25:46", output.UpdatedOn);
        Assert.True(output.IsPublic);
    }
    
    [Theory]
    [MemberData(nameof(ProjectsDeserializeTheoryData))]
    public void Should_Deserialize_Projects(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Project>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var projects = output.Items.ToList();
        Assert.Equal(1, projects[0].Id);
        Assert.Equal("Redmine", projects[0].Name);
        Assert.Equal("redmine", projects[0].Identifier);
        Assert.Contains("Redmine is a flexible project management web application", projects[0].Description);
        AssertDateTime.Equal("2007-9-29T10:03:04", projects[0].CreatedOn);
        AssertDateTime.Equal("2009-3-15T11:35:11", projects[0].UpdatedOn);
        Assert.True(projects[0].IsPublic);

        Assert.Equal(2, projects[1].Id);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "project": {
                                    "id": 1,
                                    "name": "Redmine",
                                    "identifier": "redmine",
                                    "description": "Redmine is a flexible project management web application written using Ruby on Rails framework.",
                                    "homepage": null,
                                    "status": 1,
                                    "parent": {
                                      "id": 123,
                                      "name": "foo"
                                    },
                                    "default_version": {
                                      "id": 3,
                                      "name": "2.0"
                                    },
                                    "default_assignee": {
                                      "id": 2,
                                      "name": "John Smith"
                                    },
                                    "created_on": "2007-09-29T12:03:04+02:00",
                                    "updated_on": "2009-03-15T12:35:11+01:00",
                                    "is_public": true
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <project>
                                   <id>1</id>
                                   <name>Redmine</name>
                                   <identifier>redmine</identifier>
                                   <description>
                                    Redmine is a flexible project management web application written using Ruby on Rails framework.
                                   </description>
                                   <homepage></homepage>
                                   <status>1</status>
                                   <parent id="123" name="foo"/>
                                   <default_version id="3" name="2.0"/>
                                   <default_assignee id="2" name="John Smith"/>
                                   <created_on>2007-09-29T12:03:04+02:00</created_on>
                                   <updated_on>2009-03-15T12:35:11+01:00</updated_on>
                                   <is_public>true</is_public>
                               </project>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "projects": [
                                    {
                                      "id": 1,
                                      "name": "Redmine",
                                      "identifier": "redmine",
                                      "description": "Redmine is a flexible project management web application written using Ruby on Rails framework.",
                                      "created_on": "2007-09-29T12:03:04+02:00",
                                      "updated_on": "2009-03-15T12:35:11+01:00",
                                      "is_public": true
                                    },
                                    {
                                      "id": 2
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <projects type="array">
                                   <project>
                                       <id>1</id>
                                       <name>Redmine</name>
                                       <identifier>redmine</identifier>
                                       <description>
                                        Redmine is a flexible project management web application written using Ruby on Rails framework.
                                       </description>
                                       <created_on>2007-09-29T12:03:04+02:00</created_on>
                                       <updated_on>2009-03-15T12:35:11+01:00</updated_on>
                                       <is_public>true</is_public>
                                   </project>
                                   <project>
                                       <id>2</id>
                                   </project>
                               </projects>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectWithIncludeDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "project": {
                                    "id": 45,
                                    "name": "cpIFOkw",
                                    "identifier": "cpifokw",
                                    "description": "pgRgERQ",
                                    "homepage": "ZiGXoKy",
                                    "status": 1,
                                    "is_public": true,
                                    "inherit_members": true,
                                    "custom_fields": [
                                      {
                                        "id": 2,
                                        "name": "ProjCustomField",
                                        "value": "1"
                                      }
                                    ],
                                    "trackers": [
                                      { "id": 1, "name": "Bug" },
                                      { "id": 2, "name": "Feature" },
                                      { "id": 3, "name": "Support" }
                                    ],
                                    "issue_categories": [],
                                    "time_entry_activities": [
                                      { "id": 8, "name": "Design" },
                                      { "id": 9, "name": "Development" }
                                    ],
                                    "enabled_modules": [
                                      { "id": 370, "name": "issue_tracking" },
                                      { "id": 371, "name": "time_tracking" },
                                      { "id": 372, "name": "news" },
                                      { "id": 373, "name": "documents" },
                                      { "id": 374, "name": "files" },
                                      { "id": 375, "name": "wiki" },
                                      { "id": 376, "name": "repository" },
                                      { "id": 377, "name": "boards" },
                                      { "id": 378, "name": "calendar" }
                                    ],
                                    "issue_custom_fields": [
                                      { "id": 1, "name": "IssueCustonField1" }
                                    ],
                                    "created_on": "2025-09-11T15:25:46Z",
                                    "updated_on": "2025-09-11T15:25:46Z"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8" standalone="no"?>
                               <project>
                                   <id>45</id>
                                   <name>cpIFOkw</name>
                                   <identifier>cpifokw</identifier>
                                   <description>pgRgERQ</description>
                                   <homepage>ZiGXoKy</homepage>
                                   <status>1</status>
                                   <is_public>true</is_public>
                                   <inherit_members>true</inherit_members>
                                   <custom_fields type="array">
                                       <custom_field id="2" name="ProjCustomField">
                                           <value>1</value>
                                       </custom_field>
                                   </custom_fields>
                                   <trackers type="array">
                                       <tracker id="1" name="Bug"/>
                                       <tracker id="2" name="Feature"/>
                                       <tracker id="3" name="Support"/>
                                   </trackers>
                                   <issue_categories type="array"/>
                                   <time_entry_activities type="array">
                                       <time_entry_activity id="8" name="Design"/>
                                       <time_entry_activity id="9" name="Development"/>
                                   </time_entry_activities>
                                   <enabled_modules type="array">
                                       <enabled_module id="370" name="issue_tracking"/>
                                       <enabled_module id="371" name="time_tracking"/>
                                       <enabled_module id="372" name="news"/>
                                       <enabled_module id="373" name="documents"/>
                                       <enabled_module id="374" name="files"/>
                                       <enabled_module id="375" name="wiki"/>
                                       <enabled_module id="376" name="repository"/>
                                       <enabled_module id="377" name="boards"/>
                                       <enabled_module id="378" name="calendar"/>
                                   </enabled_modules>
                                   <issue_custom_fields type="array">
                                       <custom_field id="1" name="IssueCustonField1"/>
                                   </issue_custom_fields>
                                   <created_on>2025-09-11T15:25:46Z</created_on>
                                   <updated_on>2025-09-11T15:25:46Z</updated_on>
                               </project>
                               
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
