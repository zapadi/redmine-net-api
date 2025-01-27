using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class EnumerationTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Issue_Priorities()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <issue_priorities type="array">
            <issue_priority>
                <id>3</id>
                <name>Low</name>
                <is_default>false</is_default>
            </issue_priority>
            <issue_priority>
                <id>4</id>
                <name>Normal</name>
                <is_default>true</is_default>
            </issue_priority>
        </issue_priorities>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<IssuePriority>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var issuePriorities = output.Items.ToList();
        Assert.Equal(2, issuePriorities.Count);

        Assert.Equal(3, issuePriorities[0].Id);
        Assert.Equal("Low", issuePriorities[0].Name);
        Assert.False(issuePriorities[0].IsDefault);

        Assert.Equal(4, issuePriorities[1].Id);
        Assert.Equal("Normal", issuePriorities[1].Name);
        Assert.True(issuePriorities[1].IsDefault);
    }
    
    [Fact]
    public void Should_Deserialize_TimeEntry_Activities()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <time_entry_activities type="array">
            <time_entry_activity>
                <id>8</id>
                <name>Design</name>
                <is_default>false</is_default>
            </time_entry_activity>
        </time_entry_activities>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<TimeEntryActivity>(input);
        
        Assert.NotNull(output);
        Assert.Single(output.Items);

        var timeEntryActivities = output.Items.ToList();
        Assert.Equal(8, timeEntryActivities[0].Id);
        Assert.Equal("Design", timeEntryActivities[0].Name);
        Assert.False(timeEntryActivities[0].IsDefault);
    }
    
    [Fact]
    public void Should_Deserialize_Document_Categories()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <document_categories type="array">
            <document_category>
                <id>1</id>
                <name>Uncategorized</name>
                <is_default>false</is_default>
            </document_category>
            <document_category>
                <id>2</id>
                <name>User documentation</name>
                <is_default>false</is_default>
            </document_category>
            <document_category>
                <id>3</id>
                <name>Technical documentation</name>
                <is_default>false</is_default>
            </document_category>
        </document_categories>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<DocumentCategory>(input);
        
        Assert.NotNull(output);
        Assert.Equal(3, output.TotalItems);

        var documentCategories = output.Items.ToList();
        Assert.Equal(3, documentCategories.Count);

        Assert.Equal(1, documentCategories[0].Id);
        Assert.Equal("Uncategorized", documentCategories[0].Name);
        Assert.False(documentCategories[0].IsDefault);

        Assert.Equal(2, documentCategories[1].Id);
        Assert.Equal("User documentation", documentCategories[1].Name);
        Assert.False(documentCategories[1].IsDefault);

        Assert.Equal(3, documentCategories[2].Id);
        Assert.Equal("Technical documentation", documentCategories[2].Name);
        Assert.False(documentCategories[2].IsDefault);
    }
}