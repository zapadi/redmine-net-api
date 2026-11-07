using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class SearchAsyncTests
{
    private const string EmptyResults = "<results type=\"array\" total_count=\"0\" offset=\"0\" limit=\"25\"></results>";

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task SearchAsync_With_Request_Options_Merges_Query_And_Keeps_Impersonation()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(EmptyResults));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));
        var options = new RequestOptions
        {
            ImpersonateUser = "bob",
            QueryString = new NameValueCollection { { "x", "1" }, { "limit", "999" } },
        };

        await manager.SearchAsync("redmine", 10, 5, new SearchFilterBuilder { IncludeIssues = true }, options, TestContext.Current.CancellationToken);

        var request = server.Requests.Single();
        Assert.Equal("/search.xml?x=1&limit=10&q=redmine&offset=5&issues=1", request.RawUrl);
        Assert.Equal("bob", request.Headers[RedmineConstants.IMPERSONATE_HEADER_KEY]);
        Assert.Equal("999", options.QueryString["limit"]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task SearchAsync_With_Null_Request_Options_Matches_Default_Overload()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(EmptyResults));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));

        await manager.SearchAsync("redmine", RedmineConstants.DEFAULT_PAGE_SIZE_VALUE, 0, null, null, TestContext.Current.CancellationToken);
        await manager.SearchAsync("redmine", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(["/search.xml?q=redmine&limit=25&offset=0", "/search.xml?q=redmine&limit=25&offset=0"], server.Requests.Select(r => r.RawUrl).ToArray());
    }
}
