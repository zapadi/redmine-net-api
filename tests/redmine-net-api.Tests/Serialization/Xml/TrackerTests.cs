using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class TrackerTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Tracker()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <tracker>
            <id>1</id>
            <name>Defect</name>
            <default_status id="1" name="New"/>
            <description>Description for Bug tracker</description>
        </tracker>
        """;
        
        var output = fixture.Serializer.Deserialize<Tracker>(input);
        
        Assert.NotNull(output);

        Assert.Equal(1, output.Id);
        Assert.Equal("Defect", output.Name);
        Assert.Equal("New", output.DefaultStatus.Name);
        Assert.Equal("Description for Bug tracker", output.Description);
    }
    
    [Fact]
    public void Should_Deserialize_Tracker_With_Enumerations()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.Deserialize<Tracker>(input);
        
        Assert.NotNull(output);

        Assert.Equal(1, output.Id);
        Assert.Equal("Defect", output.Name);
        Assert.Equal("New", output.DefaultStatus.Name);
        Assert.Equal("Description for Bug tracker", output.Description);
        Assert.Equal(9, output.EnabledStandardFields.Count);
    }
    
    [Fact]
    public void Should_Deserialize_Trackers()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.DeserializeToPagedResults<Tracker>(input);
        
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
}
