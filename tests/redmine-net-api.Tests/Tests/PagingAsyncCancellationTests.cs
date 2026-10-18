using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private int _inFlight;
        private int _maxInFlight;

        public int Started => Volatile.Read(ref _started);
        public int Completed => Volatile.Read(ref _completed);
        public int MaxInFlight => Volatile.Read(ref _maxInFlight);

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            throw new NotSupportedException();
        }

        public async Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            var match = Regex.Match(request.Uri.Query, "[?&]offset=([0-9]+)");
            var offset = match.Success ? int.Parse(match.Groups[1].Value) : 0;
            if (offset == 0)
            {
                return Page(offset);
            }

            onPageStarted(Interlocked.Increment(ref _started));
            var inFlight = Interlocked.Increment(ref _inFlight);
            int max;
            while (inFlight > (max = Volatile.Read(ref _maxInFlight)) && Interlocked.CompareExchange(ref _maxInFlight, inFlight, max) != max)
            {
            }

            await Task.Delay(300, CancellationToken.None);
            Interlocked.Decrement(ref _inFlight);
            Interlocked.Increment(ref _completed);

            return offset == failingOffset
                ? new RedmineTransportResponse(500, null, null)
                : Page(offset);
        }

        private static RedmineTransportResponse Page(int offset)
        {
            var items = string.Concat(Enumerable.Range(offset + 1, 25).Select(id => $"<issue><id>{id}</id></issue>"));
            return new RedmineTransportResponse(200, null, Encoding.UTF8.GetBytes($"<issues type=\"array\" total_count=\"{Total}\" offset=\"{offset}\" limit=\"25\">{items}</issues>"));
        }
    }

    private static RedmineManager CreateManager(IRedmineTransport transport, int maxConcurrentRequests = 0)
    {
        var builder = new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithTransport(transport);
        if (maxConcurrentRequests > 0)
        {
            builder.WithMaxConcurrentRequests(maxConcurrentRequests);
        }

        return new RedmineManager(builder);
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    public async Task Page_Requests_Respect_Max_Concurrent_Requests(int configured, int expected)
    {
        var transport = new SlowPagesTransport(_ => { });
        var manager = CreateManager(transport, configured);

        var issues = await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(Total, issues.Count);
        Assert.InRange(transport.MaxInFlight, Math.Min(2, expected), expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Max_Concurrent_Requests_Below_One_Throws(int value)
    {
        Assert.Throws<RedmineException>(() => new RedmineManagerOptionsBuilder().WithMaxConcurrentRequests(value));
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
