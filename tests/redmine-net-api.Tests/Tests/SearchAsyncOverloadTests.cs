using System.Linq;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

#pragma warning disable xUnit1051

[Collection(Constants.DeserializeCollection)]
public sealed class SearchAsyncOverloadTests
{
    private const string EmptyResults = "<results type=\"array\" total_count=\"0\" offset=\"0\" limit=\"25\"></results>";

    [Fact]
    public async Task Old_Call_Shapes_Compile_And_Send_Request_Without_Impersonation()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(EmptyResults));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));
        var ct = TestContext.Current.CancellationToken;

        await manager.SearchAsync("q");
        await manager.SearchAsync("q", default);
        await manager.SearchAsync("q", 1, 2);
        await manager.SearchAsync("q", 1, 0, null, default);
        await manager.SearchAsync("q", cancellationToken: ct);
        await manager.SearchAsync("q", searchFilter: null, cancellationToken: ct);

        Assert.Equal(6, server.Requests.Count);
        Assert.All(server.Requests, r => Assert.Null(r.Headers[RedmineConstants.IMPERSONATE_HEADER_KEY]));
    }

    [Fact]
    public async Task Call_With_Request_Options_Binds_To_New_Overload()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response(EmptyResults));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));
        var options = new RequestOptions { ImpersonateUser = "bob" };

        await manager.SearchAsync("q", 1, 0, null, options);

        Assert.Equal("bob", server.Requests.Single().Headers[RedmineConstants.IMPERSONATE_HEADER_KEY]);
    }
}
