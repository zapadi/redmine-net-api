using System;
using System.Collections.Generic;
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
public sealed class PagingCharacterizationTests
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

    private static TestHttpServer IssuesServer(int total, bool json = false)
    {
        return new TestHttpServer(request => request.RawUrl.Contains("issue_statuses")
            ? RedminePagedServer.IssueStatuses(7, json)
            : RedminePagedServer.Issues(request, total, json));
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

    private static string[] Urls(TestHttpServer server)
    {
        return server.Requests.Select(r => r.RawUrl).ToArray();
    }

    private static string[] Sorted(IEnumerable<string> urls)
    {
        return urls.OrderBy(u => u, StringComparer.Ordinal).ToArray();
    }

    private static int[] Ids(IEnumerable<Issue> issues)
    {
        return issues.Select(i => i.Id).ToArray();
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Get_Issues_Default_Page_Size_Fetches_All_Pages(bool async, bool json)
    {
        var ext = json ? "json" : "xml";
        using var server = IssuesServer(60, json);
        var manager = CreateManager(server, json);

        var issues = async
            ? await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<Issue>();

        Assert.Equal(Enumerable.Range(1, 60).ToArray(), Ids(issues));
        Assert.Equal(
            [$"/issues.{ext}?limit=25&offset=0", $"/issues.{ext}?limit=25&offset=25", $"/issues.{ext}?limit=25&offset=50"],
            Sorted(Urls(server)));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Issues_Custom_Page_Size_Up_To_Server_Max(bool async)
    {
        using var server = IssuesServer(250);
        var manager = CreateManager(server, pageSize: 100);

        var issues = async
            ? await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<Issue>();

        Assert.Equal(Enumerable.Range(1, 250).ToArray(), Ids(issues));
        Assert.Equal(["/issues.xml?limit=100&offset=0", "/issues.xml?limit=100&offset=100", "/issues.xml?limit=100&offset=200"], Sorted(Urls(server)));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Issues_Exact_Multiple_Of_Page_Size(bool async)
    {
        using var server = IssuesServer(50);
        var manager = CreateManager(server);

        var issues = async
            ? await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<Issue>();

        Assert.Equal(50, issues.Count);
        Assert.Equal(["/issues.xml?limit=25&offset=0", "/issues.xml?limit=25&offset=25"], Sorted(Urls(server)));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Issues_With_User_Limit_Returns_One_Page_Of_Limit(bool async)
    {
        using var server = IssuesServer(60);
        var manager = CreateManager(server);
        var options = Query(("limit", "10"), ("offset", "5"));

        var issues = async
            ? await manager.GetAsync<Issue>(options, TestContext.Current.CancellationToken)
            : manager.Get<Issue>(options);

        Assert.Equal(Enumerable.Range(6, 10).ToArray(), Ids(issues));
        Assert.Equal(["/issues.xml?limit=10&offset=5"], Urls(server));
        Assert.Equal("10", options.QueryString["limit"]);
        Assert.Equal("5", options.QueryString["offset"]);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Issues_With_User_Offset_Fetches_Remaining_Pages(bool async)
    {
        using var server = IssuesServer(60);
        var manager = CreateManager(server);
        var options = Query(("offset", "30"));

        var issues = async
            ? await manager.GetAsync<Issue>(options, TestContext.Current.CancellationToken)
            : manager.Get<Issue>(options);

        Assert.Equal(Enumerable.Range(31, 30).ToArray(), Ids(issues));
        Assert.Equal(["/issues.xml?offset=30&limit=25", "/issues.xml?offset=55&limit=25"], Sorted(Urls(server)));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Issues_With_Filters_Keeps_Filters_On_Every_Page(bool async)
    {
        using var server = IssuesServer(30);
        var manager = CreateManager(server);
        var options = Query(("project_id", "p-1"), ("status_id", "*"));

        var issues = async
            ? await manager.GetAsync<Issue>(options, TestContext.Current.CancellationToken)
            : manager.Get<Issue>(options);

        Assert.Equal(30, issues.Count);
        Assert.Equal(["/issues.xml?project_id=p-1&status_id=*&limit=25&offset=0", "/issues.xml?project_id=p-1&status_id=*&limit=25&offset=25"], Sorted(Urls(server)));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Issues_Empty_Result(bool async)
    {
        using var server = IssuesServer(0);
        var manager = CreateManager(server);

        var issues = async
            ? await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<Issue>();

        Assert.Null(issues);
        Assert.Equal(["/issues.xml?limit=25&offset=0"], Urls(server));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Get_Non_Paginated_Type_Makes_One_Request(bool async, bool json)
    {
        var ext = json ? "json" : "xml";
        using var server = IssuesServer(0, json);
        var manager = CreateManager(server, json);

        var statuses = async
            ? await manager.GetAsync<IssueStatus>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<IssueStatus>();

        Assert.Equal(7, statuses.Count);
        Assert.Equal([$"/issue_statuses.{ext}?limit=25"], Urls(server));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Non_Paginated_Type_With_User_Limit_And_Offset_Makes_One_Request(bool async)
    {
        using var server = IssuesServer(0);
        var manager = CreateManager(server, pageSize: 500);
        var options = Query(("limit", "3"), ("offset", "2"));

        var statuses = async
            ? await manager.GetAsync<IssueStatus>(options, TestContext.Current.CancellationToken)
            : manager.Get<IssueStatus>(options);

        Assert.Equal(7, statuses.Count);
        Assert.Equal(["/issue_statuses.xml?limit=3&offset=2"], Urls(server));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Count_Requests_One_Item(bool async)
    {
        using var server = IssuesServer(60);
        var manager = CreateManager(server);
        var options = Query(("project_id", "p-1"));

        var count = async
            ? await manager.CountAsync<Issue>(options, TestContext.Current.CancellationToken)
            : manager.Count<Issue>(options);

        Assert.Equal(60, count);
        Assert.Equal(["/issues.xml?project_id=p-1&limit=1&offset=0"], Urls(server));
        Assert.Null(options.QueryString["limit"]);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetPaginated_Sends_Options_As_Is(bool async)
    {
        using var server = IssuesServer(60);
        var manager = CreateManager(server);

        var first = async
            ? await manager.GetPagedAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.GetPaginated<Issue>();
        var second = async
            ? await manager.GetPagedAsync<Issue>(Query(("limit", "10"), ("offset", "20")), TestContext.Current.CancellationToken)
            : manager.GetPaginated<Issue>(Query(("limit", "10"), ("offset", "20")));

        Assert.Equal(["/issues.xml", "/issues.xml?limit=10&offset=20"], Urls(server));
        Assert.Equal(25, first.Items.Count());
        Assert.Equal(60, first.TotalItems);
        Assert.Equal(25, first.PageSize);
        Assert.Equal(0, first.Offset);
        Assert.Equal(1, first.CurrentPage);
        Assert.Equal(Enumerable.Range(21, 10).ToArray(), Ids(second.Items));
        Assert.Equal(20, second.Offset);
        Assert.Equal(3, second.CurrentPage);
    }
}
