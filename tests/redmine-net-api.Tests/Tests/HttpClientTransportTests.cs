using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class HttpClientTransportTests : IDisposable
{
    private const string ProjectXml = "<project><id>1</id><name>ţ</name><identifier>p</identifier></project>";
    private const string Errors = """<errors type="array"><error>Name cannot be blank</error></errors>""";

    private readonly List<IDisposable> _disposables = [];

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }

    private TestHttpServer Server(Func<RecordedRequest, TestHttpResponse> handler)
    {
        var server = new TestHttpServer(handler);
        _disposables.Add(server);
        return server;
    }

    private RedmineManager Manager(string url, TimeSpan? timeout = null, Action<HttpClient> configure = null)
    {
        var client = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(10) };
        _disposables.Add(client);
        configure?.Invoke(client);
        return new RedmineManager(new RedmineManagerOptionsBuilder()
            .WithHost(url)
            .WithApiKeyAuthentication("secret")
            .WithHttpClient(client));
    }

    private static TestHttpResponse Xml(string xml, int statusCode = 200)
    {
        return new TestHttpResponse { StatusCode = statusCode, ContentType = "application/xml; charset=utf-8", Body = Encoding.UTF8.GetBytes(xml) };
    }

    private static async Task Bounded(Task task)
    {
        if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken)) != task)
        {
            throw new TimeoutException("Request did not complete.");
        }

        await task;
    }

    private static async Task<T> Bounded<T>(Task<T> task)
    {
        await Bounded((Task)task);
        return await task;
    }

    private static async Task<RedmineApiException> CatchAsync(bool async, Action sync, Func<Task> asyncCall)
    {
        return async
            ? await Bounded(Assert.ThrowsAsync<RedmineApiException>(asyncCall))
            : Assert.Throws<RedmineApiException>(sync);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Constructor_With_Null_HttpClient_Should_Throw_ArgumentNullException()
    {
        Assert.Equal("httpClient", Assert.Throws<ArgumentNullException>(() => new HttpClientTransport(null)).ParamName);
        Assert.Equal("httpClient", Assert.Throws<ArgumentNullException>(() => new RedmineManagerOptionsBuilder().WithHttpClient(null)).ParamName);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void WithHttpClient_Should_Not_Be_Combined_With_WebClientOptions()
    {
        using var client = new HttpClient();
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost("http://localhost:3000/")
            .WithHttpClient(client)
            .WithWebClientOptions(new Redmine.Net.Api.Net.WebClient.RedmineWebClientOptions());

        Assert.Throws<RedmineException>(() => new RedmineManager(builder));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Crud_Should_Be_Sent_Through_HttpClient(bool async)
    {
        var server = Server(request => request.Method switch
        {
            "POST" => Xml(ProjectXml, 201),
            "PUT" or "DELETE" => new TestHttpResponse { StatusCode = 204 },
            _ => Xml(ProjectXml),
        });
        var manager = Manager(server.Url);
        var project = new Project { Name = "p", Identifier = "p" };
        var ct = TestContext.Current.CancellationToken;

        Project got, created;
        if (async)
        {
            got = await Bounded(manager.GetAsync<Project>("1", cancellationToken: ct));
            created = await Bounded(manager.CreateAsync(project, cancellationToken: ct));
            await Bounded(manager.UpdateAsync("1", project, cancellationToken: ct));
            await Bounded(manager.DeleteAsync<Project>("1", cancellationToken: ct));
        }
        else
        {
            got = manager.Get<Project>("1");
            created = manager.Create(project);
            manager.Update("1", project);
            manager.Delete<Project>("1");
        }

        Assert.Equal("ţ", got.Name);
        Assert.Equal("ţ", created.Name);
        var requests = server.Requests;
        Assert.Equal(["GET", "POST", "PUT", "DELETE"], requests.Select(r => r.Method));
        Assert.Equal(["/projects/1.xml", "/projects.xml", "/projects/1.xml", "/projects/1.xml"], requests.Select(r => r.RawUrl));
        Assert.All(requests, r => Assert.Equal("secret", r.Headers["X-Redmine-API-Key"]));
        Assert.Equal("application/xml", requests[1].Headers["Content-Type"]);
        Assert.Contains("<name>p</name>", Encoding.UTF8.GetString(requests[1].Body), StringComparison.Ordinal);
        Assert.Contains("<name>p</name>", Encoding.UTF8.GetString(requests[2].Body), StringComparison.Ordinal);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Paging_Should_Fetch_All_Pages(bool async)
    {
        const int total = 60;
        var server = Server(request =>
        {
            var match = Regex.Match(request.RawUrl, "[?&]offset=([0-9]+)");
            var offset = match.Success ? int.Parse(match.Groups[1].Value) : 0;
            var items = string.Concat(Enumerable.Range(offset + 1, Math.Max(0, Math.Min(25, total - offset))).Select(id => $"<issue><id>{id}</id></issue>"));
            return Xml($"<issues type=\"array\" total_count=\"{total}\" offset=\"{offset}\" limit=\"25\">{items}</issues>");
        });
        var manager = Manager(server.Url);

        var issues = async
            ? await Bounded(manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken))
            : manager.Get<Issue>();

        Assert.Equal(Enumerable.Range(1, total), issues.Select(i => i.Id).OrderBy(id => id));
        Assert.Equal(3, server.Requests.Count);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Download_Should_Return_Bytes_And_Report_Progress(bool async)
    {
        var data = new byte[256 * 1024];
        new Random(1).NextBytes(data);
        var server = Server(_ => new TestHttpResponse { ContentType = "application/octet-stream", Body = data });
        var manager = Manager(server.Url);
        var progress = new RecordingProgress();

        var downloaded = async
            ? await Bounded(manager.DownloadFileAsync("attachments/download/1/a.bin", progress: progress, cancellationToken: TestContext.Current.CancellationToken))
            : manager.DownloadFile("attachments/download/1/a.bin", progress: progress);

        Assert.Equal(data, downloaded);
        Assert.Equal("/attachments/download/1/a.bin", Assert.Single(server.Requests).RawUrl);
        Assert.True(SpinWait.SpinUntil(() => progress.Values.LastOrDefault() == 100, TimeSpan.FromSeconds(5)));
        Assert.All(progress.Values, value => Assert.InRange(value, 0, 100));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, 404, false)]
    [InlineData(true, 404, false)]
    [InlineData(false, 503, true)]
    [InlineData(true, 503, true)]
    public async Task Error_Status_Should_Throw_RedmineApiException(bool async, int statusCode, bool isTransient)
    {
        var server = Server(_ => new TestHttpResponse { StatusCode = statusCode, Headers = new Dictionary<string, string> { { "X-Request-Id", "abc" } } });
        var manager = Manager(server.Url);

        var exception = await CatchAsync(async, () => manager.Get<Project>("1"), () => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(statusCode, exception.HttpStatusCode);
        Assert.Equal(isTransient, exception.IsTransient);
        Assert.Equal("GET", exception.Method);
        Assert.Equal("abc", exception.ResponseHeaders["X-Request-Id"]);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_Should_Throw_With_Validation_Message(bool async)
    {
        var server = Server(_ => Xml(Errors, 422));
        var manager = Manager(server.Url);
        var project = new Project { Name = "p", Identifier = "p" };

        var exception = await CatchAsync(async, () => manager.Create(project), () => manager.CreateAsync(project, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
        Assert.Equal("POST", exception.Method);
        Assert.False(exception.IsTransient);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HttpClient_Timeout_Should_Throw_Transient_Timeout(bool async)
    {
        var server = Server(_ => new TestHttpResponse { Delay = TimeSpan.FromSeconds(5) });
        var manager = Manager(server.Url, TimeSpan.FromMilliseconds(300));

        var exception = await CatchAsync(async, () => manager.Get<Project>("1"), () => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Timeout", exception.Message);
        Assert.True(exception.IsTransient);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Equal("GET", exception.Method);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Caller_Cancellation_Should_Throw_Canceled_By_User()
    {
        var server = Server(_ => new TestHttpResponse { Delay = TimeSpan.FromSeconds(5) });
        var manager = Manager(server.Url);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        cts.CancelAfter(TimeSpan.FromMilliseconds(300));

        var exception = await Bounded(Assert.ThrowsAsync<RedmineApiException>(() => manager.GetAsync<Project>("1", cancellationToken: cts.Token)));

        Assert.Equal("The operation was canceled by the user.", exception.Message);
        Assert.False(exception.IsTransient);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Connection_Refused_Should_Throw_Transient_Exception(bool async)
    {
        var manager = Manager($"http://localhost:{TestHttpServer.GetFreePort()}/");

        var exception = await CatchAsync(async, () => manager.Get<Project>("1"), () => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.True(exception.IsTransient);
        Assert.Null(exception.HttpStatusCode);
        Assert.Equal("GET", exception.Method);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DefaultRequestHeaders_Should_Not_Override_Request_Headers(bool async)
    {
        var server = Server(_ => Xml(ProjectXml));
        var manager = Manager(server.Url, configure: client =>
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Default", "default");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Custom", "default");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Redmine-API-Key", "default-key");
            client.DefaultRequestHeaders.TryAddWithoutValidation(RedmineConstants.IMPERSONATE_HEADER_KEY, "default-user");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/plain");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "default-agent");
        });
        var options = new RequestOptions
        {
            ImpersonateUser = "bob",
            Accept = "application/xml",
            UserAgent = "custom-agent",
            Headers = new Dictionary<string, string> { { "X-Custom", "request" }, { "X-Redmine-API-Key", "override" } },
        };

        _ = async
            ? await Bounded(manager.GetAsync<Project>("1", options, TestContext.Current.CancellationToken))
            : manager.Get<Project>("1", options);

        var headers = Assert.Single(server.Requests).Headers;
        Assert.Equal("default", headers["X-Default"]);
        Assert.Equal("request", headers["X-Custom"]);
        Assert.Equal("secret", headers["X-Redmine-API-Key"]);
        Assert.Equal("bob", headers[RedmineConstants.IMPERSONATE_HEADER_KEY]);
        Assert.Equal("application/xml", headers["Accept"]);
        Assert.Equal("custom-agent", headers["User-Agent"]);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DefaultRequestHeaders_Should_Be_Sent_When_Request_Does_Not_Set_Them(bool async)
    {
        var server = Server(_ => Xml(ProjectXml));
        var manager = Manager(server.Url, configure: client =>
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Default", "default");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Redmine-API-Key", "default-key");
        });

        _ = async
            ? await Bounded(manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken))
            : manager.Get<Project>("1");

        var headers = Assert.Single(server.Requests).Headers;
        Assert.Equal("default", headers["X-Default"]);
        Assert.Equal("secret", headers["X-Redmine-API-Key"]);
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
