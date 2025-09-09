using System;
using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class VersionTests()
{
    [Theory]
    [MemberData(nameof(VersionDeserializeTheoryData))]
    public void Should_Deserialize_Version(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Version>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("0.8", output.Name);
        Assert.Equal(VersionStatus.Closed, output.Status);
        Assert.Equal(new DateTime(2008, 12, 30), output.DueDate);
        Assert.Equal(0.0f, output.EstimatedHours);
        Assert.Equal(0.0f, output.SpentHours);
        Assert.Equal(new DateTime(2008, 3, 9, 12, 52, 12, DateTimeKind.Local).AddHours(1), output.CreatedOn);
        Assert.Equal(new DateTime(2009, 11, 15, 12, 22, 12, DateTimeKind.Local).AddHours(1), output.UpdatedOn);

    }

    [Theory]
    [MemberData(nameof(VersionsDeserializeTheoryData))]
    public void Should_Deserialize_Versions(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Version>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var versions = output.Items.ToList();
        Assert.Equal(1, versions[0].Id);
        Assert.Equal("Redmine", versions[0].Project.Name);
        Assert.Equal(1, versions[0].Project.Id);
        Assert.Equal("0.7", versions[0].Name);
        Assert.Equal(VersionStatus.Closed, versions[0].Status);
        AssertDateTime.Equal("2008-4-28", versions[0].DueDate, dateOnly: true);
        Assert.Equal(VersionSharing.None, versions[0].Sharing);
        Assert.Equal("FooBarWikiPage", versions[0].WikiPageTitle);
        // AssertDateTime.Equal("2008-3-9T12:52:06", versions[0].CreatedOn);
        // AssertDateTime.Equal("2009-11-15T12:22:12", versions[0].UpdatedOn);

        Assert.Equal(2, versions[1].Id);
        Assert.Equal("Redmine", versions[1].Project.Name);
        Assert.Equal(1, versions[1].Project.Id);
        Assert.Equal("0.8", versions[1].Name);
        Assert.Equal(VersionStatus.Closed, versions[1].Status);
        AssertDateTime.Equal("2008-12-30", versions[1].DueDate,  dateOnly: true);
        Assert.Equal(VersionSharing.None, versions[1].Sharing);
        // AssertDateTime.Equal("2008-3-9T12:52:12", versions[1].CreatedOn);
        // AssertDateTime.Equal("2009-11-15T12:22:12", versions[1].UpdatedOn);

    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> VersionDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "version": {
                                    "id": 2,
                                    "project": {
                                      "id": 1,
                                      "name": "Redmine"
                                    },
                                    "name": "0.8",
                                    "description": null,
                                    "status": "closed",
                                    "due_date": "2008-12-30",
                                    "estimated_hours": 0.0,
                                    "spent_hours": 0.0,
                                    "created_on": "2008-03-09T12:52:12+01:00",
                                    "updated_on": "2009-11-15T12:22:12+01:00"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <version>
                                   <id>2</id>
                                   <project name="Redmine" id="1"/>
                                   <name>0.8</name>
                                   <description/>
                                   <status>closed</status>
                                   <due_date>2008-12-30</due_date>
                                   <estimated_hours>0.0</estimated_hours>
                                   <spent_hours>0.0</spent_hours>
                                   <created_on>2008-03-09T12:52:12+01:00</created_on>
                                   <updated_on>2009-11-15T12:22:12+01:00</updated_on>
                               </version>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> VersionsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "versions": [
                                    {
                                      "id": 1,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "name": "0.7",
                                      "description": null,
                                      "status": "closed",
                                      "due_date": "2008-04-28",
                                      "sharing": "none",
                                      "created_on": "2008-03-09T12:52:06+01:00",
                                      "updated_on": "2009-11-15T12:22:12+01:00",
                                      "wiki_page_title": "FooBarWikiPage"
                                    },
                                    {
                                      "id": 2,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "name": "0.8",
                                      "description": null,
                                      "status": "closed",
                                      "due_date": "2008-12-30",
                                      "sharing": "none",
                                      "wiki_page_title": "FooBarWikiPage",
                                      "created_on": "2008-03-09T12:52:12+01:00",
                                      "updated_on": "2009-11-15T12:22:12+01:00"
                                    }
                                  ],
                                  "total_count": 34
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <versions type="array" total_count="34">
                                   <version>
                                       <id>1</id>
                                       <project name="Redmine" id="1"/>
                                       <name>0.7</name>
                                       <description/>
                                       <status>closed</status>
                                       <due_date>2008-04-28</due_date>
                                       <sharing>none</sharing>
                                       <created_on>2008-03-09T12:52:06+01:00</created_on>
                                       <updated_on>2009-11-15T12:22:12+01:00</updated_on>
                                       <wiki_page_title>FooBarWikiPage</wiki_page_title>
                                   </version>
                                   <version>
                                       <id>2</id>
                                       <project name="Redmine" id="1"/>
                                       <name>0.8</name>
                                       <description/>
                                       <status>closed</status>
                                       <due_date>2008-12-30</due_date>
                                       <sharing>none</sharing>
                                       <wiki_page_title>FooBarWikiPage</wiki_page_title>
                                       <created_on>2008-03-09T12:52:12+01:00</created_on>
                                       <updated_on>2009-11-15T12:22:12+01:00</updated_on>
                                   </version>
                               </versions> 
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
                             
                                  