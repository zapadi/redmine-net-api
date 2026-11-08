using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

#pragma warning disable SYSLIB0014
namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class CustomTransportTests
{
    private const string BaseAddress = "http://localhost:3000/";
    private const string ProjectXml = "<project><id>1</id><name>ţ</name><identifier>p</identifier></project>";

    private sealed class FakeTransport(Func<RedmineTransportRequest, RedmineTransportResponse> responder) : IRedmineTransport
    {
        private readonly ConcurrentQueue<RedmineTransportRequest> _requests = new();

        public RedmineTransportRequest[] Requests => _requests.ToArray();

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            _requests.Enqueue(request);
            return responder(request);
        }

        public Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }

    private sealed class HangingTransport : IRedmineTransport
    {
        public int Calls;

        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            throw new NotSupportedException();
        }

        public async Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref Calls);
            Entered.TrySetResult(true);
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new RedmineTransportResponse(200, null, null);
        }
    }

    private sealed class RecordingProgress : IProgress<int>
    {
        public void Report(int value)
        {
        }
    }

    private static RedmineTransportResponse Xml(string xml, int statusCode = 200, string contentType = "application/xml; charset=utf-8")
    {
        return new RedmineTransportResponse(statusCode, new NameValueCollection { { "Content-Type", contentType } }, Encoding.UTF8.GetBytes(xml));
    }

    private static RedmineManager CreateManager(IRedmineTransport transport, Action<RedmineManagerOptionsBuilder> configure = null)
    {
        var builder = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).WithApiKeyAuthentication("secret").WithTransport(transport);
        configure?.Invoke(builder);
        return new RedmineManager(builder);
    }

    private static async Task<RedmineApiException> CatchAsync(bool async, Action sync, Func<Task> asyncCall)
    {
        return async
            ? await Assert.ThrowsAsync<RedmineApiException>(asyncCall)
            : Assert.Throws<RedmineApiException>(sync);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Verbs_Should_Be_Sent_Through_Custom_Transport(bool async)
    {
        var data = new byte[] { 0xFF, 0xFE, 0x00, 0x81 };
        var transport = new FakeTransport(request => request switch
        {
            { IsDownload: true } => new RedmineTransportResponse(200, null, data),
            { Method: "POST", ContentType: "application/octet-stream" } => Xml("<upload><token>t</token></upload>", 201),
            { Method: "PUT" or "DELETE" } => new RedmineTransportResponse(204, null, null),
            { Method: "POST" } => Xml(ProjectXml, 201),
            _ => Xml(ProjectXml),
        });
        var manager = CreateManager(transport);
        var progress = new RecordingProgress();
        var project = new Project { Name = "p", Identifier = "p" };
        var ct = TestContext.Current.CancellationToken;

        Project got, created;
        Upload upload;
        byte[] downloaded;
        if (async)
        {
            got = await manager.GetAsync<Project>("1", cancellationToken: ct);
            created = await manager.CreateAsync(project, cancellationToken: ct);
            await manager.UpdateAsync("1", project, cancellationToken: ct);
            await manager.DeleteAsync<Project>("1", cancellationToken: ct);
            upload = await manager.UploadFileAsync(data, cancellationToken: ct);
            downloaded = await manager.DownloadFileAsync("attachments/download/1/a.bin", progress: progress, cancellationToken: ct);
        }
        else
        {
            got = manager.Get<Project>("1");
            created = manager.Create(project);
            manager.Update("1", project);
            manager.Delete<Project>("1");
            upload = manager.UploadFile(data);
            downloaded = manager.DownloadFile("attachments/download/1/a.bin", progress: progress);
        }

        Assert.Equal("ţ", got.Name);
        Assert.Equal("ţ", created.Name);
        Assert.Equal("t", upload.Token);
        Assert.Equal(data, downloaded);

        var requests = transport.Requests;
        Assert.Equal(6, requests.Length);
        Assert.Equal(["GET", "POST", "PUT", "DELETE", "POST", "GET"], requests.Select(r => r.Method));
        Assert.Equal(
            [BaseAddress + "projects/1.xml", BaseAddress + "projects.xml", BaseAddress + "projects/1.xml", BaseAddress + "projects/1.xml", BaseAddress + "uploads.xml", BaseAddress + "attachments/download/1/a.bin"],
            requests.Select(r => r.Uri.AbsoluteUri));
        Assert.All(requests, r => Assert.True(r.Uri.IsAbsoluteUri));
        Assert.Equal([null, "application/xml", "application/xml", null, "application/octet-stream", null], requests.Select(r => r.ContentType));
        Assert.Contains("<name>p</name>", Encoding.UTF8.GetString(requests[1].Body));
        Assert.Contains("<name>p</name>", Encoding.UTF8.GetString(requests[2].Body));
        Assert.Null(requests[3].Body);
        Assert.Equal(data, requests[4].Body);
        Assert.Equal([false, false, false, false, false, true], requests.Select(r => r.IsDownload));
        Assert.Same(progress, requests[5].Progress);
        Assert.All(requests.Take(5), r => Assert.Null(r.Progress));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Paging_Should_Fetch_All_Pages(bool async)
    {
        const int total = 60;
        var transport = new FakeTransport(request =>
        {
            var match = Regex.Match(request.Uri.Query, "[?&]offset=([0-9]+)");
            var offset = match.Success ? int.Parse(match.Groups[1].Value) : 0;
            var items = string.Concat(Enumerable.Range(offset + 1, Math.Min(25, total - offset)).Select(id => $"<issue><id>{id}</id></issue>"));
            return Xml($"<issues type=\"array\" total_count=\"{total}\" offset=\"{offset}\" limit=\"25\">{items}</issues>");
        });
        var manager = CreateManager(transport);

        var issues = async
            ? await manager.GetAsync<Issue>(cancellationToken: TestContext.Current.CancellationToken)
            : manager.Get<Issue>();

        Assert.Equal(Enumerable.Range(1, total), issues.Select(i => i.Id).OrderBy(id => id));
        Assert.Equal(3, transport.Requests.Length);
        Assert.All(transport.Requests, r => Assert.Contains("limit=25", r.Uri.Query, StringComparison.Ordinal));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Headers_Should_Contain_Authentication_Impersonation_And_Request_Options()
    {
        var transport = new FakeTransport(_ => Xml(ProjectXml));
        var manager = CreateManager(transport);

        manager.Get<Project>("1", new RequestOptions
        {
            ImpersonateUser = "bob",
            Accept = "application/xml",
            UserAgent = "agent",
            Headers = new Dictionary<string, string> { { "X-Custom", "value" } },
            QueryString = new NameValueCollection { { "include", "trackers" } },
        });

        var request = Assert.Single(transport.Requests);
        Assert.Equal(BaseAddress + "projects/1.xml?include=trackers", request.Uri.AbsoluteUri);
        Assert.Equal("secret", request.Headers["X-Redmine-API-Key"]);
        Assert.Equal("bob", request.Headers[RedmineConstants.IMPERSONATE_HEADER_KEY]);
        Assert.Equal("application/xml", request.Headers["Accept"]);
        Assert.Equal("agent", request.Headers["User-Agent"]);
        Assert.Equal("value", request.Headers["X-Custom"]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Library_Headers_Should_Override_Request_Option_Headers_With_A_Single_Value()
    {
        var transport = new FakeTransport(_ => Xml(ProjectXml));
        var manager = CreateManager(transport);

        manager.Get<Project>("1", new RequestOptions
        {
            ImpersonateUser = "bob",
            Accept = "application/xml",
            UserAgent = "agent",
            Headers = new Dictionary<string, string>
            {
                { "x-redmine-api-key", "other" },
                { "ACCEPT", "text/plain" },
                { "user-agent", "other-agent" },
                { RedmineConstants.IMPERSONATE_HEADER_KEY, "alice" },
            },
        });

        var headers = Assert.Single(transport.Requests).Headers;
        Assert.Equal("secret", Assert.Single(headers.GetValues("X-Redmine-API-Key")));
        Assert.Equal("application/xml", Assert.Single(headers.GetValues("Accept")));
        Assert.Equal("agent", Assert.Single(headers.GetValues("User-Agent")));
        Assert.Equal("bob", Assert.Single(headers.GetValues(RedmineConstants.IMPERSONATE_HEADER_KEY)));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Basic_Authentication_Should_Set_Authorization_Header()
    {
        var transport = new FakeTransport(_ => Xml(ProjectXml));
        var manager = CreateManager(transport, b => b.WithBasicAuthentication("user", "pass"));

        manager.Get<Project>("1", new RequestOptions { Headers = new Dictionary<string, string> { { "authorization", "Bearer other" } } });

        Assert.Equal("Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass")), Assert.Single(Assert.Single(transport.Requests).Headers.GetValues("Authorization")));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, 404, "The requested resource was not found.", false)]
    [InlineData(true, 404, "The requested resource was not found.", false)]
    [InlineData(false, 503, "ServiceUnavailable", true)]
    [InlineData(true, 503, "ServiceUnavailable", true)]
    public async Task Error_Status_Should_Map_To_Exception(bool async, int statusCode, string message, bool isTransient)
    {
        var headers = new NameValueCollection { { "X-Request-Id", "abc" } };
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(statusCode, headers, Encoding.UTF8.GetBytes("ignored"))));

        var exception = await CatchAsync(async,
            () => manager.Get<Project>("1"),
            () => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(message, exception.Message);
        Assert.Equal(statusCode, exception.HttpStatusCode);
        Assert.Equal(isTransient, exception.IsTransient);
        Assert.Equal("GET", exception.Method);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
        Assert.Equal("abc", exception.CorrelationId);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, "utf-8")]
    [InlineData(true, "iso-8859-1")]
    public async Task Unprocessable_Entity_Should_Parse_Errors_With_Charset(bool async, string charset)
    {
        var body = Encoding.GetEncoding(charset).GetBytes("<errors type=\"array\"><error>Name café</error></errors>");
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(422, new NameValueCollection { { "Content-Type", "application/xml; charset=" + charset } }, body)));
        var project = new Project { Name = "p", Identifier = "p" };

        var exception = await CatchAsync(async,
            () => manager.Update("1", project),
            () => manager.UpdateAsync("1", project, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Name café", exception.Message, ignoreLineEndingDifferences: true);
        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("PUT", exception.Method);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_Should_Expose_Parsed_Errors(bool async)
    {
        var body = Encoding.UTF8.GetBytes("<errors type=\"array\"><error>Name cannot be blank</error><error>Identifier is too long</error></errors>");
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(422, null, body)));
        var project = new Project { Name = "p", Identifier = "p" };

        var exception = await CatchAsync(async,
            () => manager.Update("1", project),
            () => manager.UpdateAsync("1", project, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(["Name cannot be blank", "Identifier is too long"], exception.Errors);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_Thrown_By_Transport_Should_Keep_Errors_When_Enriched(bool async)
    {
        var thrown = RedmineApiExceptionHelper.CreateUnprocessableEntityException(
            "<errors type=\"array\"><error>Name cannot be blank</error><error>Identifier is too long</error></errors>", null, new XmlRedmineSerializer());
        var manager = CreateManager(new FakeTransport(_ => throw thrown));
        var project = new Project { Name = "p", Identifier = "p" };

        var exception = await CatchAsync(async,
            () => manager.Update("1", project),
            () => manager.UpdateAsync("1", project, cancellationToken: TestContext.Current.CancellationToken));

        Assert.NotSame(thrown, exception);
        Assert.Equal("PUT", exception.Method);
        Assert.Equal(["Name cannot be blank", "Identifier is too long"], exception.Errors);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_With_Long_Unparsable_Body_Should_Cap_The_Message_At_1024_Characters(bool async)
    {
        var body = Encoding.UTF8.GetBytes("  " + new string('x', 2000) + "\n");
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(422, null, body)));
        var project = new Project { Name = "p", Identifier = "p" };

        var exception = await CatchAsync(async,
            () => manager.Update("1", project),
            () => manager.UpdateAsync("1", project, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(new string('x', 1024), exception.Message);
        Assert.Equal(422, exception.HttpStatusCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(404, "")]
    [InlineData(422, "not xml")]
    public void Errors_Should_Be_Empty_When_Not_Parsed_From_A_422_Response(int statusCode, string body)
    {
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(statusCode, null, Encoding.UTF8.GetBytes(body))));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Empty(exception.Errors);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, SerializationType.Xml)]
    [InlineData(true, SerializationType.Xml)]
    [InlineData(false, SerializationType.Json)]
    [InlineData(true, SerializationType.Json)]
    public async Task Unprocessable_Entity_With_Unparsable_Body_Should_Use_The_Trimmed_Body_As_Message(bool async, SerializationType serializationType)
    {
        var body = Encoding.UTF8.GetBytes("  Validation failed: name is invalid\n");
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(422, new NameValueCollection { { "X-Request-Id", "abc" } }, body)), builder => builder.WithSerializationType(serializationType));
        var project = new Project { Name = "p", Identifier = "p" };
        var extension = serializationType == SerializationType.Xml ? "xml" : "json";

        var exception = await CatchAsync(async,
            () => manager.Update("1", project),
            () => manager.UpdateAsync("1", project, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Validation failed: name is invalid", exception.Message);
        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("PUT", exception.Method);
        Assert.Equal($"{BaseAddress}projects/1.{extension}", exception.Endpoint);
        Assert.Equal("abc", exception.CorrelationId);
        Assert.IsType<RedmineException>(exception.InnerException);
        Assert.IsNotType<RedmineException>(exception.InnerException?.InnerException);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData("application/xml; charset=utf-8", "utf-8", false)]
    [InlineData("application/xml; charset=\"ISO-8859-1\"", "iso-8859-1", false)]
    [InlineData("application/xml", "utf-8", false)]
    [InlineData(null, "utf-8", false)]
    [InlineData("application/xml; charset=unknown-charset", "utf-8", false)]
#if NET5_0_OR_GREATER
    [InlineData("application/xml; charset=utf-7", "utf-8", false)]
#endif
    [InlineData("application/xml", "utf-8", true)]
    [InlineData("application/xml; charset=iso-8859-1", "utf-8", true)]
    [InlineData("application/xml; charset=utf-8", "utf-16", true)]
    [InlineData("application/xml", "utf-16BE", true)]
    public void Text_Should_Be_Decoded_With_Charset_Or_Utf8(string contentType, string encoding, bool withBom)
    {
        var body = Encoding.GetEncoding(encoding).GetBytes("<project><id>1</id><name>café</name></project>");
        if (withBom)
        {
            body = [.. Encoding.GetEncoding(encoding).GetPreamble(), .. body];
        }

        var headers = contentType == null ? null : new NameValueCollection { { "Content-Type", contentType } };
        var manager = CreateManager(new FakeTransport(_ => new RedmineTransportResponse(200, headers, body)));

        var project = manager.Get<Project>("1");

        Assert.Equal("café", project.Name);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RedmineApiException_From_Transport_Should_Be_Enriched(bool async)
    {
        var inner = new IOException("inner");
        var thrown = new RedmineApiException("boom", inner, "E1", true);
        thrown.Data["attempt"] = 3;
        var manager = CreateManager(new FakeTransport(_ => throw thrown));

        var exception = await CatchAsync(async,
            () => manager.Delete<Project>("1"),
            () => manager.DeleteAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("boom", exception.Message);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal("E1", exception.ErrorCode);
        Assert.Equal(3, exception.Data["attempt"]);
        Assert.True(exception.IsTransient);
        Assert.Null(exception.HttpStatusCode);
        Assert.Equal("DELETE", exception.Method);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void RedmineApiException_From_Transport_Should_Keep_Status_Code()
    {
        var manager = CreateManager(new FakeTransport(_ => throw new RedmineApiException("limited", httpStatusCode: 429, isTransient: true)));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal(429, exception.HttpStatusCode);
        Assert.True(exception.IsTransient);
        Assert.Equal("GET", exception.Method);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Other_Exceptions_From_Transport_Should_Be_Wrapped(bool async)
    {
        var error = new IOException("broken");
        var manager = CreateManager(new FakeTransport(_ => throw error));

        var exception = await CatchAsync(async,
            () => manager.Get<Project>("1"),
            () => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("broken", exception.Message);
        Assert.Same(error, exception.InnerException);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.False(exception.IsTransient);
        Assert.Equal("GET", exception.Method);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Timeout_From_Transport_Should_Map_Like_Built_In_Timeout(bool async)
    {
        var timeout = new TimeoutException("slow");
        var manager = CreateManager(new FakeTransport(_ => throw timeout));

        var exception = await CatchAsync(async,
            () => manager.Get<Project>("1"),
            () => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Timeout", exception.Message);
        Assert.Same(timeout, exception.InnerException);
        Assert.True(exception.IsTransient);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Canceled_With_Inner_Timeout_Without_Caller_Cancellation_Should_Map_To_Timeout()
    {
        var canceled = new TaskCanceledException("canceled", new TimeoutException());
        var manager = CreateManager(new FakeTransport(_ => throw canceled));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Timeout", exception.Message);
        Assert.Same(canceled, exception.InnerException);
        Assert.True(exception.IsTransient);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void WebException_From_Transport_Should_Map_Like_Built_In()
    {
        var manager = CreateManager(new FakeTransport(_ => throw new WebException("refused", WebExceptionStatus.ConnectFailure)));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal("refused", exception.Message);
        Assert.True(exception.IsTransient);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Null_Response_Should_Throw_RedmineApiException()
    {
        var manager = CreateManager(new FakeTransport(_ => null));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Equal("GET", exception.Method);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Caller_Cancellation_In_Flight_Should_Map_Like_Built_In()
    {
        var transport = new HangingTransport();
        var manager = CreateManager(transport);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);

        var call = manager.GetAsync<Project>("1", cancellationToken: cts.Token);
        await transport.Entered.Task;
        cts.Cancel();
        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => call);

        Assert.Equal("The operation was canceled by the user.", exception.Message);
        Assert.IsAssignableFrom<OperationCanceledException>(exception.InnerException);
        Assert.False(exception.IsTransient);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Equal("GET", exception.Method);
        Assert.Equal(BaseAddress + "projects/1.xml", exception.Endpoint);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Caller_Cancellation_Before_Start_Should_Throw_OperationCanceledException()
    {
        var transport = new HangingTransport();
        var manager = CreateManager(transport);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => manager.GetAsync<Project>("1", cancellationToken: cts.Token));

        Assert.Equal(0, transport.Calls);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void WithTransport_Combined_With_WithWebClient_Should_Throw_At_Build()
    {
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost(BaseAddress)
            .WithTransport(new FakeTransport(_ => null))
            .WithWebClient(() => new WebClient());

        Assert.Throws<RedmineException>(() => new RedmineManager(builder));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void WithWebClientOptions_Combined_With_WithTransport_Should_Throw_At_Build()
    {
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost(BaseAddress)
            .WithWebClientOptions(new Redmine.Net.Api.Net.WebClient.RedmineWebClientOptions())
            .WithTransport(new FakeTransport(_ => null));

        Assert.Throws<RedmineException>(() => new RedmineManager(builder));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void WithTransport_Null_Should_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RedmineManagerOptionsBuilder().WithTransport(null));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Request_Constructor_Should_Validate_Arguments()
    {
        var uri = new Uri(BaseAddress + "projects.xml");

        Assert.Throws<ArgumentNullException>(() => new RedmineTransportRequest(null, uri));
        Assert.Throws<ArgumentNullException>(() => new RedmineTransportRequest("GET", null));
        Assert.Throws<RedmineException>(() => new RedmineTransportRequest("GET", new Uri("projects.xml", UriKind.Relative)));

        var request = new RedmineTransportRequest("GET", uri);
        Assert.Empty(request.Headers);
        Assert.Null(request.ContentType);
        Assert.Null(request.Body);
        Assert.False(request.IsDownload);
        Assert.Null(request.Progress);
        Assert.Empty(new RedmineTransportRequest("GET", uri) { Headers = null }.Headers);

        var headers = new NameValueCollection { { "X-Custom", "value" } };
        var body = new byte[] { 1 };
        var progress = new RecordingProgress();
        var initialized = new RedmineTransportRequest("POST", uri) { Headers = headers, ContentType = "application/xml", Body = body, IsDownload = true, Progress = progress };
        Assert.Same(headers, initialized.Headers);
        Assert.Equal("application/xml", initialized.ContentType);
        Assert.Same(body, initialized.Body);
        Assert.True(initialized.IsDownload);
        Assert.Same(progress, initialized.Progress);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(199, false)]
    [InlineData(200, true)]
    [InlineData(299, true)]
    [InlineData(304, false)]
    [InlineData(500, false)]
    public void Response_IsSuccessStatusCode_Should_Be_True_For_2xx(int statusCode, bool expected)
    {
        Assert.Equal(expected, new RedmineTransportResponse(statusCode, null, null).IsSuccessStatusCode);
    }
}
