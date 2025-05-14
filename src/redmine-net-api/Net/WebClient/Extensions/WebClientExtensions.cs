using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net.WebClient.Extensions;

internal static class WebClientExtensions
{
    public static void ApplyHeaders(this System.Net.WebClient client, RequestOptions options, IRedmineSerializer serializer)
    {
        client.Headers.Add("Content-Type", options.ContentType ?? serializer.ContentType);

        if (!options.UserAgent.IsNullOrWhiteSpace())
        {
            client.Headers.Add("User-Agent", options.UserAgent);
        }

        if (!options.ImpersonateUser.IsNullOrWhiteSpace())
        {
            client.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, options.ImpersonateUser);
        }

        if (options.Headers is { Count: > 0 })
        {
            foreach (var header in options.Headers)
            {
                client.Headers.Add(header.Key, header.Value);
            }
        }
    }
}