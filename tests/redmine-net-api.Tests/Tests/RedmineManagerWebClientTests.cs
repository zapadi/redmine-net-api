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
public sealed class RedmineManagerWebClientTests(XmlPrettyPrintFixture _) : IClassFixture<XmlPrettyPrintFixture>
{
    private const string Host = "http://localhost:3000";
    private const string IssuesPage = """<issues type="array" total_count="3" offset="0" limit="1"></issues>""";
    private const string Errors = """<errors type="array"><error>Name cannot be blank</error></errors>""";

    private static (RedmineManager Manager, List<FakeWebClient> Clients) CreateManager(Func<FakeWebRequest, WebResponse> responder)
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

        return (new RedmineManager(builder), clients);
    }

    private static WebException UnprocessableEntity()
    {
        return new WebException("422", null, WebExceptionStatus.ProtocolError, FakeHttpWebResponse.Create((HttpStatusCode)422, Errors));
    }

    [Fact]
    public void Build_Should_Copy_ClientFunc()
    {
        Func<WebClient> clientFunc = () => new WebClient();

        var options = new RedmineManagerOptionsBuilder().WithHost(Host).WithWebClient(clientFunc).Build();

        Assert.Same(clientFunc, options.ClientFunc);
    }

    [Fact]
    public void Custom_WebClient_Should_Be_Used_With_BaseAddress()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse("<project><id>1</id><name>foo</name></project>"));

        var project = manager.Get<Project>("1");

        Assert.Equal("foo", project.Name);
        Assert.Single(clients);
        Assert.Equal($"{Host}/projects/1.xml", clients[0].Requests[0].RequestUri.ToString());
    }

    [Fact]
    public void Custom_WebClient_Func_Returning_Null_Should_Throw_RedmineException()
    {
        var manager = new RedmineManager(new RedmineManagerOptionsBuilder().WithHost(Host).WithWebClient(() => null));

        Assert.Throws<RedmineException>(() => manager.Get<Project>("1"));
    }

    [Fact]
    public void Count_Should_Not_Mutate_Caller_RequestOptions()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(IssuesPage));
        var requestOptions = new RequestOptions { QueryString = new NameValueCollection { { "status_id", "open" } } };

        var count = manager.Count<Issue>(requestOptions);

        Assert.Equal(3, count);
        Assert.Single(requestOptions.QueryString);
        Assert.Contains("limit=1", clients[0].Requests[0].RequestUri.Query);
    }

    [Fact]
    public async Task CountAsync_Should_Send_Limit_And_Not_Mutate_Caller_RequestOptions()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(IssuesPage));
        var requestOptions = new RequestOptions();

        var count = await manager.CountAsync<Issue>(requestOptions, TestContext.Current.CancellationToken);

        Assert.Equal(3, count);
        Assert.Null(requestOptions.QueryString);
        Assert.Contains("limit=1", clients[0].Requests[0].RequestUri.Query);
    }

    [Fact]
    public void Get_Should_Not_Mutate_Caller_RequestOptions()
    {
        var (manager, _) = CreateManager(_ => new FakeWebResponse(IssuesPage));
        var requestOptions = new RequestOptions { QueryString = new NameValueCollection { { "status_id", "open" } } };

        manager.Get<Issue>(requestOptions);

        Assert.Single(requestOptions.QueryString);
        Assert.Null(requestOptions.QueryString[RedmineKeys.LIMIT]);
        Assert.Null(requestOptions.QueryString[RedmineKeys.OFFSET]);
    }

    [Fact]
    public void Update_Should_Throw_On_Unprocessable_Entity()
    {
        var (manager, _) = CreateManager(_ => throw UnprocessableEntity());

        var exception = Assert.Throws<RedmineApiException>(() => manager.Update("1", new Project { Name = "foo" }));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_On_Unprocessable_Entity()
    {
        var (manager, _) = CreateManager(_ => throw UnprocessableEntity());

        var exception = await Assert.ThrowsAsync<RedmineApiException>(() => manager.DeleteAsync<Project>("1", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_On_Unprocessable_Entity()
    {
        var (manager, _) = CreateManager(_ => throw UnprocessableEntity());

        var exception = Assert.Throws<RedmineApiException>(() => manager.Create(new Project { Name = "foo" }));

        Assert.Equal(422, exception.HttpStatusCode);
        Assert.Equal("Name cannot be blank", exception.Message);
    }

    [Fact]
    public async Task ArchiveProjectAsync_Should_Put_To_Unescaped_Url()
    {
        var (manager, clients) = CreateManager(_ => new FakeWebResponse(string.Empty));

        await manager.ArchiveProjectAsync("foo", cancellationToken: TestContext.Current.CancellationToken);

        var request = clients[0].Requests[0];
        Assert.Equal("PUT", request.Method);
        Assert.Equal($"{Host}/projects/foo/archive.xml", request.RequestUri.ToString());
    }

    [Fact]
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

    [Fact]
    public void InternalWebClient_Should_Merge_DefaultHeaders_And_Bypass_Proxy_By_Default()
    {
        var request = CreateInternalWebRequest(
            new RedmineWebClientOptions { DefaultHeaders = new Dictionary<string, string> { { "X-Custom", "custom" } } },
            webClient => webClient.Headers.Add("X-Redmine-API-Key", "secret"));

        Assert.Equal("secret", request.Headers["X-Redmine-API-Key"]);
        Assert.Equal("custom", request.Headers["X-Custom"]);
        Assert.Null(request.Proxy);
    }

    [Fact]
    public void InternalWebClient_Should_Keep_Default_Proxy_When_UseProxy_Is_True()
    {
        var request = CreateInternalWebRequest(new RedmineWebClientOptions { UseProxy = true });

        Assert.Same(WebRequest.DefaultWebProxy, request.Proxy);
    }

    [Fact]
    public void InternalWebClient_Should_Use_Configured_Proxy()
    {
        var proxy = new WebProxy("http://proxy.local:8080");

        var request = CreateInternalWebRequest(new RedmineWebClientOptions { Proxy = proxy });

        Assert.Same(proxy, request.Proxy);
    }
}
