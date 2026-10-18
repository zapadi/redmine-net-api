using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class UrlEscapingTests
{
    private static readonly RedmineApiUrls Xml = new(RedmineConstants.XML);

    [Theory]
    [InlineData("My Page", "My%20Page")]
    [InlineData("a/b", "a%2Fb")]
    [InlineData("what?", "what%3F")]
    [InlineData("C#", "C%23")]
    [InlineData("100%", "100%25")]
    [InlineData("a&b=c", "a%26b%3Dc")]
    [InlineData("Page_1-x.y~z", "Page_1-x.y~z")]
    public void Path_Segments_Are_Escaped(string value, string escaped)
    {
        Assert.Equal($"projects/{escaped}/wiki/{escaped}.xml", Xml.ProjectWikiPage(value, value));
        Assert.Equal($"projects/{escaped}/wiki/{escaped}/{escaped}.xml", Xml.ProjectWikiPageVersion(value, value, value));
        Assert.Equal($"projects/{escaped}/repository/{escaped}/revisions/{escaped}/issues/{escaped}.xml", Xml.ProjectRepositoryRemoveRelatedIssue(value, value, value, value));
        Assert.Equal($"projects/{escaped}/close.xml", Xml.ProjectClose(value));
        Assert.Equal($"projects/{escaped}/news.xml", Xml.ProjectNews(value));
        Assert.Equal($"issues/{escaped}/watchers/{escaped}.xml", Xml.IssueWatcherRemove(value, value));
        Assert.Equal($"groups/{escaped}/users/{escaped}.xml", Xml.GroupUserRemove(value, value));
        Assert.Equal($"attachments/issues/{escaped}.xml", Xml.AttachmentUpdate(value));
        Assert.Equal($"projects/{escaped}.xml", Xml.GetFragment<Project>(value));
        Assert.Equal($"projects/{escaped}.xml", Xml.UpdateFragment<Project>(value));
        Assert.Equal($"projects/{escaped}.xml", Xml.DeleteFragment<Project>(value));
        Assert.Equal($"projects/{escaped}/versions.xml", Xml.GetListFragment<Version>(value));
        Assert.Equal($"projects/{escaped}/files.xml", Xml.CreateEntityFragment<File>(value));
        Assert.Equal($"issues/{escaped}/relations.xml", Xml.CreateEntityFragment<IssueRelation>(value));
        Assert.Equal($"attachments/issues/{escaped}.xml", Xml.PatchFragment<Attachment>(value));
    }

    [Fact]
    public void Null_Id_Produces_Empty_Segment()
    {
        Assert.Equal("projects/.xml", Xml.GetFragment<Project>(null));
        Assert.Equal("projects/p/wiki/.xml", Xml.ProjectWikiPage("p", null));
    }

    [Theory]
    [InlineData("Page_1", "/projects/p-1/wiki/Page_1.xml")]
    [InlineData("My Page", "/projects/p-1/wiki/My%20Page.xml")]
    [InlineData("Café", "/projects/p-1/wiki/Caf%C3%A9.xml")]
    [InlineData("what?", "/projects/p-1/wiki/what%3F.xml")]
    [InlineData("C#", "/projects/p-1/wiki/C%23.xml")]
    [InlineData("100%", "/projects/p-1/wiki/100%25.xml")]
    public void Wiki_Page_Request_Url(string pageName, string expected)
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response("<wiki_page><title>x</title></wiki_page>"));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));

        manager.GetWikiPage("p-1", pageName);

        Assert.Equal(expected, server.Requests.Single().RawUrl);
    }
}
