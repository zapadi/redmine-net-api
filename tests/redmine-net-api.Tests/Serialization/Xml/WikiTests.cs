using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class WikiTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Wiki_Page()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <wiki_page>
            <title>UsersGuide</title>
            <parent title="Installation_Guide"/>
            <text>h1. Users Guide
            ...
            ...</text>
            <version>22</version>
            <author id="11" name="John Smith"/>
            <comments>Typo</comments>
            <created_on>2009-05-18T20:11:52Z</created_on>
            <updated_on>2012-10-02T11:38:18Z</updated_on>
        </wiki_page>
        """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.WikiPage>(input);
        
        Assert.NotNull(output);
        Assert.Equal("UsersGuide", output.Title);
        Assert.NotNull(output.ParentTitle);
        Assert.Equal("Installation_Guide", output.ParentTitle);
        
        Assert.NotNull(output.Text);
        Assert.False(string.IsNullOrWhiteSpace(output.Text), "Text should not be empty");

        var lines = output.Text!.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var firstLine = lines[0].Trim();
        
        Assert.Equal("h1. Users Guide", firstLine);
        
        Assert.Equal(22, output.Version);
        Assert.NotNull(output.Author);
        Assert.Equal(11, output.Author.Id);
        Assert.Equal("John Smith", output.Author.Name);
        Assert.Equal("Typo", output.Comments);
        Assert.Equal(new DateTime(2009, 5, 18, 20, 11, 52, DateTimeKind.Utc).ToLocalTime(), output.CreatedOn);
        Assert.Equal(new DateTime(2012, 10, 2, 11, 38, 18, DateTimeKind.Utc).ToLocalTime(), output.UpdatedOn);

    }

    [Fact]
    public void Should_Deserialize_Wiki_Pages()
    {
        const string input = """ 
        <?xml version="1.0"?>
        <wiki_pages type="array">
            <wiki_page>
            <title>UsersGuide</title>
            <version>2</version>
            <created_on>2008-03-09T12:07:08Z</created_on>
            <updated_on>2008-03-09T23:41:33+01:00</updated_on>
            </wiki_page>
        </wiki_pages>
        """;
    
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.WikiPage>(input);
        
        Assert.NotNull(output);
        Assert.Equal(1, output.TotalItems);

        var wikiPages = output.Items.ToList();
        Assert.Equal("UsersGuide", wikiPages[0].Title);
        Assert.Equal(2, wikiPages[0].Version);
        Assert.Equal(new DateTime(2008, 3, 9, 12, 7, 8, DateTimeKind.Utc).ToLocalTime(), wikiPages[0].CreatedOn);
        Assert.Equal(new DateTime(2008, 3, 9, 22, 41, 33, DateTimeKind.Utc).ToLocalTime(), wikiPages[0].UpdatedOn);
    }
}

                            

                             