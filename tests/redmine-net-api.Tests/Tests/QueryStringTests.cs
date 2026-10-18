using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class QueryStringTests
{
    private const string UploadXml = "<upload><token>abc</token></upload>";

    private static RequestOptions Query(params (string Key, string Value)[] values)
    {
        var query = new NameValueCollection();
        foreach (var (key, value) in values)
        {
            query.Add(key, value);
        }

        return new RequestOptions { QueryString = query };
    }

    [Fact]
    public async Task UploadFileAsync_Keeps_File_Name_And_Query_String()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));

        await manager.UploadFileAsync([1, 2], "a b.txt", Query(("x", "1"), ("y", "2")), TestContext.Current.CancellationToken);

        Assert.Equal("/uploads.xml?filename=a%20b.txt&x=1&y=2", server.Requests.Single().RawUrl);
    }

    [Fact]
    public void Upload_Keeps_Address_Query_And_Query_String()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));

        manager.ApiClient.Upload(manager.RedmineApiUrls.UploadFragment("a.txt"), [1], Query(("x", "1")));

        Assert.Equal("/uploads.xml?filename=a.txt&x=1", server.Requests.Single().RawUrl);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Address_Without_Query_Is_Unchanged(bool withQueryString)
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));
        var options = withQueryString ? Query(("x", "1"), ("status_id", "1"), ("status_id", "2")) : null;

        await manager.UploadFileAsync([1], null, options, TestContext.Current.CancellationToken);

        Assert.Equal(withQueryString ? "/uploads.xml?x=1&status_id=1,2" : "/uploads.xml", server.Requests.Single().RawUrl);
    }
}
