using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public abstract class RedmineTransportContractTests
{
    private static readonly string[] TransportHeaders = ["Host", "Connection", "Accept-Encoding", "Expect", "Content-Length"];

    internal abstract IRedmineTransport CreateTransport(string baseUrl, TimeSpan? timeout = null, Dictionary<string, string> defaultHeaders = null, string userAgent = null);

    private static NameValueCollection Headers(params string[] nameValues)
    {
        var headers = new NameValueCollection();
        for (var i = 0; i < nameValues.Length; i += 2)
        {
            headers[nameValues[i]] = nameValues[i + 1];
        }

        return headers;
    }

    private static Dictionary<string, string> SentHeaders(RecordedRequest request)
    {
        return request.Headers.AllKeys
            .Where(key => !TransportHeaders.Contains(key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(key => key.ToLowerInvariant(), key => request.Headers[key]);
    }

    private static void AssertHeaders(Dictionary<string, string> expected, RecordedRequest request)
    {
        Assert.Equal(
            expected.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value).OrderBy(h => h.Key, StringComparer.Ordinal),
            SentHeaders(request).OrderBy(h => h.Key, StringComparer.Ordinal));
    }

    private static async Task<RedmineTransportResponse> SendAsync(IRedmineTransport transport, RedmineTransportRequest request, bool async)
    {
        return async
            ? await transport.SendAsync(request, TestContext.Current.CancellationToken)
            : transport.Send(request);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Should_Send_Method_Uri_Query_And_Headers(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/xml; charset=utf-8", Body = Encoding.UTF8.GetBytes("<ok>ţ</ok>"), Headers = new Dictionary<string, string> { { "X-Response", "value" } } });
        var transport = CreateTransport(server.Url, userAgent: "agent");
        var request = new RedmineTransportRequest
        {
            Method = HttpVerbs.GET,
            Address = "issues.xml",
            QueryString = new NameValueCollection { { "status_id", "open" }, { "limit", "1" } },
            Headers = Headers("X-Redmine-API-Key", "secret", RedmineConstants.IMPERSONATE_HEADER_KEY, "bob")
        };

        var response = await SendAsync(transport, request, async);

        var sent = Assert.Single(server.Requests);
        Assert.Equal("GET", sent.Method);
        Assert.Equal("/issues.xml?status_id=open&limit=1", sent.RawUrl);
        AssertHeaders(new Dictionary<string, string> { { "User-Agent", "agent" }, { "X-Redmine-API-Key", "secret" }, { RedmineConstants.IMPERSONATE_HEADER_KEY, "bob" } }, sent);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("<ok>ţ</ok>", response.Content);
        Assert.Equal("value", response.Headers["X-Response"]);
        Assert.Equal(server.Url, request.BaseAddress);
    }

    [Theory]
    [InlineData(false, "POST")]
    [InlineData(true, "POST")]
    [InlineData(false, "PUT")]
    [InlineData(true, "PATCH")]
    public async Task Body_Should_Be_Sent_With_Content_Type(bool async, string method)
    {
        var body = Encoding.UTF8.GetBytes("<project><name>ţ</name></project>");
        using var server = new TestHttpServer(_ => new TestHttpResponse { StatusCode = 201, Body = [1, 2] });
        var transport = CreateTransport(server.Url, userAgent: "agent");

        var response = await SendAsync(transport, new RedmineTransportRequest
        {
            Method = method,
            Address = "projects.xml",
            Headers = Headers("Content-Type", "text/plain"),
            ContentType = "application/xml",
            Body = body
        }, async);

        var sent = Assert.Single(server.Requests);
        Assert.Equal(method, sent.Method);
        Assert.Equal(body, sent.Body);
        AssertHeaders(new Dictionary<string, string> { { "User-Agent", "agent" }, { "Content-Type", "application/xml" } }, sent);
        Assert.Equal(201, response.StatusCode);
        Assert.Equal(new byte[] { 1, 2 }, response.Body);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Delete_Without_Body_Should_Send_Empty_Body(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse());
        var transport = CreateTransport(server.Url, userAgent: "agent");

        await SendAsync(transport, new RedmineTransportRequest { Method = HttpVerbs.DELETE, Address = "projects/1.xml", Headers = Headers() }, async);

        var sent = Assert.Single(server.Requests);
        Assert.Equal("DELETE", sent.Method);
        Assert.Empty(sent.Body);
        AssertHeaders(new Dictionary<string, string> { { "User-Agent", "agent" } }, sent);
    }

    [Theory]
    [InlineData(false, 404)]
    [InlineData(true, 404)]
    [InlineData(false, 500)]
    [InlineData(true, 503)]
    [InlineData(false, 422)]
    [InlineData(true, 422)]
    public async Task Error_Status_Should_Be_Returned_With_Body_And_Headers(bool async, int statusCode)
    {
        var body = Encoding.UTF8.GetBytes("<errors type=\"array\"><error>Name ţ</error></errors>");
        using var server = new TestHttpServer(_ => new TestHttpResponse { StatusCode = statusCode, ContentType = "application/xml", Body = body, Headers = new Dictionary<string, string> { { "X-Request-Id", "abc" } } });
        var transport = CreateTransport(server.Url);

        var request = new RedmineTransportRequest { Method = HttpVerbs.PUT, Address = "projects/1.xml", Headers = Headers(), ContentType = "application/xml", Body = [1] };

        var response = await SendAsync(transport, request, async);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(statusCode, response.StatusCode);
        Assert.Equal(body, response.Body);
        Assert.Null(response.BodyError);
        Assert.Equal("abc", response.Headers["X-Request-Id"]);
        Assert.Equal(server.Url, request.BaseAddress);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Download_Should_Return_Bytes_And_Report_Progress(bool async)
    {
        var data = new byte[32 * 1024];
        new Random(1).NextBytes(data);
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/octet-stream", Body = data });
        var transport = CreateTransport(server.Url);
        var progress = new RecordingProgress();

        var response = await SendAsync(transport, new RedmineTransportRequest { Method = HttpVerbs.DOWNLOAD, Address = "attachments/download/1/a.bin", Headers = Headers(), Progress = progress }, async);

        Assert.Equal("GET", Assert.Single(server.Requests).Method);
        Assert.Equal(data, response.Body);
        Assert.Null(response.Content);
        Assert.True(SpinWait.SpinUntil(() => progress.Values.Length > 0, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task SendAsync_Should_Throw_Timeout_When_Timeout_Elapses()
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Delay = TimeSpan.FromSeconds(5) });
        var transport = CreateTransport(server.Url, TimeSpan.FromMilliseconds(300));

        var exception = await Record.ExceptionAsync(() => transport.SendAsync(new RedmineTransportRequest { Method = HttpVerbs.GET, Address = "projects/1.xml", Headers = Headers() }, TestContext.Current.CancellationToken));

        Assert.True(exception is TimeoutException or WebException { Status: WebExceptionStatus.Timeout }, exception?.ToString());
    }

    [Fact]
    public async Task SendAsync_Should_Throw_OperationCanceledException_With_Inner_When_Canceled_In_Flight()
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Delay = TimeSpan.FromSeconds(5) });
        var transport = CreateTransport(server.Url);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        cts.CancelAfter(TimeSpan.FromMilliseconds(300));

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => transport.SendAsync(new RedmineTransportRequest { Method = HttpVerbs.GET, Address = "projects/1.xml", Headers = Headers() }, cts.Token));

        Assert.NotNull(exception.InnerException);
        Assert.Equal(cts.Token, exception.CancellationToken);
    }

    [Fact]
    public async Task SendAsync_Should_Throw_OperationCanceledException_Without_Inner_When_Canceled_Before_Start()
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse());
        var transport = CreateTransport(server.Url);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => transport.SendAsync(new RedmineTransportRequest { Method = HttpVerbs.GET, Address = "projects/1.xml", Headers = Headers() }, cts.Token));

        Assert.Null(exception.InnerException);
        Assert.Empty(server.Requests);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Transport_Defaults_Should_Be_Sent_Only_When_Request_Does_Not_Set_Them(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse());
        var transport = CreateTransport(server.Url, defaultHeaders: new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "default" },
            { "Accept", "text/plain" },
            { "User-Agent", "default-agent" },
            { "X-Redmine-API-Key", "default-key" },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "default-user" },
            { "Content-Type", "text/plain" }
        }, userAgent: "options-agent");

        await SendAsync(transport, new RedmineTransportRequest
        {
            Method = HttpVerbs.POST,
            Address = "projects.xml",
            Headers = Headers("X-Custom", "request", "Accept", "application/xml", "User-Agent", "custom-agent", "X-Redmine-API-Key", "secret", RedmineConstants.IMPERSONATE_HEADER_KEY, "bob"),
            ContentType = "application/xml",
            Body = [1]
        }, async);

        AssertHeaders(new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "request" },
            { "Accept", "application/xml" },
            { "User-Agent", "custom-agent" },
            { "X-Redmine-API-Key", "secret" },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "bob" },
            { "Content-Type", "application/xml" }
        }, Assert.Single(server.Requests));
    }

    private sealed class RecordingProgress : IProgress<int>
    {
        private readonly List<int> _values = [];

        public int[] Values
        {
            get
            {
                lock (_values)
                {
                    return _values.ToArray();
                }
            }
        }

        public void Report(int value)
        {
            lock (_values)
            {
                _values.Add(value);
            }
        }
    }
}

[Collection(Constants.DeserializeCollection)]
public sealed class WebClientTransportContractTests(XmlPrettyPrintFixture _) : RedmineTransportContractTests, IClassFixture<XmlPrettyPrintFixture>
{
    internal override IRedmineTransport CreateTransport(string baseUrl, TimeSpan? timeout = null, Dictionary<string, string> defaultHeaders = null, string userAgent = null)
    {
        var options = new RedmineManagerOptionsBuilder()
            .WithHost(baseUrl)
            .WithWebClientOptions(new RedmineWebClientOptions { Timeout = timeout, DefaultHeaders = defaultHeaders, UserAgent = userAgent })
            .Build();

        return new WebClientTransport(options);
    }
}
