using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public sealed class SearchTests()
{
    [Theory]
    [MemberData(nameof(SearchDeserializeTheoryData))]
    public void Should_Deserialize_Search_Result(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Search>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);
        Assert.Equal(25, output.PageSize);

        var results = output.Items.ToList();
        Assert.Equal(5, results[0].Id);
        Assert.Equal("Wiki: Wiki_Page_Name", results[0].Title);
        Assert.Equal("wiki-page", results[0].Type);
        Assert.Equal("http://www.redmine.org/projects/new_crm_dev/wiki/Wiki_Page_Name", results[0].Url);
        Assert.Equal("h1. Wiki Page Name wiki_keyword", results[0].Description);
        AssertDateTime.Equal("2016-3-25T05:23:35", results[0].DateTime);

        Assert.Equal(10, results[1].Id);
        Assert.Equal("Issue #10 (Closed): Issue_Title", results[1].Title);
        Assert.Equal("issue closed", results[1].Type);
        Assert.Equal("http://www.redmin.org/issues/10", results[1].Url);
        Assert.Equal("issue_keyword", results[1].Description);
        AssertDateTime.Equal("2016-3-24T05:18:59", results[1].DateTime);

    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> SearchDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "results": [
                                    {
                                      "id": 5,
                                      "title": "Wiki: Wiki_Page_Name",
                                      "type": "wiki-page",
                                      "url": "http://www.redmine.org/projects/new_crm_dev/wiki/Wiki_Page_Name",
                                      "description": "h1. Wiki Page Name wiki_keyword",
                                      "datetime": "2016-03-25T05:23:35Z"
                                    },
                                    {
                                      "id": 10,
                                      "title": "Issue #10 (Closed): Issue_Title",
                                      "type": "issue closed",
                                      "url": "http://www.redmin.org/issues/10",
                                      "description": "issue_keyword",
                                      "datetime": "2016-03-24T05:18:59Z"
                                    }
                                  ],
                                  "total_count": 2,
                                  "offset": 0,
                                  "limit": 25
                                }
                                """;

            const string xml = """
                               <results total_count="2" offset="0" limit="25" type="array">
                                   <result>
                                       <id>5</id>
                                       <title>Wiki: Wiki_Page_Name</title>
                                       <type>wiki-page</type>
                                       <url>http://www.redmine.org/projects/new_crm_dev/wiki/Wiki_Page_Name</url>
                                       <description>h1. Wiki Page Name wiki_keyword</description>
                                       <datetime>2016-03-25T05:23:35Z</datetime>
                                   </result>
                                   <result>
                                       <id>10</id>
                                       <title>Issue #10 (Closed): Issue_Title</title>
                                       <type>issue closed</type>
                                       <url>http://www.redmin.org/issues/10</url>
                                       <description>issue_keyword</description>
                                       <datetime>2016-03-24T05:18:59Z</datetime>
                                   </result>
                               </results>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}