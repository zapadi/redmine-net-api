using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

#pragma warning disable SYSLIB0014
namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class RedmineApiClientCharacterizationTests
{
    private const string ApiKey = "secret";
    private const string ProjectXml = "<project><id>1</id><name>foo</name></project>";
    private const string NonAscii = "<project><id>1</id><name>Ünïcödé ✓ ţară</name></project>";
    private static readonly string[] TransportHeaders = ["Host", "Connection", "Accept-Encoding", "Expect"];

    private static RedmineManager CreateManager(string url, Action<RedmineManagerOptionsBuilder> configure = null)
    {
        var builder = new RedmineManagerOptionsBuilder().WithHost(url).WithApiKeyAuthentication(ApiKey);
        configure?.Invoke(builder);
        return new RedmineManager(builder);
    }

    private static TestHttpResponse Response(string body, int statusCode = 200, string contentType = "application/xml; charset=utf-8", Dictionary<string, string> headers = null)
    {
        return new TestHttpResponse { StatusCode = statusCode, ContentType = contentType, Body = Encoding.UTF8.GetBytes(body), Headers = headers ?? new Dictionary<string, string>() };
    }

    private static Dictionary<string, string> SentHeaders(RecordedRequest request)
    {
        return request.Headers.AllKeys
            .Where(key => !TransportHeaders.Contains(key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(key => key.ToLowerInvariant(), key => request.Headers[key]);
    }

    private static void AssertHeaders(Dictionary<string, string> expected, RecordedRequest request)
    {
        var actual = SentHeaders(request);
        Assert.Equal(
            expected.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value).OrderBy(h => h.Key, StringComparer.Ordinal),
            actual.OrderBy(h => h.Key, StringComparer.Ordinal));
    }

    private static async Task<RedmineApiException> CatchAsync(bool async, Func<ApiResponseMessage> sync, Func<Task<ApiResponseMessage>> asyncCall)
    {
        return async
            ? await Assert.ThrowsAsync<RedmineApiException>(asyncCall)
            : Assert.Throws<RedmineApiException>(() => sync());
    }

    private static void AssertException(RedmineApiException exception, string message, int? statusCode, string method, string endpoint, bool isTransient, string errorCode)
    {
        Assert.Equal(message, exception.Message);
        Assert.Equal(statusCode, exception.HttpStatusCode);
        Assert.Equal(method, exception.Method);
        Assert.Equal(endpoint, exception.Endpoint);
        Assert.Equal(isTransient, exception.IsTransient);
        Assert.Equal(errorCode, exception.ErrorCode);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Decode_Utf8_Charset()
    {
        using var server = new TestHttpServer(_ => Response(NonAscii));
        var manager = CreateManager(server.Url);

        var response = manager.ApiClient.Get("projects/1.xml");

        Assert.Equal(NonAscii, response.Content);
        Assert.Null(response.RawContent);
        Assert.True(response.IsSuccessful);
        Assert.Equal(200, response.StatusCode);
        Assert.Null(response.EndPoint);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Decode_Utf8_Charset()
    {
        using var server = new TestHttpServer(_ => Response(NonAscii));
        var manager = CreateManager(server.Url);

        var response = await manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(NonAscii, response.Content);
        Assert.Null(response.RawContent);
        Assert.Equal(200, response.StatusCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Without_Charset_Should_Decode_With_WebClient_Default_Encoding(bool async)
    {
        var bytes = Encoding.UTF8.GetBytes(NonAscii);
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/xml", Body = bytes });
        var manager = CreateManager(server.Url);

        var response = async
            ? await manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Get("projects/1.xml");

        using var webClient = new WebClient();
        Assert.Equal(webClient.Encoding.GetString(bytes), response.Content);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_With_Latin1_Charset_Should_Decode_With_Charset(bool async)
    {
        var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes("<name>café</name>");
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/xml; charset=iso-8859-1", Body = bytes });
        var manager = CreateManager(server.Url);

        var response = async
            ? await manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Get("projects/1.xml");

        Assert.Equal("<name>café</name>", response.Content);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Should_Strip_Utf8_Bom(bool async)
    {
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(NonAscii)).ToArray();
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/xml", Body = bytes });
        var manager = CreateManager(server.Url);

        var response = async
            ? await manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Get("projects/1.xml");

        Assert.Equal(NonAscii, response.Content);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Should_Send_Request_And_Return_Response_Headers(bool async)
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml, headers: new Dictionary<string, string> { { "X-Response", "value" } }));
        var manager = CreateManager(server.Url);
        var requestOptions = new RequestOptions { QueryString = new NameValueCollection { { "status_id", "open" }, { "include", "a,b" }, { "name", "x y" } } };

        var response = async
            ? await manager.ApiClient.GetAsync("issues.xml", requestOptions, TestContext.Current.CancellationToken)
            : manager.ApiClient.Get("issues.xml", requestOptions);

        var request = Assert.Single(server.Requests);
        Assert.Equal("GET", request.Method);
        Assert.Equal("/issues.xml?status_id=open&include=a,b&name=x%20y", request.RawUrl);
        Assert.Empty(request.Body);
        AssertHeaders(new Dictionary<string, string> { { "User-Agent", "Redmine.Net.Api.Net" }, { "X-Redmine-API-Key", ApiKey } }, request);
        Assert.Equal("value", response.Headers["X-Response"]);
        Assert.Equal(ProjectXml, response.Content);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Create_Should_Send_Body_And_Return_Raw_Content(bool async)
    {
        using var server = new TestHttpServer(_ => Response(NonAscii, 201));
        var manager = CreateManager(server.Url);
        var payload = manager.Serializer.Serialize(new Project { Name = "Ünïcödé" });

        var response = async
            ? await manager.ApiClient.CreateAsync("projects.xml", payload, cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Create("projects.xml", payload);

        var request = Assert.Single(server.Requests);
        Assert.Equal("POST", request.Method);
        Assert.Equal("/projects.xml", request.RawUrl);
        Assert.Equal(Encoding.UTF8.GetBytes(payload), request.Body);
        AssertHeaders(new Dictionary<string, string>
        {
            { "User-Agent", "Redmine.Net.Api.Net" },
            { "X-Redmine-API-Key", ApiKey },
            { "Content-Type", "application/xml" },
            { "Content-Length", request.Body.Length.ToString() }
        }, request);
        Assert.Equal(Encoding.UTF8.GetBytes(NonAscii), response.RawContent);
        Assert.Null(response.Content);
        Assert.Equal(201, response.StatusCode);
        Assert.True(response.IsSuccessful);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, "PUT")]
    [InlineData(true, "PUT")]
    [InlineData(false, "PATCH")]
    [InlineData(true, "PATCH")]
    public async Task Update_And_Patch_Should_Send_Body(bool async, string method)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { StatusCode = 204 });
        var manager = CreateManager(server.Url, b => b.WithSerializationType(SerializationType.Json));
        const string payload = "{\"project\":{\"name\":\"Ünïcödé\"}}";

        ApiResponseMessage response;
        if (method == "PUT")
        {
            response = async
                ? await manager.ApiClient.UpdateAsync("projects/1.json", payload, cancellationToken: TestContext.Current.CancellationToken)
                : manager.ApiClient.Update("projects/1.json", payload);
        }
        else
        {
            response = async
                ? await manager.ApiClient.PatchAsync("projects/1.json", payload, cancellationToken: TestContext.Current.CancellationToken)
                : manager.ApiClient.Patch("projects/1.json", payload);
        }

        var request = Assert.Single(server.Requests);
        Assert.Equal(method, request.Method);
        Assert.Equal(Encoding.UTF8.GetBytes(payload), request.Body);
        AssertHeaders(new Dictionary<string, string>
        {
            { "User-Agent", "Redmine.Net.Api.Net" },
            { "X-Redmine-API-Key", ApiKey },
            { "Content-Type", "application/json" },
            { "Content-Length", request.Body.Length.ToString() }
        }, request);
        Assert.Empty(response.RawContent);
        Assert.Equal(204, response.StatusCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Delete_Should_Send_Empty_Body_Without_Content_Type(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse());
        var manager = CreateManager(server.Url);

        var response = async
            ? await manager.ApiClient.DeleteAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Delete("projects/1.xml");

        var request = Assert.Single(server.Requests);
        Assert.Equal("DELETE", request.Method);
        Assert.Empty(request.Body);
        AssertHeaders(new Dictionary<string, string>
        {
            { "User-Agent", "Redmine.Net.Api.Net" },
            { "X-Redmine-API-Key", ApiKey },
            { "Content-Length", "0" }
        }, request);
        Assert.Empty(response.RawContent);
        Assert.Equal(200, response.StatusCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Upload_Should_Send_Octet_Stream(bool async)
    {
        var data = new byte[] { 0, 1, 2, 255 };
        using var server = new TestHttpServer(_ => Response("<upload><token>t</token></upload>", 201));
        var manager = CreateManager(server.Url);

        var response = async
            ? await manager.ApiClient.UploadFileAsync("uploads.xml", data, cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Upload("uploads.xml", data);

        var request = Assert.Single(server.Requests);
        Assert.Equal("POST", request.Method);
        Assert.Equal(data, request.Body);
        AssertHeaders(new Dictionary<string, string>
        {
            { "User-Agent", "Redmine.Net.Api.Net" },
            { "X-Redmine-API-Key", ApiKey },
            { "Content-Type", "application/octet-stream" },
            { "Content-Length", "4" }
        }, request);
        Assert.Equal("<upload><token>t</token></upload>", Encoding.UTF8.GetString(response.RawContent));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Download_Should_Return_Bytes(bool async)
    {
        var data = Encoding.UTF8.GetBytes(NonAscii);
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/octet-stream", Body = data });
        var manager = CreateManager(server.Url);
        var requestOptions = new RequestOptions { QueryString = new NameValueCollection { { "a", "1" } } };

        var response = async
            ? await manager.ApiClient.DownloadAsync("attachments/download/1/a.txt", requestOptions, cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Download("attachments/download/1/a.txt", requestOptions);

        var request = Assert.Single(server.Requests);
        Assert.Equal("GET", request.Method);
        Assert.Equal("/attachments/download/1/a.txt?a=1", request.RawUrl);
        AssertHeaders(new Dictionary<string, string> { { "User-Agent", "Redmine.Net.Api.Net" }, { "X-Redmine-API-Key", ApiKey } }, request);
        Assert.Equal(data, response.RawContent);
        Assert.Null(response.Content);
        Assert.Equal(200, response.StatusCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Download_Should_Report_Progress(bool async)
    {
        var data = new byte[64 * 1024];
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/octet-stream", Body = data });
        var manager = CreateManager(server.Url);
        var progress = new RecordingProgress();

        var result = async
            ? await manager.DownloadFileAsync("attachments/download/1/a.bin", progress: progress, cancellationToken: TestContext.Current.CancellationToken)
            : manager.DownloadFile("attachments/download/1/a.bin", progress: progress);

        Assert.Equal(data, result);
        Assert.True(SpinWait.SpinUntil(() => progress.Values.Length > 0, TimeSpan.FromSeconds(5)));
        Assert.All(progress.Values, value => Assert.InRange(value, 0, 100));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public void DownloadFile_With_Progress_Should_Restore_SynchronizationContext_Without_Posting(bool withContext)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Body = [1, 2, 3] });
        var manager = CreateManager(server.Url);
        var context = withContext ? new RecordingSynchronizationContext() : null;
        SynchronizationContext before = null;
        SynchronizationContext after = null;
        byte[] result = null;
        Exception error = null;

        var thread = new Thread(() =>
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(context);
                before = SynchronizationContext.Current;
                result = manager.DownloadFile("attachments/download/1/a.bin", progress: new RecordingProgress());
                after = SynchronizationContext.Current;
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }) { IsBackground = true };
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)));

        Assert.Null(error);
        Assert.Equal(new byte[] { 1, 2, 3 }, result);
        Assert.Same(before, after);
        Assert.Same(context, after);
        Assert.Equal(0, context?.Posts ?? 0);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void DownloadFile_Should_Resolve_Absolute_Address()
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Body = [1] });
        var manager = CreateManager($"http://localhost:{TestHttpServer.GetFreePort()}");

        var result = manager.DownloadFile($"{server.Url}attachments/download/1/a.bin", progress: new RecordingProgress());

        Assert.Equal(new byte[] { 1 }, result);
        Assert.Equal("/attachments/download/1/a.bin", Assert.Single(server.Requests).RawUrl);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Create_Should_Apply_Header_Precedence(bool async)
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml, 201));
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions
        {
            UserAgent = "options-agent",
            DefaultHeaders = PrecedenceDefaultHeaders()
        }));

        if (async)
        {
            await manager.ApiClient.CreateAsync("projects.xml", ProjectXml, PrecedenceRequestOptions(), TestContext.Current.CancellationToken);
        }
        else
        {
            manager.ApiClient.Create("projects.xml", ProjectXml, PrecedenceRequestOptions());
        }

        var request = Assert.Single(server.Requests);
        AssertHeaders(new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "request" },
            { "Accept", "application/xml" },
            { "User-Agent", "custom-agent" },
            { "X-Redmine-API-Key", ApiKey },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "bob" },
            { "Content-Type", "application/custom+xml" },
            { "Content-Length", request.Body.Length.ToString() }
        }, request);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_Should_Apply_Header_Precedence(bool async)
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions
        {
            UserAgent = "options-agent",
            DefaultHeaders = PrecedenceDefaultHeaders()
        }));

        if (async)
        {
            await manager.ApiClient.GetAsync("projects/1.xml", PrecedenceRequestOptions(), TestContext.Current.CancellationToken);
        }
        else
        {
            manager.ApiClient.Get("projects/1.xml", PrecedenceRequestOptions());
        }

        var request = Assert.Single(server.Requests);
        AssertHeaders(new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "request" },
            { "Accept", "application/xml" },
            { "User-Agent", "custom-agent" },
            { "X-Redmine-API-Key", ApiKey },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "bob" },
            { "Content-Type", "text/html" },
