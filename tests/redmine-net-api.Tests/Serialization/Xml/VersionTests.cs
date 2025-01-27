using System;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class VersionTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Version()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Version>(input);
        
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

    [Fact]
    public void Should_Deserialize_Versions()
    {
        const string input = """
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
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Version>(input);
        
        Assert.NotNull(output);
        Assert.Equal(34, output.TotalItems);

        var versions = output.Items.ToList();
        Assert.Equal(1, versions[0].Id);
        Assert.Equal("Redmine", versions[0].Project.Name);
        Assert.Equal(1, versions[0].Project.Id);
        Assert.Equal("0.7", versions[0].Name);
        Assert.Equal(VersionStatus.Closed, versions[0].Status);
        Assert.Equal(new DateTime(2008, 4, 28), versions[0].DueDate);
        Assert.Equal(VersionSharing.None, versions[0].Sharing);
        Assert.Equal("FooBarWikiPage", versions[0].WikiPageTitle);
        Assert.Equal(new DateTime(2008, 3, 9, 12, 52, 6, DateTimeKind.Local).AddHours(1), versions[0].CreatedOn);
        Assert.Equal(new DateTime(2009, 11, 15, 12, 22, 12, DateTimeKind.Local).AddHours(1), versions[0].UpdatedOn);

        Assert.Equal(2, versions[1].Id);
        Assert.Equal("Redmine", versions[1].Project.Name);
        Assert.Equal(1, versions[1].Project.Id);
        Assert.Equal("0.8", versions[1].Name);
        Assert.Equal(VersionStatus.Closed, versions[1].Status);
        Assert.Equal(new DateTime(2008, 12, 30), versions[1].DueDate);
        Assert.Equal(VersionSharing.None, versions[1].Sharing);
        Assert.Equal(new DateTime(2008, 3, 9, 12, 52, 12, DateTimeKind.Local).AddHours(1), versions[1].CreatedOn);
        Assert.Equal(new DateTime(2009, 11, 15, 12, 22, 12, DateTimeKind.Local).AddHours(1), versions[1].UpdatedOn);

    }
}
                             
                                  