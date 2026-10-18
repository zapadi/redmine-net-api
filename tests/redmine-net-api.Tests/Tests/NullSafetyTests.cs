using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
using Xunit;

#pragma warning disable SYSLIB0014
namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class NullSafetyTests
{
    private sealed class FixedTransport(RedmineTransportResponse response) : IRedmineTransport
    {
        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            return response;
        }

        public Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(response);
        }
    }

    [Fact]
    public void Error_Response_Headers_With_Null_Key_Or_Value_Are_Handled()
    {
        var headers = new NameValueCollection { { null, "orphan" }, { "X-Empty", null }, { "X-Request-Id", "abc" } };
        var options = new RedmineManagerOptionsBuilder().WithHost("http://localhost:3000").Build();
        var client = new RedmineApiClient(new FixedTransport(new RedmineTransportResponse(404, headers, null)), new RedmineNoAuthentication(), options.Serializer);

        var exception = Assert.ThrowsAny<RedmineApiException>(() => client.Get("projects/1.xml"));

        Assert.Equal(404, exception.HttpStatusCode);
        Assert.Equal("abc", exception.ResponseHeaders["X-Request-Id"]);
        Assert.Equal(string.Empty, exception.ResponseHeaders["X-Empty"]);
        Assert.Equal(2, exception.ResponseHeaders.Count);
    }

    [Fact]
    public void Custom_Web_Client_Without_Base_Address_Option_Does_Not_Throw()
    {
        using var server = new TestHttpServer(_ => RedminePagedServer.Response("<project><id>1</id></project>"));
        var transport = new WebClientTransport(new RedmineManagerOptions
        {
            ClientFunc = () => new System.Net.WebClient(),
            WebClientOptions = new RedmineWebClientOptions(),
        });

        var response = transport.Send(new RedmineTransportRequest(HttpVerbs.GET, new Uri(server.Url + "projects/1.xml")));

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("/projects/1.xml", server.Requests.Single().RawUrl);
    }
}
