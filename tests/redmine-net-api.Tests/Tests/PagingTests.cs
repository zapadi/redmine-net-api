using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class PagingTests
{
    private static RedmineManager CreateManager(TestHttpServer server, bool json = false, int pageSize = 0)
    {
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost(server.Url)
            .WithSerializationType(json ? SerializationType.Json : SerializationType.Xml);
        if (pageSize > 0)
        {
            builder.WithPageSize(pageSize);
        }

        return new RedmineManager(builder);
    }

    private static RequestOptions Query(params (string Key, string Value)[] values)
    {
        var query = new NameValueCollection();
        foreach (var (key, value) in values)
        {
            query.Add(key, value);
        }

        return new RequestOptions { QueryString = query };
    }

    private static string[] SortedUrls(TestHttpServer server)
    {
        return server.Requests.Select(r => r.RawUrl).OrderBy(u => u, StringComparer.Ordinal).ToArray();
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Page_Size_Above_Server_Max_Returns_All_Items(bool async, bool json)
    {
        var ext = json ? "json" : "xml";
        using var server = new TestHttpServer(r => RedminePagedServer.Issues(r, 250, json));
        var manager = CreateManager(server, json, pageSize: 500);

        var issues = async
            ? await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<Issue>();

        Assert.Equal(Enumerable.Range(1, 250).ToArray(), issues.Select(i => i.Id).ToArray());
        Assert.Equal([$"/issues.{ext}?limit=500&offset=0", $"/issues.{ext}?limit=500&offset=100", $"/issues.{ext}?limit=500&offset=200"], SortedUrls(server));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task User_Limit_Above_Server_Max_Returns_Limit_Items(bool async)
    {
        using var server = new TestHttpServer(r => RedminePagedServer.Issues(r, 1000));
        var manager = CreateManager(server);
        var options = Query(("limit", "250"), ("offset", "10"));

        var issues = async
            ? await manager.GetAsync<Issue>(options, TestContext.Current.CancellationToken)
            : manager.Get<Issue>(options);

        Assert.Equal(Enumerable.Range(11, 250).ToArray(), issues.Select(i => i.Id).ToArray());
        Assert.Equal(["/issues.xml?limit=250&offset=10", "/issues.xml?limit=250&offset=110", "/issues.xml?limit=250&offset=210"], SortedUrls(server));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task User_Limit_Above_Total_Returns_All_Items(bool async)
    {
        using var server = new TestHttpServer(r => RedminePagedServer.Issues(r, 120));
        var manager = CreateManager(server);
        var options = Query(("limit", "250"));

        var issues = async
            ? await manager.GetAsync<Issue>(options, TestContext.Current.CancellationToken)
            : manager.Get<Issue>(options);

        Assert.Equal(Enumerable.Range(1, 120).ToArray(), issues.Select(i => i.Id).ToArray());
        Assert.Equal(["/issues.xml?limit=250&offset=0", "/issues.xml?limit=250&offset=100"], SortedUrls(server));
    }

    [Fact]
    public void Stops_When_Server_Returns_No_Items_Before_Total()
    {
        using var server = new TestHttpServer(r => RedminePagedServer.ParseQuery(r.RawUrl)["offset"] == "50"
            ? RedminePagedServer.Response("<issues type=\"array\" total_count=\"1000\" offset=\"50\" limit=\"25\"></issues>")
            : RedminePagedServer.Issues(r, 1000));
        var manager = CreateManager(server);

        var issues = manager.Get<Issue>();

        Assert.Equal(50, issues.Count);
        Assert.Equal(["/issues.xml?limit=25&offset=0", "/issues.xml?limit=25&offset=25", "/issues.xml?limit=25&offset=50"], SortedUrls(server));
    }
}
