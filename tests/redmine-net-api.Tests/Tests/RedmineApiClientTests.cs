using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Infrastructure.Constants.DeserializeCollection)]
public sealed class RedmineApiClientTests
{
    private const string BaseAddress = "http://localhost:3000/";

    private sealed class FakeTransport(Func<RedmineTransportRequest, RedmineTransportResponse> responder) : IRedmineTransport
    {
        public List<RedmineTransportRequest> Requests { get; } = [];

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            Requests.Add(request);
            request.BaseAddress = BaseAddress;
            return responder(request);
        }

        public Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }

    private static RedmineTransportResponse Ok(string content = null, byte[] body = null, int statusCode = 200, NameValueCollection headers = null)
    {
        return new RedmineTransportResponse { IsSuccessStatusCode = true, StatusCode = statusCode, Content = content, Body = body, Headers = headers };
    }

    private static RedmineTransportResponse Error(int statusCode, byte[] body = null, NameValueCollection headers = null, Exception error = null, Exception bodyError = null)
    {
        return new RedmineTransportResponse { StatusCode = statusCode, Body = body, Headers = headers, Error = error, BodyError = bodyError };
    }

    private static (RedmineApiClient Client, FakeTransport Transport) CreateClient(Func<RedmineTransportRequest, RedmineTransportResponse> responder, IRedmineAuthentication authentication = null, SerializationType serializationType = SerializationType.Xml)
    {
        var transport = new FakeTransport(responder);
        var options = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).WithSerializationType(serializationType).Build();
        return (new RedmineApiClient(transport, authentication ?? new RedmineApiKeyAuthentication("secret"), options.Serializer), transport);
    }

    private static async Task<RedmineApiException> CatchAsync(bool async, Func<ApiResponseMessage> sync, Func<Task<ApiResponseMessage>> asyncCall)
    {
        return async
            ? await Assert.ThrowsAsync<RedmineApiException>(asyncCall)
            : Assert.Throws<RedmineApiException>(() => sync());
    }

    [Fact]
    public async Task Verbs_Should_Map_To_Transport_Requests()
    {
        var (client, transport) = CreateClient(_ => Ok());
        var data = new byte[] { 1, 2 };
        var progress = new Progress<int>();
        var ct = TestContext.Current.CancellationToken;

        client.Get("a.xml");
        client.GetPaged("b.xml");
        client.Create("c.xml", "<c/>");
        client.Update("d.xml", "<d/>");
        client.Patch("e.xml", "<e/>");
        client.Delete("f.xml");
        client.Upload("g.xml", data);
        client.Download("h.bin", progress: progress);
        await client.GetAsync("a.xml", cancellationToken: ct);
        await client.GetPagedAsync("b.xml", cancellationToken: ct);
        await client.CreateAsync("c.xml", "<c/>", cancellationToken: ct);
        await client.UpdateAsync("d.xml", "<d/>", cancellationToken: ct);
        await client.PatchAsync("e.xml", "<e/>", cancellationToken: ct);
        await client.DeleteAsync("f.xml", cancellationToken: ct);
        await client.UploadFileAsync("g.xml", data, cancellationToken: ct);
        await client.DownloadAsync("h.bin", progress: progress, cancellationToken: ct);

        var expected = new (string Method, string Address, string ContentType, byte[] Body)[]
        {
            ("GET", "a.xml", null, null),
            ("GET", "b.xml", null, null),
            ("POST", "c.xml", "application/xml", Encoding.UTF8.GetBytes("<c/>")),
            ("PUT", "d.xml", "application/xml", Encoding.UTF8.GetBytes("<d/>")),
            ("PATCH", "e.xml", "application/xml", Encoding.UTF8.GetBytes("<e/>")),
            ("DELETE", "f.xml", null, null),
            ("POST", "g.xml", "application/octet-stream", data),
            ("DOWNLOAD", "h.bin", null, null),
        };
        Assert.Equal(16, transport.Requests.Count);
        for (var i = 0; i < transport.Requests.Count; i++)
        {
            var request = transport.Requests[i];
            var item = expected[i % expected.Length];
            Assert.Equal(item.Method, request.Method);
            Assert.Equal(item.Address, request.Address);
            Assert.Equal(item.ContentType, request.ContentType);
            Assert.Equal(item.Body, request.Body);
            Assert.Null(request.QueryString);
            Assert.Equal(item.Method == "DOWNLOAD" ? progress : null, request.Progress);
        }
    }

    [Fact]
    public void Create_With_Json_Serializer_Should_Use_Json_Content_Type()
    {
        var (client, transport) = CreateClient(_ => Ok(), serializationType: SerializationType.Json);

        client.Create("c.json", "{}");

        Assert.Equal("application/json", transport.Requests[0].ContentType);
    }

    [Fact]
    public void Upload_With_Null_Data_Should_Keep_Content_Type_And_Null_Body()
    {
        var (client, transport) = CreateClient(_ => Ok());

        client.Upload("g.xml", null);

        Assert.Equal("application/octet-stream", transport.Requests[0].ContentType);
        Assert.Null(transport.Requests[0].Body);
    }

    [Fact]
    public void Headers_Should_Be_Resolved_With_Library_Headers_Winning()
    {
        var (client, transport) = CreateClient(_ => Ok());
        var queryString = new NameValueCollection { { "a", "1" } };

        client.Create("c.xml", "<c/>", new RequestOptions
        {
            QueryString = queryString,
            Accept = "application/xml",
            UserAgent = "custom-agent",
            ContentType = "application/custom+xml",
            ImpersonateUser = "bob",
            Headers = new Dictionary<string, string>
            {
                { "x-custom", "request" },
                { "accept", "text/html" },
                { "User-Agent", "header-agent" },
                { "X-Redmine-API-Key", "other" },
                { RedmineConstants.IMPERSONATE_HEADER_KEY, "alice" },
                { "Content-Type", "text/html" }
            }
        });

        var request = transport.Requests[0];
        Assert.Same(queryString, request.QueryString);
        Assert.Equal("application/custom+xml", request.ContentType);
        Assert.Equal(["x-custom", "accept", "User-Agent", "X-Redmine-API-Key", RedmineConstants.IMPERSONATE_HEADER_KEY, "Content-Type"], request.Headers.AllKeys);
        Assert.Equal(["request"], request.Headers.GetValues("X-Custom"));
        Assert.Equal(["text/html", "application/xml"], request.Headers.GetValues("Accept"));
        Assert.Equal(["header-agent", "custom-agent"], request.Headers.GetValues("User-Agent"));
        Assert.Equal(["other", "secret"], request.Headers.GetValues("X-Redmine-API-Key"));
        Assert.Equal(["alice", "bob"], request.Headers.GetValues(RedmineConstants.IMPERSONATE_HEADER_KEY));
        Assert.Equal(["text/html"], request.Headers.GetValues("Content-Type"));
    }

    [Fact]
    public void Get_Should_Ignore_RequestOptions_ContentType_And_Whitespace_Values()
    {
        var (client, transport) = CreateClient(_ => Ok());

        client.Get("a.xml", new RequestOptions { ContentType = "application/custom+xml", Accept = " ", UserAgent = "", ImpersonateUser = " " });

        var request = transport.Requests[0];
        Assert.Null(request.ContentType);
        Assert.Equal(["X-Redmine-API-Key"], request.Headers.AllKeys);
    }

    [Fact]
    public void Basic_Authentication_Should_Set_Authorization_Header()
    {
        var (client, transport) = CreateClient(_ => Ok(), new RedmineBasicAuthentication("user", "pass"));

        client.Get("a.xml", new RequestOptions { Headers = new Dictionary<string, string> { { "Authorization", "Bearer other" } } });

        Assert.Equal(["Bearer other", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass"))], transport.Requests[0].Headers.GetValues("Authorization"));
    }

    [Fact]
    public void No_Authentication_Should_Not_Add_Headers()
    {
        var (client, transport) = CreateClient(_ => Ok(), new RedmineNoAuthentication());

        client.Get("a.xml");

        Assert.Empty(transport.Requests[0].Headers);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Success_Should_Map_To_Response_Message(bool async)
    {
        var headers = new NameValueCollection { { "X-Response", "value" } };
        var (client, _) = CreateClient(_ => Ok("content", [1], 201, headers));

        var response = async
            ? await client.GetAsync("a.xml", cancellationToken: TestContext.Current.CancellationToken)
            : client.Get("a.xml");

        Assert.True(response.IsSuccessful);
        Assert.Equal(201, response.StatusCode);
        Assert.Same(headers, response.Headers);
        Assert.Equal("content", response.Content);
        Assert.Equal(new byte[] { 1 }, response.RawContent);
        Assert.Null(response.EndPoint);
    }

    [Theory]
    [InlineData(false, 404, "The requested resource was not found.", false)]
    [InlineData(true, 401, "Authentication is required or has failed.", false)]
    [InlineData(false, 403, "You don't have permission to access this resource.", false)]
    [InlineData(true, 406, "The requested format is not supported.", false)]
    [InlineData(false, 409, "The resource you are trying to update has been modified since you last retrieved it.", false)]
    [InlineData(true, 500, "The server encountered an unexpected error.", false)]
    [InlineData(false, 502, "BadGateway", true)]
    [InlineData(true, 304, "NotModified", false)]
    public async Task Error_Status_Should_Map_To_Exception(bool async, int statusCode, string message, bool isTransient)
    {
        var error = new WebException("protocol", WebExceptionStatus.ProtocolError);
        var headers = new NameValueCollection { { "X-Request-Id", "abc" } };
        var (client, _) = CreateClient(_ => Error(statusCode, [], headers, error));

        var exception = await CatchAsync(async,
            () => client.Download("h.bin"),
            () => client.DownloadAsync("h.bin", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(message, exception.Message);
        Assert.Equal(statusCode, exception.HttpStatusCode);
        Assert.Equal(isTransient, exception.IsTransient);
        Assert.Null(exception.ErrorCode);
        Assert.Equal("GET", exception.Method);
        Assert.Equal(BaseAddress + "h.bin", exception.Endpoint);
        Assert.Equal("abc", exception.CorrelationId);
        Assert.Equal("abc", exception.ResponseHeaders["x-request-id"]);
        Assert.Same(error, exception.InnerException);
    }

    [Fact]
    public void Error_Status_Should_Use_Inner_Exception_Of_Transport_Error()
    {
        var inner = new InvalidOperationException("inner");
        var (client, _) = CreateClient(_ => Error(404, error: new WebException("protocol", inner)));

        var exception = Assert.Throws<RedmineApiException>(() => client.Get("a.xml"));

        Assert.Same(inner, exception.InnerException);
        Assert.Null(exception.ResponseHeaders);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_Should_Parse_Errors(bool async)
    {
        var body = Encoding.UTF8.GetPreamble();
        body = [.. body, .. Encoding.UTF8.GetBytes("<errors type=\"array\"><error>Name ţ</error></errors>")];
        var (client, _) = CreateClient(_ => Error(422, body, new NameValueCollection(), new WebException("protocol")));

        var exception = await CatchAsync(async,
            () => client.Update("d.xml", "<d/>"),
            () => client.UpdateAsync("d.xml", "<d/>", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Name ţ", exception.Message);
        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("PUT", exception.Method);
        Assert.Equal(BaseAddress + "d.xml", exception.Endpoint);
        Assert.Null(exception.InnerException);
        Assert.Null(exception.ErrorCode);
    }

    [Fact]
    public void Unprocessable_Entity_With_Empty_Body_Should_Use_Default_Message()
    {
        var (client, _) = CreateClient(_ => Error(422, []));

        var exception = Assert.Throws<RedmineApiException>(() => client.Create("c.xml", "<c/>"));

        Assert.Equal("Validation failed for the submitted data.", exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Unprocessable_Entity_Without_Body_Should_Map_Status_Code()
    {
        var error = new WebException("protocol");
        var (client, _) = CreateClient(_ => Error(422, error: error));

        var exception = Assert.Throws<RedmineApiException>(() => client.Create("c.xml", "<c/>"));

        Assert.Equal(((HttpStatusCode)422).ToString(), exception.Message);
        Assert.Same(error, exception.InnerException);
    }

    [Fact]
    public void Unprocessable_Entity_With_Body_Read_Error_Should_Throw_With_Unknown_ErrorCode()
    {
        var bodyError = new System.IO.IOException("read failed");
        var (client, _) = CreateClient(_ => Error(422, bodyError: bodyError, error: new WebException("protocol")));

        var exception = Assert.Throws<RedmineApiException>(() => client.Create("c.xml", "<c/>"));

        Assert.Equal("read failed", exception.Message);
        Assert.Same(bodyError, exception.InnerException);
        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Fact]
    public void Body_Read_Error_Should_Be_Ignored_For_Other_Status_Codes()
    {
        var (client, _) = CreateClient(_ => Error(404, bodyError: new System.IO.IOException("read failed")));

        var exception = Assert.Throws<RedmineApiException>(() => client.Get("a.xml"));

        Assert.Equal("The requested resource was not found.", exception.Message);
    }

    [Theory]
    [InlineData(false, WebExceptionStatus.ConnectFailure, true)]
    [InlineData(true, WebExceptionStatus.ConnectFailure, true)]
    [InlineData(false, WebExceptionStatus.NameResolutionFailure, false)]
    [InlineData(true, WebExceptionStatus.ReceiveFailure, true)]
    [InlineData(false, WebExceptionStatus.Timeout, true)]
    [InlineData(true, WebExceptionStatus.RequestCanceled, false)]
    public async Task Transport_WebException_Should_Map_To_Exception(bool async, WebExceptionStatus status, bool isTransient)
    {
        var inner = new InvalidOperationException("inner");
        var webException = new WebException("failure", inner, status, null);
        var (client, _) = CreateClient(r =>
        {
            r.BaseAddress = BaseAddress;
            throw webException;
        });

        var exception = await CatchAsync(async,
            () => client.Delete("f.xml"),
            () => client.DeleteAsync("f.xml", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(status == WebExceptionStatus.Timeout ? "Timeout" : "failure", exception.Message);
        Assert.Equal(isTransient, exception.IsTransient);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Null(exception.HttpStatusCode);
        Assert.Null(exception.ResponseHeaders);
        Assert.Equal("DELETE", exception.Method);
        Assert.Equal(BaseAddress + "f.xml", exception.Endpoint);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Transport_WebException_Without_BaseAddress_Should_Use_Address_As_Endpoint()
    {
        var transport = new ThrowingTransport(new WebException("failure"));
        var client = new RedmineApiClient(transport, new RedmineNoAuthentication(), null);

        var exception = Assert.Throws<RedmineApiException>(() => client.Get("a.xml"));

        Assert.Equal("a.xml", exception.Endpoint);
        Assert.IsType<WebException>(exception.InnerException);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Transport_Timeout_WebException_Should_Map_To_Timeout(bool withInner)
    {
        var inner = new WebException("canceled", WebExceptionStatus.RequestCanceled);
        var timeout = new WebException(nameof(WebExceptionStatus.Timeout), withInner ? inner : null, WebExceptionStatus.Timeout, null);
        var client = new RedmineApiClient(new ThrowingTransport(timeout, BaseAddress), new RedmineNoAuthentication(), null);

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => client.GetAsync("a.xml", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("Timeout", exception.Message);
        Assert.True(exception.IsTransient);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Null(exception.HttpStatusCode);
        Assert.Equal(BaseAddress + "a.xml", exception.Endpoint);
        Assert.Same(withInner ? inner : timeout, exception.InnerException);
    }

    [Fact]
    public async Task Transport_OperationCanceledException_With_Inner_Should_Map_To_Canceled_By_User()
    {
        var inner = new WebException("canceled", WebExceptionStatus.RequestCanceled);
        var client = new RedmineApiClient(new ThrowingTransport(new OperationCanceledException("c", inner), BaseAddress), new RedmineNoAuthentication(), null);

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => client.DownloadAsync("h.bin", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("The operation was canceled by the user.", exception.Message);
        Assert.False(exception.IsTransient);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Null(exception.HttpStatusCode);
        Assert.Null(exception.ResponseHeaders);
        Assert.Equal("GET", exception.Method);
        Assert.Equal(BaseAddress + "h.bin", exception.Endpoint);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public async Task Transport_OperationCanceledException_Without_Inner_Should_Propagate()
    {
        var canceled = new OperationCanceledException();
        var client = new RedmineApiClient(new ThrowingTransport(canceled), new RedmineNoAuthentication(), null);

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => client.GetAsync("a.xml", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Same(canceled, exception);
    }

    [Fact]
    public void Other_Transport_Exceptions_Should_Propagate()
    {
        var error = new RedmineException("boom");
        var client = new RedmineApiClient(new ThrowingTransport(error), new RedmineNoAuthentication(), null);

        Assert.Same(error, Assert.Throws<RedmineException>(() => client.Get("a.xml")));
    }

    private sealed class ThrowingTransport(Exception exception, string baseAddress = null) : IRedmineTransport
    {
        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            request.BaseAddress = baseAddress;
            throw exception;
        }

        public Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            request.BaseAddress = baseAddress;
            throw exception;
        }
    }
}
