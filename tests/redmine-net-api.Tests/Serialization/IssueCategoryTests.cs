using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class IssueCategoryTests()
{
    [Theory]
    [MemberData(nameof(IssueCategoryDeserializeTheoryData))]
    public void Should_Deserialize_Issue_Category(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Redmine.Net.Api.Types.IssueCategory>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("UI", output.Name);
    }
    
    [Theory]
    [MemberData(nameof(IssueCategoriesDeserializeTheoryData))]
    public void Should_Deserialize_Issue_Categories(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.IssueCategory>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var issueCategories = output.Items.ToList();
        Assert.Equal(2, issueCategories.Count);

        Assert.Equal(57, issueCategories[0].Id);
        Assert.Equal("Foo", issueCategories[0].Project.Name);
        Assert.Equal(17, issueCategories[0].Project.Id);
        Assert.Equal("UI", issueCategories[0].Name);
        Assert.Equal("John Smith", issueCategories[0].AssignTo.Name);
        Assert.Equal(22, issueCategories[0].AssignTo.Id);

        Assert.Equal(58, issueCategories[1].Id);
        Assert.Equal("Foo", issueCategories[1].Project.Name);
        Assert.Equal(17, issueCategories[1].Project.Id);
        Assert.Equal("Test", issueCategories[1].Name);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueCategoryDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue_category": {
                                    "id": 2,
                                    "project": {
                                      "id": 1,
                                      "name": "Redmine"
                                    },
                                    "name": "UI"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <issue_category>
                                   <id>2</id>
                                   <project name="Redmine" id="1"/>
                                   <name>UI</name>
                               </issue_category>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueCategoriesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue_categories": [
                                    {
                                      "id": 57,
                                      "project": {
                                        "id": 17,
                                        "name": "Foo"
                                      },
                                      "name": "UI",
                                      "assigned_to": {
                                        "id": 22,
                                        "name": "John Smith"
                                      }
                                    },
                                    {
                                      "id": 58,
                                      "project": {
                                        "id": 17,
                                        "name": "Foo"
                                      },
                                      "name": "Test"
                                    }
                                  ],
                                  "total_count": 2
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <issue_categories type="array" total_count="2">
                                   <issue_category>
                                       <id>57</id>
                                       <project name="Foo" id="17"/>
                                       <name>UI</name>
                                       <assigned_to name="John Smith" id="22"/>
                                   </issue_category>
                                   <issue_category>
                                       <id>58</id>
                                       <project name="Foo" id="17"/>
                                       <name>Test</name>
                                   </issue_category>
                               </issue_categories>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}