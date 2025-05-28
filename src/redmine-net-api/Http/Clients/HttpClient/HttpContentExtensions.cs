#if !NET20

using System.Net;

namespace Redmine.Net.Api.Http.Clients.HttpClient;

internal static class HttpContentExtensions
{
    public static bool IsUnprocessableEntity(this HttpStatusCode statusCode)
    {
        return
#if NET5_0_OR_GREATER
         statusCode == HttpStatusCode.UnprocessableEntity;
#else
         (int)statusCode == 422;
#endif
    }
}

#endif