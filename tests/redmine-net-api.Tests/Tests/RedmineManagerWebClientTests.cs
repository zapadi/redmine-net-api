using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
using Redmine.Net.Api.Types;
using Xunit;

#pragma warning disable SYSLIB0014
namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class RedmineManagerWebClientTests
{
    private const string Host = "http://localhost:3000";
    private const string IssuesPage = """<issues type="array" total_count="3" offset="0" limit="1"></issues>""";
    private const string Errors = """<errors type="array"><error>Name cannot be blank</error></errors>""";

    private static (RedmineManager Manager, List<FakeWebClient> Clients) CreateManager(Func<FakeWebRequest, WebResponse> responder, IRedmineWebClientOptions clientOptions = null)
    {
        var clients = new List<FakeWebClient>();
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost(Host)
            .WithWebClient(() =>
            {
                var client = new FakeWebClient(responder);
                clients.Add(client);
                return client;
            });

        if (clientOptions != null)
        {
            builder.WithWebClientOptions(clientOptions);
        }

        return (new RedmineManager(builder), clients);
    }

    private static RedmineManager CreateHangingManager(TimeSpan? timeout)
    {
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost(Host)
            .WithWebClientOptions(new RedmineWebClientOptions { Timeout = timeout })
            .WithWebClient(() => new HangingWebClient());

        return new RedmineManager(builder);
    }

    private static WebResponse SlowResponse()
    {
        System.Threading.Thread.Sleep(500);
        return new FakeWebResponse("abc");
    }

    private static async Task<T> Bounded<T>(Task<T> task)
    {
        if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10))) != task)
        {
            throw new TimeoutException("Request did not complete.");
        }

        return await task;
    }

    private static WebException UnprocessableEntity()
    {
        return new WebException("422", null, WebExceptionStatus.ProtocolError, FakeHttpWebResponse.Create((HttpStatusCode)422, Errors));
    }

    private static WebException ProtocolError(HttpStatusCode statusCode, WebHeaderCollection headers = null)
    {
        return new WebException(statusCode.ToString(), null, WebExceptionStatus.ProtocolError, FakeHttpWebResponse.Create(statusCode, string.Empty, headers));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Set_Exception_Details_On_Not_Found()
    {
        var (manager, _) = CreateManager(_ => throw ProtocolError(HttpStatusCode.NotFound, new WebHeaderCollection { { "X-Request-Id", "abc" } }));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal(404, exception.HttpStatusCode);
        Assert.Equal("GET", exception.Method);
        Assert.Equal($"{Host}/projects/1.xml", exception.Endpoint);
        Assert.Equal("abc", exception.CorrelationId);
        Assert.Equal("abc", exception.ResponseHeaders["x-request-id"]);
        Assert.False(exception.IsTransient);
        Assert.Null(exception.ErrorCode);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadRequest)]
    public void Get_Should_Not_Mark_Exception_As_Transient(HttpStatusCode statusCode)
    {
        var (manager, _) = CreateManager(_ => throw ProtocolError(statusCode));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal((int)statusCode, exception.HttpStatusCode);
        Assert.False(exception.IsTransient);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(HttpStatusCode.RequestTimeout)]
    [InlineData((HttpStatusCode)429)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public async Task GetAsync_Should_Mark_Exception_As_Transient(HttpStatusCode statusCode)
    {
        var (manager, _) = CreateManager(_ => throw ProtocolError(statusCode, new WebHeaderCollection { { "Retry-After", "120" } }));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal((int)statusCode, exception.HttpStatusCode);
        Assert.True(exception.IsTransient);
        Assert.Equal("120", exception.ResponseHeaders["Retry-After"]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Mark_Connection_Failure_As_Transient()
    {
        var (manager, _) = CreateManager(_ => throw new WebException("refused", WebExceptionStatus.ConnectFailure));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal("refused", exception.Message);
        Assert.True(exception.IsTransient);
        Assert.Null(exception.HttpStatusCode);
        Assert.Null(exception.ResponseHeaders);
        Assert.Equal("GET", exception.Method);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Throw_RedmineApiException_On_Protocol_Error_Without_Response()
    {
        var (manager, _) = CreateManager(_ => throw new WebException("protocol", null, WebExceptionStatus.ProtocolError, null));

        var exception = Assert.Throws<RedmineApiException>(() => manager.Get<Project>("1"));

        Assert.Equal("protocol", exception.Message);
        Assert.Null(exception.HttpStatusCode);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Throw_RedmineApiException_On_Protocol_Error_Without_Response()
    {
        var (manager, _) = CreateManager(_ => throw new WebException("protocol", null, WebExceptionStatus.ProtocolError, null));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("protocol", exception.Message);
        Assert.Null(exception.HttpStatusCode);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void UploadFile_With_Null_Data_Should_Throw_RedmineApiException()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("<upload><token>t</token></upload>"));

        var exception = Assert.Throws<RedmineApiException>(() => manager.UploadFile(null));

        Assert.Equal("POST", exception.Method);
        Assert.Empty(Assert.Single(clients).Requests);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Create_Should_Apply_RequestOptions_Headers()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("<project><id>1</id><name>foo</name></project>"));
        var requestOptions = new RequestOptions
        {
            Accept = "application/xml",
            UserAgent = "custom-agent",
            ContentType = "application/custom+xml",
            Headers = new Dictionary<string, string> { { "X-Custom", "custom" } }
        };

        manager.Create(new Project { Name = "foo" }, requestOptions: requestOptions);

        var request = clients[0].Requests[0];
        Assert.Equal("application/xml", request.Headers[HttpRequestHeader.Accept]);
        Assert.Equal("custom-agent", request.Headers[HttpRequestHeader.UserAgent]);
        Assert.Equal("application/custom+xml", request.Headers[HttpRequestHeader.ContentType]);
        Assert.Equal("custom", request.Headers["X-Custom"]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Not_Override_Authentication_And_Impersonation_With_RequestOptions_Headers()
    {
        var clients = new List<FakeWebClient>();
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
            .WithHost(Host)
            .WithApiKeyAuthentication("secret")
            .WithWebClient(() =>
            {
                var client = new FakeWebClient(_ => new FakeWebResponse("<project><id>1</id></project>"));
                clients.Add(client);
                return client;
            }));
        var requestOptions = new RequestOptions
        {
            ImpersonateUser = "bob",
            Headers = new Dictionary<string, string>
            {
                { "X-Redmine-API-Key", "other" },
                { RedmineConstants.IMPERSONATE_HEADER_KEY, "alice" }
            }
        };

        manager.Get<Project>("1", requestOptions);

        var request = clients[0].Requests[0];
        Assert.Equal("secret", request.Headers["X-Redmine-API-Key"]);
        Assert.Equal("bob", request.Headers[RedmineConstants.IMPERSONATE_HEADER_KEY]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Create_Without_RequestOptions_Should_Send_Default_Headers()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("<project><id>1</id><name>foo</name></project>"));

        manager.Create(new Project { Name = "foo" });

        var request = clients[0].Requests[0];
        Assert.Equal(RedmineConstants.CONTENT_TYPE_APPLICATION_XML, request.Headers[HttpRequestHeader.ContentType]);
        Assert.Null(request.Headers[HttpRequestHeader.Accept]);
        Assert.Null(request.Headers[HttpRequestHeader.UserAgent]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Keep_Per_Request_UserAgent()
    {
        var request = CreateInternalWebRequest(
            new RedmineWebClientOptions { UserAgent = "options-agent" },
            webClient => webClient.Headers[HttpRequestHeader.UserAgent] = "request-agent");

        Assert.Equal("request-agent", request.UserAgent);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Fall_Back_To_Default_UserAgent()
    {
        var request = CreateInternalWebRequest(new RedmineWebClientOptions { UserAgent = null });

        Assert.Equal("RedmineDotNetAPIClient", request.UserAgent);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Use_Options_UserAgent_By_Default()
    {
        var request = CreateInternalWebRequest(new RedmineWebClientOptions { UserAgent = "options-agent" });

        Assert.Equal("options-agent", request.UserAgent);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Build_Should_Copy_ClientFunc()
    {
        Func<WebClient> clientFunc = () => new WebClient();

        var options = new RedmineManagerOptionsBuilder().WithHost(Host).WithWebClient(clientFunc).Build();

        Assert.Same(clientFunc, options.ClientFunc);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Custom_WebClient_Should_Be_Used_With_BaseAddress()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("<project><id>1</id><name>foo</name></project>"));

        var project = manager.Get<Project>("1");

        Assert.Equal("foo", project.Name);
        Assert.Single(clients);
        Assert.Equal($"{Host}/projects/1.xml", clients[0].Requests[0].RequestUri.ToString());
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Custom_WebClient_Func_Returning_Null_Should_Throw_RedmineException()
    {
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(Host).WithWebClient(() => null));

        Assert.Throws<RedmineException>(() => manager.Get<Project>("1"));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Count_Should_Not_Mutate_Caller_RequestOptions()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(IssuesPage));
        var requestOptions = new RequestOptions { QueryString = new NameValueCollection { { "status_id", "open" } } };

        var count = manager.Count<Issue>(requestOptions);

        Assert.Equal(3, count);
        Assert.Single(requestOptions.QueryString);
        Assert.Contains("limit=1", clients[0].Requests[0].RequestUri.Query);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task CountAsync_Should_Send_Limit_And_Not_Mutate_Caller_RequestOptions()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(IssuesPage));
        var requestOptions = new RequestOptions();

        var count = await manager.CountAsync<Issue>(requestOptions, TestContext.Current.CancellationToken);

        Assert.Equal(3, count);
        Assert.Null(requestOptions.QueryString);
        Assert.Contains("limit=1", clients[0].Requests[0].RequestUri.Query);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_Should_Not_Mutate_Caller_RequestOptions()
    {
        var (manager, _) = CreateManager(_ => new FakeWebResponse(IssuesPage));
        var requestOptions = new RequestOptions { QueryString = new NameValueCollection { { "status_id", "open" } } };

        manager.Get<Issue>(requestOptions);

        Assert.Single(requestOptions.QueryString);
        Assert.Null(requestOptions.QueryString[RedmineKeys.LIMIT]);
        Assert.Null(requestOptions.QueryString[RedmineKeys.OFFSET]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Update_Should_Throw_On_Unprocessable_Entity()
    {
        var (manager, _) = CreateManager(_ => throw UnprocessableEntity());

        var exception = Assert.Throws<RedmineApiException>(() => manager.Update("1", new Project { Name = "foo" }));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
        Assert.Equal("PUT", exception.Method);
        Assert.Equal($"{Host}/projects/1.xml", exception.Endpoint);
        Assert.NotNull(exception.ResponseHeaders);
        Assert.False(exception.IsTransient);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task DeleteAsync_Should_Throw_On_Unprocessable_Entity()
    {
        var (manager, _) = CreateManager(_ => throw UnprocessableEntity());

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.DeleteAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Create_Should_Throw_On_Unprocessable_Entity()
    {
        var (manager, _) = CreateManager(_ => throw UnprocessableEntity());

        var exception = Assert.Throws<RedmineApiException>(() => manager.Create(new Project { Name = "foo" }));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task ArchiveProjectAsync_Should_Put_To_Unescaped_Url()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(string.Empty));

        await manager.ArchiveProjectAsync("foo", cancellationToken: TestContext.Current.CancellationToken);

        var request = clients[0].Requests[0];
        Assert.Equal("PUT", request.Method);
        Assert.Equal($"{Host}/projects/foo/archive.xml", request.RequestUri.ToString());
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void UnarchiveProject_Should_Put_To_Unescaped_Url()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(string.Empty));

        manager.UnarchiveProject("foo");

        var request = clients[0].Requests[0];
        Assert.Equal("PUT", request.Method);
        Assert.Equal($"{Host}/projects/foo/unarchive.xml", request.RequestUri.ToString());
    }

    private static HttpWebRequest CreateInternalWebRequest(RedmineWebClientOptions clientOptions, Action<InternalWebClient> configure = null)
    {
        var options = new RedmineManagerOptionsBuilder()
            .WithHost(Host)
            .WithWebClientOptions(clientOptions)
            .Build();
        using var webClient = new InternalWebClient(options);
        configure?.Invoke(webClient);

        var getWebRequest = typeof(InternalWebClient).GetMethod("GetWebRequest", BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(Uri)], null);
        return (HttpWebRequest)getWebRequest!.Invoke(webClient, [new Uri($"{Host}/projects.xml")]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Merge_DefaultHeaders_And_Bypass_Proxy_By_Default()
    {
        var request = CreateInternalWebRequest(
            new RedmineWebClientOptions { DefaultHeaders = new Dictionary<string, string> { { "X-Custom", "custom" } } },
            webClient => webClient.Headers.Add("X-Redmine-API-Key", "secret"));

        Assert.Equal("secret", request.Headers["X-Redmine-API-Key"]);
        Assert.Equal("custom", request.Headers["X-Custom"]);
        Assert.Null(request.Proxy);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Send_DefaultHeaders_Not_Set_By_Request()
    {
        var request = CreateInternalWebRequest(new RedmineWebClientOptions { DefaultHeaders = new Dictionary<string, string> { { "X-Custom", "default" } } });

        Assert.Equal("default", request.Headers["X-Custom"]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Not_Override_Request_Headers_With_DefaultHeaders()
    {
        var request = CreateInternalWebRequest(
            new RedmineWebClientOptions
            {
                DefaultHeaders = new Dictionary<string, string>
                {
                    { "X-Custom", "default" },
                    { "Accept", "text/plain" },
                    { "User-Agent", "default-agent" },
                    { "X-Redmine-API-Key", "default-key" },
                    { "Content-Type", "text/plain" }
                }
            },
            webClient =>
            {
                webClient.Headers["X-Custom"] = "request";
                webClient.Headers[HttpRequestHeader.Accept] = "application/xml";
                webClient.Headers[HttpRequestHeader.UserAgent] = "request-agent";
                webClient.Headers["X-Redmine-API-Key"] = "secret";
                webClient.Headers[HttpRequestHeader.ContentType] = RedmineConstants.CONTENT_TYPE_APPLICATION_XML;
            });

        Assert.Equal("request", request.Headers["X-Custom"]);
        Assert.Equal("application/xml", request.Accept);
        Assert.Equal("request-agent", request.UserAgent);
        Assert.Equal("secret", request.Headers["X-Redmine-API-Key"]);
        Assert.Equal(RedmineConstants.CONTENT_TYPE_APPLICATION_XML, request.ContentType);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Keep_Default_Proxy_When_UseProxy_Is_True()
    {
        var request = CreateInternalWebRequest(new RedmineWebClientOptions { UseProxy = true });

        Assert.Same(WebRequest.DefaultWebProxy, request.Proxy);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void InternalWebClient_Should_Use_Configured_Proxy()
    {
        var proxy = new WebProxy("http://proxy.local:8080");

        var request = CreateInternalWebRequest(new RedmineWebClientOptions { Proxy = proxy });

        Assert.Same(proxy, request.Proxy);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Custom_WebClientOptions_Implementation_Should_Create_ApiClient()
    {
        var (manager, _) = CreateManager(_ => new FakeWebResponse("<project><id>1</id><name>foo</name></project>"), new CustomWebClientOptions());

        var project = manager.Get<Project>("1");

        Assert.Equal("foo", project.Name);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Throw_When_Request_Is_Canceled_Without_Caller_Cancellation()
    {
        var (manager, _) = CreateManager(_ => throw new WebException("canceled", WebExceptionStatus.RequestCanceled));

        await Assert.ThrowsAsync<RedmineApiException>(() => manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData("attachments/download/1/a.txt", Host + "/attachments/download/1/a.txt")]
    [InlineData("http://files.local:8080/attachments/download/1/a.txt", "http://files.local:8080/attachments/download/1/a.txt")]
    public void DownloadFile_Should_Resolve_Address(string address, string expected)
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("abc"));

        var data = manager.DownloadFile(address);

        Assert.Equal("abc", System.Text.Encoding.UTF8.GetString(data));
        Assert.Equal(expected, clients[0].Requests[0].RequestUri.ToString());
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData("attachments/download/1/a.txt", Host + "/attachments/download/1/a.txt")]
    [InlineData("http://files.local:8080/attachments/download/1/a.txt", "http://files.local:8080/attachments/download/1/a.txt")]
    public void DownloadFile_With_Progress_Should_Resolve_Address(string address, string expected)
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("abc"));

        var data = manager.DownloadFile(address, progress: new Progress<int>());

        Assert.Equal("abc", System.Text.Encoding.UTF8.GetString(data));
        Assert.Equal(expected, clients[0].Requests[0].RequestUri.ToString());
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task DownloadFile_With_Progress_Should_Not_Time_Out_When_Timeout_Is_Set()
    {
        var (manager, _) = CreateManager(_ => SlowResponse(), new RedmineWebClientOptions { Timeout = TimeSpan.FromMilliseconds(100) });

        var data = await Bounded(Task.Run(() => manager.DownloadFile("attachments/download/1/a.txt", progress: new Progress<int>()), TestContext.Current.CancellationToken));

        Assert.Equal("abc", System.Text.Encoding.UTF8.GetString(data));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task DownloadFileAsync_Should_Not_Time_Out_When_Timeout_Is_Set()
    {
        var (manager, _) = CreateManager(_ => SlowResponse(), new RedmineWebClientOptions { Timeout = TimeSpan.FromMilliseconds(100) });

        var data = await Bounded(manager.DownloadFileAsync("attachments/download/1/a.txt", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("abc", System.Text.Encoding.UTF8.GetString(data));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Throw_Timeout_When_Timeout_Is_Set()
    {
        var manager = CreateHangingManager(TimeSpan.FromMilliseconds(200));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => Bounded(manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken)));

        Assert.Equal(nameof(WebExceptionStatus.Timeout), exception.Message);
        Assert.True(exception.IsTransient);
        Assert.Equal("GET", exception.Method);
        Assert.Equal($"{Host}/projects/1.xml", exception.Endpoint);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Throw_Timeout_When_Timeout_Is_Zero()
    {
        var manager = CreateHangingManager(TimeSpan.Zero);

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => Bounded(manager.GetAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken)));

        Assert.Equal(nameof(WebExceptionStatus.Timeout), exception.Message);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Not_Time_Out_When_Timeout_Is_Not_Set()
    {
        var manager = CreateHangingManager(null);
        using var cts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        cts.CancelAfter(TimeSpan.FromMilliseconds(500));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => Bounded(manager.GetAsync<Project>("1", cancellationToken: cts.Token)));

        Assert.NotEqual(nameof(WebExceptionStatus.Timeout), exception.Message);
        Assert.False(exception.IsTransient);
        Assert.True(cts.IsCancellationRequested);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAsync_Should_Throw_With_Unknown_ErrorCode_When_Canceled_By_User()
    {
        var manager = CreateHangingManager(TimeSpan.FromSeconds(30));
        using var cts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        cts.CancelAfter(TimeSpan.FromMilliseconds(200));

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => Bounded(manager.GetAsync<Project>("1", cancellationToken: cts.Token)));

        Assert.Equal("The operation was canceled by the user.", exception.Message);
        Assert.Equal("UNKNOWN", exception.ErrorCode);
        Assert.False(exception.IsTransient);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Creating_Manager_Should_Not_Reset_CheckCertificateRevocationList()
    {
        var original = ServicePointManager.CheckCertificateRevocationList;
        try
        {
            ServicePointManager.CheckCertificateRevocationList = true;

            CreateManager(_ => new FakeWebResponse(string.Empty));

            Assert.True(ServicePointManager.CheckCertificateRevocationList);
        }
        finally
        {
            ServicePointManager.CheckCertificateRevocationList = original;
        }
    }

#pragma warning disable CS0618
    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Obsolete_GetCurrentUserAsync_Should_Pass_Parameters_And_Impersonation()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("<user><id>1</id></user>"));

        await Redmine.Net.Api.Async.RedmineManagerAsyncExtensions.GetCurrentUserAsync(manager, new NameValueCollection { { "include", "groups" } }, "bob", TestContext.Current.CancellationToken);

        var request = clients[0].Requests[0];
        Assert.Contains("include=groups", request.RequestUri.Query);
        Assert.Equal("bob", request.Headers[RedmineConstants.IMPERSONATE_HEADER_KEY]);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Obsolete_CountAsync_Should_Pass_Include()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(IssuesPage));

        var count = await Redmine.Net.Api.Async.RedmineManagerAsyncExtensions.CountAsync<Issue>(manager, "journals", "watchers");

        Assert.Equal(3, count);
        Assert.Contains("include=journals,watchers", Uri.UnescapeDataString(clients[0].Requests[0].RequestUri.Query));
    }
#pragma warning restore CS0618

    [Fact(Timeout = Constants.TestTimeout)]
    public void Get_List_Should_Return_Null_On_Empty_Response()
    {
        var (manager, _) = CreateManager(_ => new FakeWebResponse(string.Empty));

        Assert.Null(manager.Get<Issue>());
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task GetAllWikiPagesAsync_Should_Return_Null_On_Empty_Response()
    {
        var (manager, _) = CreateManager(_ => new FakeWebResponse(string.Empty));

        Assert.Null(await manager.GetAllWikiPagesAsync("foo", cancellationToken: TestContext.Current.CancellationToken));
    }
}
