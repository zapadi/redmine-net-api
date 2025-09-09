using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class TrackerTests()
{
    [Theory]
    [MemberData(nameof(TrackerDeserializeTheoryData))]
    public void Should_Deserialize_Tracker(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Tracker>(input);
        
        Assert.NotNull(output);

        Assert.Equal(1, output.Id);
        Assert.Equal("Defect", output.Name);
        Assert.Equal("New", output.DefaultStatus.Name);
        Assert.Equal("Description for Bug tracker", output.Description);
    }
    
    [Theory]
    [MemberData(nameof(TrakerWithEnumerationDeserializeTheoryData))]
    public void Should_Deserialize_Tracker_With_Enumerations(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Tracker>(input);
        
        Assert.NotNull(output);

        Assert.Equal(1, output.Id);
        Assert.Equal("Defect", output.Name);
        Assert.Equal("New", output.DefaultStatus.Name);
        Assert.Equal("Description for Bug tracker", output.Description);
        Assert.Equal(9, output.EnabledStandardFields.Count);
    }
    
    [Theory]
    [MemberData(nameof(TrakersDeserializeTheoryData))]
    public void Should_Deserialize_Trackers(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Tracker>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var trackers = output.Items.ToList();
        Assert.Equal(2, trackers.Count);

        Assert.Equal(1, trackers[0].Id);
        Assert.Equal("Defect", trackers[0].Name);
        Assert.Equal("New", trackers[0].DefaultStatus.Name);
        Assert.Equal("Description for Bug tracker", trackers[0].Description);
        Assert.Equal(9, trackers[0].EnabledStandardFields.Count);

        Assert.Equal(2, trackers[1].Id);
        Assert.Equal("Feature", trackers[1].Name);
        Assert.Equal("New", trackers[1].DefaultStatus.Name);
        Assert.Equal("Description for Feature request tracker", trackers[1].Description);
        Assert.Equal(9, trackers[1].EnabledStandardFields.Count);

    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TrackerDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "tracker": {
                                    "id": 1,
                                    "name": "Defect",
                                    "default_status": {
                                      "id": 1,
                                      "name": "New"
                                    },
                                    "description": "Description for Bug tracker"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <tracker>
                                   <id>1</id>
                                   <name>Defect</name>
                                   <default_status id="1" name="New"/>
                                   <description>Description for Bug tracker</description>
                               </tracker>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TrakersDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "trackers": [
                                    {
                                      "id": 1,
                                      "name": "Defect",
                                      "default_status": {
                                        "id": 1,
                                        "name": "New"
                                      },
                                      "description": "Description for Bug tracker",
                                      "enabled_standard_fields": [
                                        "assigned_to_id",
                                        "category_id",
                                        "fixed_version_id",
                                        "parent_issue_id",
                                        "start_date",
                                        "due_date",
                                        "estimated_hours",
                                        "done_ratio",
                                        "description"
                                      ]
                                    },
                                    {
                                      "id": 2,
                                      "name": "Feature",
                                      "default_status": {
                                        "id": 1,
                                        "name": "New"
                                      },
                                      "description": "Description for Feature request tracker",
                                      "enabled_standard_fields": [
                                        "assigned_to_id",
                                        "category_id",
                                        "fixed_version_id",
                                        "parent_issue_id",
                                        "start_date",
                                        "due_date",
                                        "estimated_hours",
                                        "done_ratio",
                                        "description"
                                      ]
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <trackers>
                                   <tracker>
                                       <id>1</id>
                                       <name>Defect</name>
                                       <default_status id="1" name="New"/>
                                       <description>Description for Bug tracker</description>
                                       <enabled_standard_fields type="array">
                                           <field>assigned_to_id</field>
                                           <field>category_id</field>
                                           <field>fixed_version_id</field>
                                           <field>parent_issue_id</field>
                                           <field>start_date</field>
                                           <field>due_date</field>
                                           <field>estimated_hours</field>
                                           <field>done_ratio</field>
                                           <field>description</field>
                                       </enabled_standard_fields>
                                   </tracker>
                                   <tracker>
                                       <id>2</id>
                                       <name>Feature</name>
                                       <default_status id="1" name="New"/>
                                       <description>Description for Feature request tracker</description>
                                       <enabled_standard_fields type="array">
                                           <field>assigned_to_id</field>
                                           <field>category_id</field>
                                           <field>fixed_version_id</field>
                                           <field>parent_issue_id</field>
                                           <field>start_date</field>
                                           <field>due_date</field>
                                           <field>estimated_hours</field>
                                           <field>done_ratio</field>
                                           <field>description</field>
                                       </enabled_standard_fields>
                                   </tracker>
                               </trackers>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TrakerWithEnumerationDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "tracker": {
                                    "id": 1,
                                    "name": "Defect",
                                    "default_status": {
                                      "id": 1,
                                      "name": "New"
                                    },
                                    "description": "Description for Bug tracker",
                                    "enabled_standard_fields": [
                                      "assigned_to_id",
                                      "category_id",
                                      "fixed_version_id",
                                      "parent_issue_id",
                                      "start_date",
                                      "due_date",
                                      "estimated_hours",
                                      "done_ratio",
                                      "description"
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <tracker>
                                   <id>1</id>
                                   <name>Defect</name>
                                   <default_status id="1" name="New"/>
                                   <description>Description for Bug tracker</description>
                                   <enabled_standard_fields>
                                       <field>assigned_to_id</field>
                                       <field>category_id</field>
                                       <field>fixed_version_id</field>
                                       <field>parent_issue_id</field>
                                       <field>start_date</field>
                                       <field>due_date</field>
                                       <field>estimated_hours</field>
                                       <field>done_ratio</field>
                                       <field>description</field>
                                   </enabled_standard_fields>
                               </tracker>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
