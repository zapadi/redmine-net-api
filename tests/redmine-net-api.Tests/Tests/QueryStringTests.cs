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

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task UploadFileAsync_Keeps_File_Name_And_Query_String()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));

        await manager.UploadFileAsync([1, 2], "a b.txt", Query(("x", "1"), ("y", "2")), TestContext.Current.CancellationToken);

        Assert.Equal("/uploads.xml?filename=a%20b.txt&x=1&y=2", server.Requests.Single().RawUrl);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Upload_Keeps_Address_Query_And_Query_String()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));

        manager.ApiClient.Upload(manager.RedmineApiUrls.UploadFragment("a.txt"), [1], Query(("x", "1")));

        Assert.Equal("/uploads.xml?filename=a.txt&x=1", server.Requests.Single().RawUrl);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, "/uploads.xml?filename=a.txt&x=1")]
    [InlineData(true, "/uploads.xml?filename=a.txt&x=1")]
    public void Address_Query_Keys_Are_Not_Duplicated(bool encode, string expected)
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var builder = new RedmineManagerOptionsBuilder().WithHost(server.Url);
        if (encode)
        {
            builder.WithQueryStringEncoding();
        }

        var manager = new RedmineManager(builder);

        manager.ApiClient.Upload(manager.RedmineApiUrls.UploadFragment("a.txt"), [1], Query(("filename", "b.txt"), ("x", "1")));

        Assert.Equal(expected, server.Requests.Single().RawUrl);
    }

    [Theory(Timeout = Constants.TestTimeout)]
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

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, "/search.xml?q=C&C&limit=25&offset=0")]
    [InlineData(true, "/search.xml?q=C%26C&limit=25&offset=0")]
    public void Search_Query_Is_Encoded_Only_When_Enabled(bool encode, string expected)
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response("<results type=\"array\" total_count=\"0\" offset=\"0\" limit=\"25\"></results>"));
        var builder = new RedmineManagerOptionsBuilder().WithHost(server.Url);
        if (encode)
        {
            builder.WithQueryStringEncoding();
        }

        var manager = new RedmineManager(builder);

        manager.Search("C&C");

        Assert.Equal(expected, server.Requests.Single().RawUrl);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, "/issues.xml?subject=~C++&status_id=1,2&limit=1&offset=0")]
    [InlineData(true, "/issues.xml?subject=~C%2B%2B&status_id=1%2C2&limit=1&offset=0")]
    public async Task Query_String_Keys_And_Values_Are_Encoded_Only_When_Enabled(bool encode, string expected)
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response("<issues type=\"array\" total_count=\"0\" offset=\"0\" limit=\"1\"></issues>"));
        var builder = new RedmineManagerOptionsBuilder().WithHost(server.Url);
        if (encode)
        {
            builder.WithQueryStringEncoding();
        }

        var manager = new RedmineManager(builder);

        await manager.CountAsync<Redmine.Net.Api.Types.Issue>(Query(("subject", "~C++"), ("status_id", "1"), ("status_id", "2")), TestContext.Current.CancellationToken);

        Assert.Equal(expected, server.Requests.Single().RawUrl);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Encoding_Keeps_Address_Query()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(UploadXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url).WithQueryStringEncoding());

        manager.ApiClient.Upload(manager.RedmineApiUrls.UploadFragment("a b.txt"), [1], Query(("x y", "1&2")));

        Assert.Equal("/uploads.xml?filename=a%20b.txt&x%20y=1%262", server.Requests.Single().RawUrl);
    }
}
