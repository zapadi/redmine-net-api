using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class EnumerationTests()
{
    [Theory]
    [MemberData(nameof(IssuePrioritiesDeserializeTheoryData))]
    public void IssuePriorities_Should_Deserialize(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<IssuePriority>(input);
        
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
    
    [Theory]
    [MemberData(nameof(TimeEntryActivitiesDeserializeTheoryData))]
    public void TimeEntryActivities_Should_Deserialize(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<TimeEntryActivity>(input);
        
        Assert.NotNull(output);
        Assert.Single(output.Items);

        var timeEntryActivities = output.Items.ToList();
        Assert.Equal(8, timeEntryActivities[0].Id);
        Assert.Equal("Design", timeEntryActivities[0].Name);
        Assert.False(timeEntryActivities[0].IsDefault);
    }
    
    [Theory]
    [MemberData(nameof(DocumentCategoriesDeserializeTheoryData))]
    public void DocumentCategories_Should_Deserialize(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<DocumentCategory>(input);
        
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
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssuePrioritiesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue_priorities": [
                                    {
                                      "id": 3,
                                      "name": "Low",
                                      "is_default": false
                                    },
                                    {
                                      "id": 4,
                                      "name": "Normal",
                                      "is_default": true
                                    }
                                  ]
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TimeEntryActivitiesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "time_entry_activities": [
                                    {
                                      "id": 8,
                                      "name": "Design",
                                      "is_default": false
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <time_entry_activities type="array">
                                   <time_entry_activity>
                                       <id>8</id>
                                       <name>Design</name>
                                       <is_default>false</is_default>
                                   </time_entry_activity>
                               </time_entry_activities>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> DocumentCategoriesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "document_categories": [
                                    {
                                      "id": 1,
                                      "name": "Uncategorized",
                                      "is_default": false
                                    },
                                    {
                                      "id": 2,
                                      "name": "User documentation",
                                      "is_default": false
                                    },
                                    {
                                      "id": 3,
                                      "name": "Technical documentation",
                                      "is_default": false
                                    }
                                  ]
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}