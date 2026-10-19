using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class ApiCoverageRequestTests
{
    private const string BaseAddress = "http://localhost:3000/";

    private sealed class RecordingTransport : IRedmineTransport
    {
        private readonly ConcurrentQueue<RedmineTransportRequest> _requests = new();

        public RedmineTransportRequest Single => Assert.Single(_requests.ToArray());

        public RedmineTransportRequest[] All => _requests.ToArray();

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            _requests.Enqueue(request);
            return new RedmineTransportResponse(204, null, null);
        }

        public Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }

    private static RedmineManager CreateManager(IRedmineTransport transport, SerializationType serializationType)
    {
        var builder = new RedmineManagerOptionsBuilder()
            .WithHost(BaseAddress)
            .WithApiKeyAuthentication("secret")
            .WithSerializationType(serializationType)
            .WithTransport(transport);
        return new RedmineManager(builder);
    }

    private static string Body(RedmineTransportRequest request)
    {
        return request.Body == null ? null : Encoding.UTF8.GetString(request.Body);
    }

    private static void AssertXml(string expected, RedmineTransportRequest request)
    {
        var body = Body(request);
        Assert.True(XNode.DeepEquals(XElement.Parse(expected), XElement.Parse(body)), body);
    }

    private static void AssertJson(string expected, RedmineTransportRequest request)
    {
        var body = Body(request);
        Assert.True(JToken.DeepEquals(JObject.Parse(expected), JObject.Parse(body)), body);
    }

    private static void AssertBody(SerializationType type, string expectedXml, string expectedJson, RedmineTransportRequest request)
    {
        if (type == SerializationType.Xml)
        {
            AssertXml(expectedXml, request);
        }
        else
        {
            AssertJson(expectedJson, request);
        }
    }

    private static string Ext(SerializationType type) => type == SerializationType.Xml ? "xml" : "json";

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml, false)]
    [InlineData(SerializationType.Xml, true)]
    [InlineData(SerializationType.Json, false)]
    [InlineData(SerializationType.Json, true)]
    public async Task UpdateJournal_Should_Put_Notes_And_PrivateNotes(SerializationType type, bool async)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);

        if (async)
        {
            await manager.UpdateJournalAsync(7, "new <notes>", false, cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            manager.UpdateJournal(7, "new <notes>", false);
        }

        var request = transport.Single;
        Assert.Equal("PUT", request.Method);
        Assert.Equal($"{BaseAddress}journals/7.{Ext(type)}", request.Uri.ToString());
        AssertBody(type,
            "<journal><notes>new &lt;notes&gt;</notes><private_notes>false</private_notes></journal>",
            """{"journal":{"notes":"new <notes>","private_notes":"false"}}""",
            request);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml)]
    [InlineData(SerializationType.Json)]
    public void UpdateJournal_Without_Notes_Should_Only_Send_PrivateNotes(SerializationType type)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);

        manager.UpdateJournal(7, null, true);

        AssertBody(type,
            "<journal><private_notes>true</private_notes></journal>",
            """{"journal":{"private_notes":"true"}}""",
            transport.Single);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml, false)]
    [InlineData(SerializationType.Xml, true)]
    [InlineData(SerializationType.Json, false)]
    [InlineData(SerializationType.Json, true)]
    public async Task DeleteJournalNotes_Should_Put_Empty_Notes(SerializationType type, bool async)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);

        if (async)
        {
            await manager.DeleteJournalNotesAsync(7, cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            manager.DeleteJournalNotes(7);
        }

        var request = transport.Single;
        Assert.Equal("PUT", request.Method);
        Assert.Equal($"{BaseAddress}journals/7.{Ext(type)}", request.Uri.ToString());
        AssertBody(type, "<journal><notes /></journal>", """{"journal":{"notes":""}}""", request);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task UpdateJournal_Without_Changes_Should_Throw_RedmineException()
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, SerializationType.Json);

        Assert.Throws<RedmineException>(() => manager.UpdateJournal(7, null));
        await Assert.ThrowsAsync<RedmineException>(() => manager.UpdateJournalAsync(7, null, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml, false)]
    [InlineData(SerializationType.Xml, true)]
    [InlineData(SerializationType.Json, false)]
    [InlineData(SerializationType.Json, true)]
    public async Task UpdateMyAccount_Should_Put_Account(SerializationType type, bool async)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);
        var account = new MyAccount
        {
            Id = 1,
            Login = "admin",
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            IsAdmin = true,
            ApiKey = "key",
            CustomFields =
            [
                new MyAccountCustomField { Id = 4, Name = "Phone", Value = "123" },
                new MyAccountCustomField { Id = 5, Name = "Skills", Multiple = true, Value = "a", Values = ["a", "b"] }
            ]
        };

        if (async)
        {
            await manager.UpdateMyAccountAsync(account, cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            manager.UpdateMyAccount(account);
        }

        var request = transport.Single;
        Assert.Equal("PUT", request.Method);
        Assert.Equal($"{BaseAddress}my/account.{Ext(type)}", request.Uri.ToString());
        AssertBody(type,
            """<user><firstname>Ada</firstname><lastname>Lovelace</lastname><mail>ada@example.com</mail><custom_fields type="array"><custom_field id="4"><value>123</value></custom_field><custom_field id="5"><value type="array"><value>a</value><value>b</value></value></custom_field></custom_fields></user>""",
            """{"user":{"firstname":"Ada","lastname":"Lovelace","mail":"ada@example.com","custom_fields":[{"id":4,"value":"123"},{"id":5,"value":["a","b"]}]}}""",
            request);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public async Task UpdateMyAccount_Null_Should_Throw_RedmineException()
    {
        var manager = CreateManager(new RecordingTransport(), SerializationType.Json);

        Assert.Throws<RedmineException>(() => manager.UpdateMyAccount(null));
        await Assert.ThrowsAsync<RedmineException>(() => manager.UpdateMyAccountAsync(null, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml, false)]
    [InlineData(SerializationType.Xml, true)]
    [InlineData(SerializationType.Json, false)]
    [InlineData(SerializationType.Json, true)]
    public async Task AddUsersToGroup_Should_Post_User_Ids(SerializationType type, bool async)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);

        if (async)
        {
            await manager.AddUsersToGroupAsync(5, [1, 2], cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            manager.AddUsersToGroup(5, [1, 2]);
        }

        var request = transport.Single;
        Assert.Equal("POST", request.Method);
        Assert.Equal($"{BaseAddress}groups/5/users.{Ext(type)}", request.Uri.ToString());
        AssertBody(type,
            """<user_ids type="array"><user_id>1</user_id><user_id>2</user_id></user_ids>""",
            """{"user_ids":[1,2]}""",
            request);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml, false)]
    [InlineData(SerializationType.Xml, true)]
    [InlineData(SerializationType.Json, false)]
    [InlineData(SerializationType.Json, true)]
    public async Task RemoveUsersFromGroup_Should_Delete_With_User_Ids(SerializationType type, bool async)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);

        if (async)
        {
            await manager.RemoveUsersFromGroupAsync(5, [1, 2], cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            manager.RemoveUsersFromGroup(5, [1, 2]);
        }

        var request = transport.Single;
        Assert.Equal("DELETE", request.Method);
        Assert.Equal($"/groups/5/users.{Ext(type)}?user_ids[]=1&user_ids[]=2", request.Uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped));
        Assert.Null(request.Body);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(SerializationType.Xml, false)]
    [InlineData(SerializationType.Xml, true)]
    [InlineData(SerializationType.Json, false)]
    [InlineData(SerializationType.Json, true)]
    public async Task AddWatchersToIssue_Should_Post_Watcher_User_Ids(SerializationType type, bool async)
    {
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, type);

        if (async)
        {
            await manager.AddWatchersToIssueAsync(9, [3, 4], cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            manager.AddWatchersToIssue(9, [3, 4]);
        }

        var request = transport.Single;
        Assert.Equal("POST", request.Method);
        Assert.Equal($"{BaseAddress}issues/9/watchers.{Ext(type)}", request.Uri.ToString());
        AssertBody(type,
            """<watcher><user_ids type="array"><user_id>3</user_id><user_id>4</user_id></user_ids></watcher>""",
            """{"watcher":{"user_ids":[3,4]}}""",
            request);
    }

    [Theory(Timeout = Constants.TestTimeout)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Bulk_Helpers_Without_User_Ids_Should_Throw_RedmineException(bool isNull)
    {
        var userIds = isNull ? null : Array.Empty<int>();
        var transport = new RecordingTransport();
        var manager = CreateManager(transport, SerializationType.Json);
        var ct = TestContext.Current.CancellationToken;

        Assert.Throws<RedmineException>(() => manager.AddUsersToGroup(5, userIds));
        Assert.Throws<RedmineException>(() => manager.RemoveUsersFromGroup(5, userIds));
        Assert.Throws<RedmineException>(() => manager.AddWatchersToIssue(9, userIds));
        await Assert.ThrowsAsync<RedmineException>(() => manager.AddUsersToGroupAsync(5, userIds, cancellationToken: ct));
        await Assert.ThrowsAsync<RedmineException>(() => manager.RemoveUsersFromGroupAsync(5, userIds, cancellationToken: ct));
        await Assert.ThrowsAsync<RedmineException>(() => manager.AddWatchersToIssueAsync(9, userIds, cancellationToken: ct));
        Assert.Empty(transport.All);
    }
}
