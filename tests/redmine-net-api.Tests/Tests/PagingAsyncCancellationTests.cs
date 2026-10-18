using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class PagingAsyncCancellationTests
{
    private const int Total = 200;

    private sealed class SlowPagesTransport(Action<int> onPageStarted, int failingOffset = -1) : IRedmineTransport
    {
        private int _started;
        private int _completed;

        public int Started => Volatile.Read(ref _started);
        public int Completed => Volatile.Read(ref _completed);

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            throw new NotSupportedException();
        }

        public async Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            var offset = int.Parse(request.QueryString?["offset"] ?? "0");
            if (offset == 0)
            {
                return Page(offset);
            }

            onPageStarted(Interlocked.Increment(ref _started));
            await Task.Delay(300, CancellationToken.None);
            Interlocked.Increment(ref _completed);

            return offset == failingOffset
                ? new RedmineTransportResponse { StatusCode = 500 }
                : Page(offset);
        }

        private static RedmineTransportResponse Page(int offset)
        {
            var items = string.Concat(Enumerable.Range(offset + 1, 25).Select(id => $"<issue><id>{id}</id></issue>"));
            return new RedmineTransportResponse
            {
                IsSuccessStatusCode = true,
                StatusCode = 200,
                Content = $"<issues type=\"array\" total_count=\"{Total}\" offset=\"{offset}\" limit=\"25\">{items}</issues>",
            };
        }
    }

    private static RedmineManager CreateManager(IRedmineTransport transport)
    {
        return new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000"), transport);
    }

    [Fact]
    public async Task Cancellation_Waits_For_Started_Page_Requests()
    {
        using var cts = new CancellationTokenSource();
        var transport = new SlowPagesTransport(started =>
        {
            if (started == 3)
            {
                cts.Cancel();
            }
        });
        var manager = CreateManager(transport);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => manager.GetAsync<Issue>(cancellationToken: cts.Token));

        Assert.Equal(3, transport.Started);
        Assert.Equal(transport.Started, transport.Completed);
    }

    [Fact]
    public async Task Page_Failure_Surfaces_After_Started_Pages_Complete()
    {
        var transport = new SlowPagesTransport(_ => { }, failingOffset: 50);
        var manager = CreateManager(transport);

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(500, exception.HttpStatusCode);
        Assert.Equal(Total / 25 - 1, transport.Started);
        Assert.Equal(transport.Started, transport.Completed);
    }
}
