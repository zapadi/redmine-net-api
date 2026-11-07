using System;
using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class ProjectTests
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

    [Theory]
    [MemberData(nameof(ProjectSerializeTheoryData))]
    public void Should_Serialize_Project(string expected, SerializerKind kind)
    {
        var project = new Project
        {
            Id = 1,
            Name = "Redmine",
            Identifier = "redmine",
            Description = "Redmine",
            CreatedOn = DateTime.Parse("2025-9-11T15:25:46"),
            IsPublic = true,
            UpdatedOn = DateTime.Parse("2025-9-11T15:25:46"),
            Status = ProjectStatus.Archived,
            Parent = 2.ToIdentifier(),
            InheritMembers = true,
            HomePage = "www.redmine.com",
            DefaultAssignee = 3.ToIdentifier(),
            DefaultVersion = 4.ToIdentifier(),
            Trackers = [new ProjectTracker(1, "Tracker1"), new ProjectTracker(2, "Tracker2")],
            EnabledModules = [new ProjectEnabledModule("module1"), new ProjectEnabledModule("module2"), new ProjectEnabledModule("module3")],
            TimeEntryActivities = [new ProjectTimeEntryActivity(90, "prjTimeEntryActivity")],
            IssueCategories = [new ProjectIssueCategory(123, "prjIssueCategory"), new ProjectIssueCategory(234, "prjIssueCategory2")],
            IssueCustomFields = [IssueCustomField.CreateSingle(500, "icf500", "val500"), IssueCustomField.CreateMultiple(501, "icf501", ["val501", "501val"])],
#pragma warning disable CS0618
            CustomFieldValues =
            [
                new CustomField()
                {
                    Id = 789,
                    PossibleValues = [new CustomFieldPossibleValue("val789")]
                },
                new CustomField()
                {
                    Id = 790,
                    PossibleValues = [new CustomFieldPossibleValue("val790"), new CustomFieldPossibleValue("790val")]
                }
            ]
#pragma warning restore CS0618
        };
        
        var serializer = SerializerFactory.Create(kind);

        var output = serializer.Serialize(project);

        Assert.Equal(expected, output);
    }

    [Theory]
    [MemberData(nameof(ProjectCustomFieldsSerializeTheoryData))]
    public void Should_Serialize_Project_With_ProjectCustomFields(string expected, SerializerKind kind)
    {
        var project = new Project
        {
            Name = "Redmine",
            Identifier = "redmine",
            ProjectCustomFields = [IssueCustomField.CreateSingle(1, "PcfString", "hello"), IssueCustomField.CreateMultiple(2, "PcfList", ["a", "c"])]
        };

        var serializer = SerializerFactory.Create(kind);

        var output = serializer.Serialize(project);

        Assert.Equal(expected, output);
    }

    [Theory]
    [MemberData(nameof(ProjectWithCustomFieldValuesDeserializeTheoryData))]
    public void Should_Deserialize_Project_ProjectCustomFields(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<Project>(input);

        Assert.NotNull(output);
        Assert.Equal(2, output.ProjectCustomFields.Count);

        Assert.Equal(1, output.ProjectCustomFields[0].Id);
        Assert.Equal("PcfString", output.ProjectCustomFields[0].Name);
        Assert.Equal(["hello"], output.ProjectCustomFields[0].Values.Select(v => v.Info));

        Assert.Equal(2, output.ProjectCustomFields[1].Id);
        Assert.Equal("PcfList", output.ProjectCustomFields[1].Name);
        Assert.True(output.ProjectCustomFields[1].Multiple);
        Assert.Equal(["a", "c"], output.ProjectCustomFields[1].Values.Select(v => v.Info));

#pragma warning disable CS0618
        Assert.Equal(2, output.CustomFieldValues.Count);
        Assert.Equal(1, output.CustomFieldValues[0].Id);
        Assert.Equal("PcfString", output.CustomFieldValues[0].Name);
        Assert.Equal(2, output.CustomFieldValues[1].Id);
        Assert.Equal("PcfList", output.CustomFieldValues[1].Name);
#pragma warning restore CS0618
    }

    [Theory]
    [MemberData(nameof(ProjectCustomFieldsSerializeTheoryData))]
    public void Should_Serialize_ProjectCustomFields_Instead_Of_CustomFieldValues_When_Both_Are_Set(string expected, SerializerKind kind)
    {
        var project = new Project
        {
            Name = "Redmine",
            Identifier = "redmine",
            ProjectCustomFields = [IssueCustomField.CreateSingle(1, "PcfString", "hello"), IssueCustomField.CreateMultiple(2, "PcfList", ["a", "c"])],
#pragma warning disable CS0618
            CustomFieldValues = [new CustomField { Id = 99, Name = "legacy" }]
#pragma warning restore CS0618
        };

        var serializer = SerializerFactory.Create(kind);

        var output = serializer.Serialize(project);

        Assert.Equal(expected, output);
    }

    [Theory]
    [MemberData(nameof(ProjectWithEmptyCustomFieldValuesDeserializeTheoryData))]
    public void Should_Deserialize_Empty_ProjectCustomFields_And_Write_Xml_With_Ids(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<Project>(input);

        Assert.Equal(2, output.ProjectCustomFields.Count);
        Assert.All(output.ProjectCustomFields, cf => Assert.True(cf.Values == null || cf.Values.Count == 0));

        var xml = SerializerFactory.Create(SerializerKind.Xml).Serialize(output);

        Assert.DoesNotContain("<custom_field />", xml);
        Assert.Contains("<custom_field id=\"1\"", xml);
        Assert.Contains("<custom_field id=\"2\"", xml);
    }

    [Fact]
    public void Should_Serialize_Xml_Custom_Field_Without_Values_With_Id_Only()
    {
        var project = new Project
        {
            Name = "Redmine",
            Identifier = "redmine",
            ProjectCustomFields = [IssueCustomField.CreateMultiple(2, "PcfList", [])]
        };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(project);

        Assert.Equal("""
                     <project>
                     <name>Redmine</name>
                     <identifier>redmine</identifier>
                     <inherit_members>false</inherit_members>
                     <is_public>false</is_public>
                     <custom_fields type="array">
                     <custom_field id="2" />
                     </custom_fields>
                     </project>
                     """, output);
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

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectWithCustomFieldValuesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {"project":{"id":3,"name":"pcf","identifier":"pcf","description":null,"homepage":"","status":1,"is_public":false,"inherit_members":false,"custom_fields":[{"id":1,"name":"PcfString","value":"hello"},{"id":2,"name":"PcfList","multiple":true,"value":["a","c"]}],"created_on":"2026-10-17T17:50:15Z","updated_on":"2026-10-17T17:50:15Z"}}
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?><project><id>3</id><name>pcf</name><identifier>pcf</identifier><description/><homepage></homepage><status>1</status><is_public>false</is_public><inherit_members>false</inherit_members><custom_fields type="array"><custom_field id="1" name="PcfString"><value>hello</value></custom_field><custom_field id="2" name="PcfList" multiple="true"><value type="array"><value>a</value><value>c</value></value></custom_field></custom_fields><created_on>2026-10-17T17:50:15Z</created_on><updated_on>2026-10-17T17:50:15Z</updated_on></project>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectWithEmptyCustomFieldValuesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {"project":{"id":3,"name":"pcf","identifier":"pcf","custom_fields":[{"id":1,"name":"PcfString","value":null},{"id":2,"name":"PcfList","multiple":true,"value":[]}]}}
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?><project><id>3</id><name>pcf</name><identifier>pcf</identifier><custom_fields type="array"><custom_field id="1" name="PcfString"><value/></custom_field><custom_field id="2" name="PcfList" multiple="true"><value type="array"></value></custom_field></custom_fields></project>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectCustomFieldsSerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "project": {
                                    "name": "Redmine",
                                    "identifier": "redmine",
                                    "inherit_members": "false",
                                    "is_public": "false",
                                    "custom_fields": [
                                      {
                                        "id": 1,
                                        "name": "PcfString",
                                        "multiple": "false",
                                        "value": "hello"
                                      },
                                      {
                                        "id": 2,
                                        "name": "PcfList",
                                        "multiple": "true",
                                        "value": [
                                          "a",
                                          "c"
                                        ]
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <project>
                               <name>Redmine</name>
                               <identifier>redmine</identifier>
                               <inherit_members>false</inherit_members>
                               <is_public>false</is_public>
                               <custom_fields type="array">
                               <custom_field id="1">
                               <multiple>false</multiple>
                               <value>hello</value>
                               </custom_field>
                               <custom_field id="2">
                               <multiple>true</multiple>
                               <value type="array">
                               <value>a</value>
                               <value>c</value>
                               </value>
                               </custom_field>
                               </custom_fields>
                               </project>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ProjectSerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "project": {
                                    "name": "Redmine",
                                    "identifier": "redmine",
                                    "description": "Redmine",
                                    "homepage": "www.redmine.com",
                                    "inherit_members": "true",
                                    "is_public": "true",
                                    "parent_id": 2,
                                    "default_assigned_to_id": 3,
                                    "default_version_id": 4,
                                    "tracker_ids": [
                                      1,
                                      2
                                    ],
                                    "enabled_module_names": [
                                      "module1",
                                      "module2",
                                      "module3"
                                    ],
                                    "custom_field_values": {
                                      "789": "val789",
                                      "790": [
                                        "val790",
                                        "790val"
                                      ]
                                    }
                                  }
                                }
                                """;

            const string xml = """
                               <project>
                               <name>Redmine</name>
                               <identifier>redmine</identifier>
                               <description>Redmine</description>
                               <homepage>www.redmine.com</homepage>
                               <inherit_members>true</inherit_members>
                               <is_public>true</is_public>
                               <parent_id>2</parent_id>
                               <default_assigned_to_id>3</default_assigned_to_id>
                               <default_version_id>4</default_version_id>
                               <tracker_ids type="array">
                               <tracker_id>1</tracker_id>
                               <tracker_id>2</tracker_id>
                               </tracker_ids>
                               <enabled_module_names type="array">
                               <enabled_module_name>module1</enabled_module_name>
                               <enabled_module_name>module2</enabled_module_name>
                               <enabled_module_name>module3</enabled_module_name>
                               </enabled_module_names>
                               <custom_fields type="array">
                               <custom_field id="789">
                               <value>val789</value>
                               </custom_field>
                               <custom_field id="790">
                               <value type="array">
                               <value>val790</value>
                               <value>790val</value>
                               </value>
                               </custom_field>
                               </custom_fields>
                               </project>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}