#if !NETFRAMEWORK
            { "Content-Length", "0" }
#endif
        }, request);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Delete_Should_Apply_Header_Precedence(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse());
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions
        {
            UserAgent = "options-agent",
            DefaultHeaders = PrecedenceDefaultHeaders()
        }));

        if (async)
        {
            await manager.ApiClient.DeleteAsync("projects/1.xml", PrecedenceRequestOptions(), TestContext.Current.CancellationToken);
        }
        else
        {
            manager.ApiClient.Delete("projects/1.xml", PrecedenceRequestOptions());
        }

        var request = Assert.Single(server.Requests);
        AssertHeaders(new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "request" },
            { "Accept", "application/xml" },
            { "User-Agent", "custom-agent" },
            { "X-Redmine-API-Key", ApiKey },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "bob" },
            { "Content-Type", "text/html" },
            { "Content-Length", "0" }
        }, request);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Send_Options_UserAgent_And_DefaultHeaders_When_Request_Does_Not_Set_Them()
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions
        {
            UserAgent = "options-agent",
            DefaultHeaders = PrecedenceDefaultHeaders()
        }));

        manager.ApiClient.Get("projects/1.xml");

        AssertHeaders(new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "default" },
            { "Accept", "text/plain" },
            { "User-Agent", "default-agent" },
            { "X-Redmine-API-Key", ApiKey },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "default-user" },
            { "Content-Type", "text/plain" },
