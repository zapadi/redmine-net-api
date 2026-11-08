#if NETFRAMEWORK
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public class RedmineApiExceptionSerializationTests
{
    [Fact]
    public void Should_Round_Trip_Request_Details()
    {
        var exception = new RedmineApiException("boom", null, "E1", true, "abc", "PUT", "http://localhost:3000/projects/1.xml", 503,
            new Dictionary<string, string> { { "X-Request-Id", "abc" } }, ["Name cannot be blank", "Identifier is too long"]);

        var copy = RoundTrip(exception);

        Assert.Equal("boom", copy.Message);
        Assert.Equal(503, copy.HttpStatusCode);
        Assert.Equal("PUT", copy.Method);
        Assert.Equal("http://localhost:3000/projects/1.xml", copy.Endpoint);
        Assert.Equal("abc", copy.CorrelationId);
        Assert.Equal("E1", copy.ErrorCode);
        Assert.True(copy.IsTransient);
        Assert.Equal(exception.ResponseHeaders, copy.ResponseHeaders);
        Assert.Equal(["Name cannot be blank", "Identifier is too long"], copy.Errors);
    }

    [Fact]
    public void Should_Round_Trip_Without_Status_Code()
    {
        var exception = new RedmineApiException("boom", "E1", false);

        var copy = RoundTrip(exception);

        Assert.Null(copy.HttpStatusCode);
        Assert.Equal("E1", copy.ErrorCode);
        Assert.False(copy.IsTransient);
        Assert.Null(copy.ResponseHeaders);
        Assert.Empty(copy.Errors);
    }

    private static RedmineApiException RoundTrip(RedmineApiException exception)
    {
        using var stream = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, exception);
        stream.Position = 0;
        return (RedmineApiException)formatter.Deserialize(stream);
    }
}
#endif
