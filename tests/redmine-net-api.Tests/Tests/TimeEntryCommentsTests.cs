using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class TimeEntryCommentsTests
{
    private const string BaseAddress = "http://localhost:3000/";
    private const string TimeEntryXml = "<time_entry><id>1</id></time_entry>";

    private sealed class FakeTransport : IRedmineTransport
    {
        private readonly ConcurrentQueue<RedmineTransportRequest> _requests = new();

        public RedmineTransportRequest[] Requests => _requests.ToArray();

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            _requests.Enqueue(request);
            return request.Method == "PUT"
                ? new RedmineTransportResponse(204, null, null)
                : new RedmineTransportResponse(201, new NameValueCollection { { "Content-Type", "application/xml; charset=utf-8" } }, Encoding.UTF8.GetBytes(TimeEntryXml));
        }

        public Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }

    [Fact]
    public void Builder_Should_Default_Time_Entry_Comments_Max_Length_To_255()
    {
        var options = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).Build();

        Assert.Equal(255, options.TimeEntryCommentsMaxLength);
    }

    [Fact]
    public void Builder_Should_Use_Custom_Time_Entry_Comments_Max_Length()
    {
        var options = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).WithTimeEntryCommentsMaxLength(1024).Build();

        Assert.Equal(1024, options.TimeEntryCommentsMaxLength);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Builder_Should_Throw_When_Time_Entry_Comments_Max_Length_Is_Not_Positive(int maxLength)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RedmineManagerOptionsBuilder().WithTimeEntryCommentsMaxLength(maxLength));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(false, null, 255)]
    [InlineData(true, null, 255)]
    [InlineData(false, 300, 300)]
    [InlineData(true, 300, 300)]
    [InlineData(false, 100, 100)]
    [InlineData(true, 100, 100)]
    public async Task Create_And_Update_Should_Truncate_Time_Entry_Comments_To_The_Configured_Length(bool async, int? maxLength, int expectedLength)
    {
        var transport = new FakeTransport();
        var builder = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).WithApiKeyAuthentication("secret").WithTransport(transport);
        if (maxLength.HasValue)
        {
            builder.WithTimeEntryCommentsMaxLength(maxLength.Value);
        }
        var manager = new RedmineManager(builder);
        var comments = new string('a', 400);
        var created = new TimeEntry { Hours = 1, Comments = comments };
        var updated = new TimeEntry { Hours = 1, Comments = comments };
        var ct = TestContext.Current.CancellationToken;

        if (async)
        {
            await manager.CreateAsync(created, cancellationToken: ct);
            await manager.UpdateAsync("1", updated, cancellationToken: ct);
        }
        else
        {
            manager.Create(created);
            manager.Update("1", updated);
        }

        var expected = new XmlRedmineSerializer().Serialize(new TimeEntry { Hours = 1, Comments = new string('a', expectedLength) });
        Assert.Equal([expected, expected.ReplaceEndings()], transport.Requests.Select(r => Encoding.UTF8.GetString(r.Body ?? [])));
    }
}
