using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

internal static class RedminePagedServer
{
    public const int ServerMaxLimit = 100;

    public static NameValueCollection ParseQuery(string rawUrl)
    {
        var query = new NameValueCollection();
        var index = rawUrl.IndexOf('?');
        if (index < 0)
        {
            return query;
        }

        foreach (var pair in rawUrl.Substring(index + 1).Split(['&'], StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split(['='], 2);
            query.Add(Uri.UnescapeDataString(parts[0]), parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty);
        }

        return query;
    }

    public static TestHttpResponse Issues(RecordedRequest request, int total, bool json = false)
    {
        var query = ParseQuery(request.RawUrl);
        var limit = int.TryParse(query["limit"], out var l) && l > 0 ? Math.Min(l, ServerMaxLimit) : 25;
        var offset = int.TryParse(query["offset"], out var o) && o > 0 ? o : 0;
        var ids = Enumerable.Range(offset + 1, Math.Max(0, Math.Min(limit, total - offset))).ToList();

        var body = json
            ? $"{{\"issues\":[{string.Join(",", ids.Select(id => $"{{\"id\":{id}}}"))}],\"total_count\":{total},\"offset\":{offset},\"limit\":{limit}}}"
            : $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><issues type=\"array\" total_count=\"{total}\" offset=\"{offset}\" limit=\"{limit}\">{string.Concat(ids.Select(id => $"<issue><id>{id}</id></issue>"))}</issues>";

        return Response(body, json);
    }

    public static TestHttpResponse IssueStatuses(int count, bool json = false)
    {
        var ids = Enumerable.Range(1, count).ToList();
        var body = json
            ? $"{{\"issue_statuses\":[{string.Join(",", ids.Select(id => $"{{\"id\":{id},\"name\":\"S{id}\"}}"))}]}}"
            : $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><issue_statuses type=\"array\">{string.Concat(ids.Select(id => $"<issue_status><id>{id}</id><name>S{id}</name></issue_status>"))}</issue_statuses>";

        return Response(body, json);
    }

    public static TestHttpResponse Response(string body, bool json = false)
    {
        return new TestHttpResponse
        {
            ContentType = json ? "application/json; charset=utf-8" : "application/xml; charset=utf-8",
            Body = Encoding.UTF8.GetBytes(body),
            Headers = new Dictionary<string, string>(),
        };
    }
}
