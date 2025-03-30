using System;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public sealed class SearchTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Search_Result()
    {
        const string input = """
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
        
        var output = fixture.Serializer.DeserializeToPagedResults<Search>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);
        Assert.Equal(25, output.PageSize);

        var results = output.Items.ToList();
        Assert.Equal(5, results[0].Id);
        Assert.Equal("Wiki: Wiki_Page_Name", results[0].Title);
        Assert.Equal("wiki-page", results[0].Type);
        Assert.Equal("http://www.redmine.org/projects/new_crm_dev/wiki/Wiki_Page_Name", results[0].Url);
        Assert.Equal("h1. Wiki Page Name wiki_keyword", results[0].Description);
        Assert.Equal(new DateTime(2016, 3, 25, 5, 23, 35, DateTimeKind.Utc).ToLocalTime(), results[0].DateTime);

        Assert.Equal(10, results[1].Id);
        Assert.Equal("Issue #10 (Closed): Issue_Title", results[1].Title);
        Assert.Equal("issue closed", results[1].Type);
        Assert.Equal("http://www.redmin.org/issues/10", results[1].Url);
        Assert.Equal("issue_keyword", results[1].Description);
        Assert.Equal(new DateTime(2016, 3, 24, 5, 18, 59, DateTimeKind.Utc).ToLocalTime(), results[1].DateTime);

    }
}