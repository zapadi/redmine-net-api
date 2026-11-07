using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class BaseAddressRequestTests
{
    private const string ProjectXml = "<project><id>1</id><name>foo</name></project>";

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData("", "/projects/1.xml")]
    [InlineData("redmine", "/redmine/projects/1.xml")]
    [InlineData("redmine/", "/redmine/projects/1.xml")]
    [InlineData("apps/redmine", "/apps/redmine/projects/1.xml")]
    [InlineData("redmine.v5", "/redmine.v5/projects/1.xml")]
    [InlineData("redmine.v5/", "/redmine.v5/projects/1.xml")]
    [InlineData("redmine/issues.xml", "/redmine/projects/1.xml")]
    public void Requests_Are_Sent_Under_The_Configured_Base_Path(string path, string expected)
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(ProjectXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url + path));

        manager.Get<Project>("1");

        Assert.Equal(expected, server.Requests.Single().RawUrl);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Attachment_Patch_Is_Sent_Under_The_Configured_Base_Path()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(string.Empty));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url + "redmine/"));

        manager.ApiClient.Patch(manager.RedmineApiUrls.PatchFragment<Attachment>("12"), "<attachments/>");

        Assert.Equal("/redmine/attachments/issues/12.xml", server.Requests.Single().RawUrl);
    }
}
