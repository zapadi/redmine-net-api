#if !NET20
using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace Redmine.Net.Api.Http.Clients.HttpClient;

internal static class HttpResponseHeadersExtensions
{
    public static NameValueCollection ToNameValueCollection(this HttpResponseHeaders headers)
    {
        if (headers == null) return null;
        
        var collection = new NameValueCollection();
        foreach (var header in headers)
        {
            var combinedValue = string.Join(", ", header.Value);
            collection.Add(header.Key, combinedValue);
        }
        return collection;
    }
}
#endif