#if !NETFRAMEWORK
            { "Content-Length", "0" }
#endif
        }, Assert.Single(server.Requests));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Basic_Authentication_Should_Send_Authorization_Header(bool async)
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url).WithBasicAuthentication("user", "pass"));
        var requestOptions = new RequestOptions { Headers = new Dictionary<string, string> { { "Authorization", "Bearer other" } } };

        if (async)
        {
            await manager.ApiClient.GetAsync("projects/1.xml", requestOptions, TestContext.Current.CancellationToken);
        }
        else
        {
            manager.ApiClient.Get("projects/1.xml", requestOptions);
        }

        AssertHeaders(new Dictionary<string, string>
        {
            { "User-Agent", "Redmine.Net.Api.Net" },
            { "Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass")) }
        }, Assert.Single(server.Requests));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void No_Authentication_Should_Keep_RequestOptions_Authentication_Headers()
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(server.Url));
        var requestOptions = new RequestOptions { Headers = new Dictionary<string, string> { { "X-Redmine-API-Key", "other" } } };

        manager.ApiClient.Get("projects/1.xml", requestOptions);

        AssertHeaders(new Dictionary<string, string>
        {
            { "User-Agent", "Redmine.Net.Api.Net" },
            { "X-Redmine-API-Key", "other" }
        }, Assert.Single(server.Requests));
    }

    private static Dictionary<string, string> PrecedenceDefaultHeaders()
    {
        return new Dictionary<string, string>
        {
            { "X-Default", "default" },
            { "X-Custom", "default" },
            { "Accept", "text/plain" },
            { "User-Agent", "default-agent" },
            { "X-Redmine-API-Key", "default-key" },
            { RedmineConstants.IMPERSONATE_HEADER_KEY, "default-user" },
            { "Content-Type", "text/plain" }
        };
    }

    private static RequestOptions PrecedenceRequestOptions()
    {
        return new RequestOptions
        {
            Accept = "application/xml",
            UserAgent = "custom-agent",
            ContentType = "application/custom+xml",
            ImpersonateUser = "bob",
            Headers = new Dictionary<string, string>
            {
                { "X-Custom", "request" },
                { "Accept", "text/html" },
                { "User-Agent", "header-agent" },
                { "X-Redmine-API-Key", "other" },
                { RedmineConstants.IMPERSONATE_HEADER_KEY, "alice" },
                { "Content-Type", "text/html" }
            }
        };
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Not_Found_Should_Throw_With_Details(bool async)
    {
        using var server = new TestHttpServer(_ => Response("not found", 404, "text/plain", new Dictionary<string, string> { { "X-Request-Id", "abc" } }));
        var manager = CreateManager(server.Url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Get("projects/1.xml"),
            () => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "The requested resource was not found.", 404, "GET", $"{server.Url}projects/1.xml", false, null);
        Assert.Equal("abc", exception.CorrelationId);
        Assert.Equal("abc", exception.ResponseHeaders["x-request-id"]);
        Assert.Equal("text/plain", exception.ResponseHeaders["Content-Type"]);
        var inner = Assert.IsType<WebException>(exception.InnerException);
        Assert.Equal(WebExceptionStatus.ProtocolError, inner.Status);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, 401, "Authentication is required or has failed.", false)]
    [InlineData(true, 403, "You don't have permission to access this resource.", false)]
    [InlineData(false, 406, "The requested format is not supported.", false)]
    [InlineData(true, 409, "The resource you are trying to update has been modified since you last retrieved it.", false)]
    [InlineData(false, 500, "The server encountered an unexpected error.", false)]
    [InlineData(true, 503, "ServiceUnavailable", true)]
    [InlineData(false, 429, "TooManyRequests", true)]
    [InlineData(true, 400, "BadRequest", false)]
    public async Task Error_Status_Should_Map_To_Exception(bool async, int statusCode, string message, bool isTransient)
    {
        using var server = new TestHttpServer(_ => Response("error", statusCode, "text/plain"));
        var manager = CreateManager(server.Url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Delete("projects/1.xml"),
            () => manager.ApiClient.DeleteAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, message, statusCode, "DELETE", $"{server.Url}projects/1.xml", isTransient, null);
        Assert.Null(exception.CorrelationId);
        Assert.IsType<WebException>(exception.InnerException);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Download_Error_Should_Report_Get_Method(bool async)
    {
        using var server = new TestHttpServer(_ => Response("error", 404, "text/plain"));
        var manager = CreateManager(server.Url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Download("attachments/download/1/a.txt"),
            () => manager.ApiClient.DownloadAsync("attachments/download/1/a.txt", cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "The requested resource was not found.", 404, "GET", $"{server.Url}attachments/download/1/a.txt", false, null);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void DownloadFile_With_Progress_Error_Should_Throw_Like_Without_Progress()
    {
        using var server = new TestHttpServer(_ => Response("error", 404, "text/plain"));
        var manager = CreateManager(server.Url);
        var expected = Assert.Throws<RedmineApiException>(() => manager.DownloadFile("attachments/download/1/a.bin"));
        Exception error = null;

        var thread = new Thread(() =>
        {
            try
            {
                manager.DownloadFile("attachments/download/1/a.bin", progress: new RecordingProgress());
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }) { IsBackground = true };
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)));

        var exception = Assert.IsType<RedmineApiException>(error);
        Assert.Equal(expected.Message, exception.Message);
        Assert.Equal(expected.HttpStatusCode, exception.HttpStatusCode);
        Assert.Equal("GET", exception.Method);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Not_Found_With_Stalled_Body_Should_Throw_Without_Reading_Body(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { StatusCode = 404, ContentType = "text/plain", Body = Encoding.UTF8.GetBytes("error"), ContentLength = 1024 });
        var manager = CreateManager(server.Url);

        var call = Task.Run(() => CatchAsync(async,
            () => manager.ApiClient.Get("projects/1.xml"),
            () => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken)), TestContext.Current.CancellationToken);

        Assert.Same(call, await Task.WhenAny(call, Task.Delay(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken)));
        AssertException(await call, "The requested resource was not found.", 404, "GET", $"{server.Url}projects/1.xml", false, null);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task DownloadAsync_With_Progress_Error_Should_Throw()
    {
        using var server = new TestHttpServer(_ => Response("error", 404, "text/plain"));
        var manager = CreateManager(server.Url);

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.ApiClient.DownloadAsync("attachments/download/1/a.txt", progress: new RecordingProgress(), cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "The requested resource was not found.", 404, "GET", $"{server.Url}attachments/download/1/a.txt", false, null);
        Assert.IsType<WebException>(exception.InnerException);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_Should_Throw_Validation_Errors(bool async)
    {
        const string errors = "<errors type=\"array\"><error>Name cannot be blank</error><error>Identifier is too long ţ</error></errors>";
        using var server = new TestHttpServer(_ => Response(errors, 422, headers: new Dictionary<string, string> { { "X-Request-Id", "r1" } }));
        var manager = CreateManager(server.Url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Update("projects/1.xml", ProjectXml),
            () => manager.ApiClient.UpdateAsync("projects/1.xml", ProjectXml, cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "Name cannot be blank" + Environment.NewLine + "Identifier is too long ţ", 422, "PUT", $"{server.Url}projects/1.xml", false, null);
        Assert.Equal("r1", exception.CorrelationId);
        Assert.Null(exception.InnerException);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_With_Empty_Body_Should_Throw_Default_Message(bool async)
    {
        using var server = new TestHttpServer(_ => Response(string.Empty, 422));
        var manager = CreateManager(server.Url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Create("projects.xml", ProjectXml),
            () => manager.ApiClient.CreateAsync("projects.xml", ProjectXml, cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "Validation failed for the submitted data.", 422, "POST", $"{server.Url}projects.xml", false, null);
        Assert.Null(exception.InnerException);
        Assert.NotNull(exception.ResponseHeaders);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unprocessable_Entity_With_Invalid_Body_Should_Throw_With_Body_As_Message_And_Request_Details(bool async)
    {
        using var server = new TestHttpServer(_ => Response("not xml", 422));
        var manager = CreateManager(server.Url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Create("projects.xml", ProjectXml),
            () => manager.ApiClient.CreateAsync("projects.xml", ProjectXml, cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "not xml", 422, "POST", $"{server.Url}projects.xml", false, null);
        Assert.NotNull(exception.ResponseHeaders);
        Assert.IsType<RedmineException>(exception.InnerException);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Connection_Reset_Should_Throw_Api_Exception(bool async)
    {
        await using var server = ResettingServer.Start();
        var url = server.Url;
        var manager = CreateManager(url);

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Get("projects/1.xml"),
            () => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Null(exception.HttpStatusCode);
        Assert.Equal("GET", exception.Method);
        Assert.Equal($"{url}projects/1.xml", exception.Endpoint);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.Null(exception.ResponseHeaders);
        Assert.Null(exception.CorrelationId);
        Assert.NotNull(exception.InnerException);
#if NETFRAMEWORK
        Assert.True(exception.IsTransient);
#else
        Assert.False(exception.IsTransient);
        Assert.IsType<System.Net.Http.HttpRequestException>(exception.InnerException);
        Assert.Equal(exception.InnerException.Message, exception.Message);
#endif
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Timeout_Should_Throw_Transient_Timeout(bool async)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Delay = TimeSpan.FromSeconds(5) });
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions { Timeout = TimeSpan.FromMilliseconds(300) }));

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Get("projects/1.xml"),
            () => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "Timeout", null, "GET", $"{server.Url}projects/1.xml", true, "UNKNOWN");
        Assert.Null(exception.ResponseHeaders);
        Assert.NotNull(exception.InnerException);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_With_Canceled_Token_Should_Throw_OperationCanceledException_Before_Sending()
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions { Timeout = TimeSpan.Zero }));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: cts.Token));

        Assert.Equal(cts.Token, exception.CancellationToken);
        Assert.Empty(server.Requests);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(null)]
    [InlineData(30000)]
    public async Task GetAsync_Canceled_In_Flight_Should_Throw_Canceled_By_User(int? timeoutMilliseconds)
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Delay = TimeSpan.FromSeconds(5) });
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions { Timeout = timeoutMilliseconds.HasValue ? TimeSpan.FromMilliseconds(timeoutMilliseconds.Value) : null }));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        cts.CancelAfter(TimeSpan.FromMilliseconds(300));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: cts.Token));

        AssertException(exception, "The operation was canceled by the user.", null, "GET", $"{server.Url}projects/1.xml", false, "UNKNOWN");
        Assert.Null(exception.ResponseHeaders);
        var inner = Assert.IsType<WebException>(exception.InnerException);
        Assert.Equal(WebExceptionStatus.RequestCanceled, inner.Status);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task DownloadAsync_With_Timeout_Should_Time_Out_On_Request_Timeout()
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse { Body = [1, 2], Delay = TimeSpan.FromSeconds(2) });
        var manager = CreateManager(server.Url, b => b.WithWebClientOptions(new RedmineWebClientOptions { Timeout = TimeSpan.FromMilliseconds(200) }));

        var exception = await Record.ExceptionAsync(() => manager.ApiClient.DownloadAsync("attachments/download/1/a.bin", cancellationToken: TestContext.Current.CancellationToken));

        var apiException = Assert.IsType<RedmineApiException>(exception);
        AssertException(apiException, "Timeout", null, "GET", $"{server.Url}attachments/download/1/a.bin", true, "UNKNOWN");
        Assert.Equal(WebExceptionStatus.Timeout, Assert.IsType<WebException>(apiException.InnerException).Status);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Custom_WebClient_Should_Report_Ok_Status_And_Use_Its_BaseAddress()
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml, 201));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
            .WithHost($"http://localhost:{TestHttpServer.GetFreePort()}")
            .WithWebClient(() => new WebClient { BaseAddress = server.Url }));

        var response = manager.ApiClient.Create("projects.xml", ProjectXml);

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(ProjectXml, Encoding.UTF8.GetString(response.RawContent));
        Assert.Equal("/projects.xml", Assert.Single(server.Requests).RawUrl);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Custom_WebClient_Error_Should_Use_Its_BaseAddress_For_Endpoint(bool async)
    {
        using var server = new TestHttpServer(_ => Response("error", 404, "text/plain"));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
            .WithHost($"http://localhost:{TestHttpServer.GetFreePort()}")
            .WithWebClient(() => new WebClient { BaseAddress = server.Url + "redmine/" }));

        var exception = await CatchAsync(async,
            () => manager.ApiClient.Get("projects/1.xml"),
            () => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        AssertException(exception, "The requested resource was not found.", 404, "GET", $"{server.Url}redmine/projects/1.xml", false, null);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Custom_WebClient_Encoding_Should_Decode_Get_Response(bool async)
    {
        var latin1 = Encoding.GetEncoding("iso-8859-1");
        using var server = new TestHttpServer(_ => new TestHttpResponse { ContentType = "application/xml", Body = latin1.GetBytes("<name>café</name>") });
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
            .WithHost(server.Url)
            .WithWebClient(() => new WebClient { Encoding = latin1 }));

        var response = async
            ? await manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken)
            : manager.ApiClient.Get("projects/1.xml");

        Assert.Equal("<name>café</name>", response.Content);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Custom_WebClient_Should_Keep_Its_Headers_And_QueryString()
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
            .WithHost(server.Url)
            .WithApiKeyAuthentication(ApiKey)
            .WithWebClient(() =>
            {
                var client = new WebClient();
                client.Headers["X-Client"] = "client";
                client.Headers["X-Redmine-API-Key"] = "client-key";
                client.QueryString = new NameValueCollection { { "client", "1" } };
                return client;
            }));

        manager.ApiClient.Get("projects/1.xml");

        var request = Assert.Single(server.Requests);
        Assert.Equal("/projects/1.xml?client=1", request.RawUrl);
        AssertHeaders(new Dictionary<string, string> { { "X-Client", "client" }, { "X-Redmine-API-Key", ApiKey } }, request);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Custom_WebClient_Func_Returning_Null_Should_Throw_RedmineException_Even_When_Canceled()
    {
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithWebClient(() => null));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var exception = await Assert.ThrowsAsync<RedmineException>(() => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: cts.Token));

        Assert.Equal("The web client function configured with WithWebClient returned null.", exception.Message);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Custom_WebClient_Func_Throwing_TimeoutException_Should_Be_Wrapped_In_RedmineApiException()
    {
        var timeout = new TimeoutException("t", new WebException("canceled", WebExceptionStatus.RequestCanceled));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithWebClient(() => throw timeout));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Same(timeout, exception.InnerException);
        Assert.Equal("t", exception.Message);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Custom_WebClient_Func_Throwing_OperationCanceledException_With_Inner_Should_Propagate()
    {
        var canceled = new OperationCanceledException("c", new InvalidOperationException("inner"));
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithWebClient(() => throw canceled));

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => manager.ApiClient.GetAsync("projects/1.xml", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Same(canceled, exception);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Invalid_RequestOptions_Header_Overridden_Later_Should_Still_Throw(bool async)
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = CreateManager(server.Url);
        var requestOptions = new RequestOptions { Accept = "application/xml", Headers = new Dictionary<string, string> { { "Accept", "bad\r\nvalue" } } };
        var expected = Assert.ThrowsAny<ArgumentException>(() => new WebHeaderCollection { ["Accept"] = "bad\r\nvalue" });

        var exception = async
            ? await Assert.ThrowsAnyAsync<ArgumentException>(() => manager.ApiClient.GetAsync("projects/1.xml", requestOptions, TestContext.Current.CancellationToken))
            : Assert.ThrowsAny<ArgumentException>(() => manager.ApiClient.Get("projects/1.xml", requestOptions));

        Assert.Equal(expected.GetType(), exception.GetType());
        Assert.Equal(expected.Message, exception.Message);
        Assert.Empty(server.Requests);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Invalid_RequestOptions_Header_With_Canceled_Token_Should_Throw_OperationCanceledException()
    {
        using var server = new TestHttpServer(_ => Response(ProjectXml));
        var manager = CreateManager(server.Url);
        var requestOptions = new RequestOptions { Headers = new Dictionary<string, string> { { "Accept", "bad\r\nvalue" } } };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => manager.ApiClient.GetAsync("projects/1.xml", requestOptions, cts.Token));

        Assert.Empty(server.Requests);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Custom_WebClient_Func_Throwing_WebException_Should_Use_Address_As_Endpoint()
    {
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").WithWebClient(() => throw new WebException("boom", WebExceptionStatus.SendFailure)));

        var exception = Assert.Throws<RedmineApiException>(() => manager.ApiClient.Get("projects/1.xml"));

        AssertException(exception, "boom", null, "GET", "projects/1.xml", true, "UNKNOWN");
        Assert.IsType<WebException>(exception.InnerException);
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

    private sealed class RecordingSynchronizationContext : SynchronizationContext
    {
        public int Posts;

        public override void Post(SendOrPostCallback d, object state)
        {
            Interlocked.Increment(ref Posts);
            base.Post(d, state);
        }
    }
}
