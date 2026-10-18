using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Net;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class HttpClientTransportContractTests : RedmineTransportContractTests, IDisposable
{
    private readonly List<HttpClient> _clients = [];

    protected override IRedmineTransport CreateTransport(string baseUrl, TimeSpan? timeout = null, Dictionary<string, string> defaultHeaders = null, string userAgent = null)
    {
        var client = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(30) };
        _clients.Add(client);
        foreach (var header in defaultHeaders ?? [])
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (userAgent != null)
        {
            client.DefaultRequestHeaders.Remove("User-Agent");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
        }

        return new HttpClientTransport(client);
    }

    protected override void AssertTimeout(Exception exception)
    {
        Assert.IsAssignableFrom<OperationCanceledException>(Assert.IsType<TimeoutException>(exception).InnerException);
    }

    [Fact]
    public async Task Header_Set_To_Null_Should_Be_Skipped()
    {
        using var server = new TestHttpServer(_ => new TestHttpResponse());
        var transport = CreateTransport(server.Url);
        var headers = new NameValueCollection { ["X-Null"] = null };

        await transport.SendAsync(Request("GET", server.Url + "projects/1.xml", headers), TestContext.Current.CancellationToken);

        Assert.Null(Assert.Single(server.Requests).Headers["X-Null"]);
    }

    public void Dispose()
    {
        foreach (var client in _clients)
        {
            client.Dispose();
        }
    }
}
