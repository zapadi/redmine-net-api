using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Http.Clients.WebClient;

internal static class WebClientExtensions
{
    public static void ApplyHeaders(this System.Net.WebClient client, RequestOptions options, IRedmineSerializer serializer)
    {
        client.Headers.Add(RedmineConstants.CONTENT_TYPE_HEADER_KEY, options.ContentType ?? serializer.ContentType);

        if (!options.UserAgent.IsNullOrWhiteSpace())
        {
            client.Headers.Add(RedmineConstants.USER_AGENT_HEADER_KEY, options.UserAgent);
        }

        if (!options.ImpersonateUser.IsNullOrWhiteSpace())
        {
            client.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, options.ImpersonateUser);
        }

        if (options.Headers is not { Count: > 0 })
        {
            return;
        }
        
        foreach (var header in options.Headers)
        {
            client.Headers.Add(header.Key, header.Value);
        }
    internal static HttpStatusCode? GetStatusCode(this System.Net.WebClient webClient)
    {
        if (webClient is InternalWebClient iwc)
        {
            return iwc.StatusCode;
        }
        
        return null;
    }
}