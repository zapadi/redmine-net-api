using System;
using Redmine.Net.Api.Http.Constants;
#if !NET20
using System.Net.Http;
#endif

namespace Redmine.Net.Api.Http.Helpers;

internal static class RedmineHttpMethodHelper
{
#if !NET20
    private static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");
    private static readonly HttpMethod DownloadMethod = new HttpMethod("DOWNLOAD");

    /// <summary>
    /// Gets an HttpMethod instance for the specified HTTP verb.
    /// </summary>
    /// <param name="verb">The HTTP verb (GET, POST, etc.).</param>
    /// <returns>An HttpMethod instance corresponding to the verb.</returns>
    /// <exception cref="ArgumentException">Thrown when the verb is not supported.</exception>
    public static HttpMethod GetHttpMethod(string verb)
    {
        return verb switch
        {
            HttpConstants.HttpVerbs.GET => HttpMethod.Get,
            HttpConstants.HttpVerbs.POST => HttpMethod.Post,
            HttpConstants.HttpVerbs.PUT => HttpMethod.Put,
            HttpConstants.HttpVerbs.PATCH => PatchMethod,
            HttpConstants.HttpVerbs.DELETE => HttpMethod.Delete,
            HttpConstants.HttpVerbs.DOWNLOAD => DownloadMethod,
            _ => throw new ArgumentException($"Unsupported HTTP verb: {verb}")
        };
    }
#endif
    /// <summary>
    /// Determines whether the specified HTTP method is a GET or DOWNLOAD method.
    /// </summary>
    /// <param name="method">The HTTP method to check.</param>
    /// <returns>True if the method is GET or DOWNLOAD; otherwise, false.</returns>
    public static bool IsGetOrDownload(string method)
    {
        return method == HttpConstants.HttpVerbs.GET || method == HttpConstants.HttpVerbs.DOWNLOAD;
    }
    
    /// <summary>
    /// Determines whether the HTTP status code represents a transient error.
    /// </summary>
    /// <param name="statusCode">The HTTP response status code.</param>
    /// <returns>True if the status code represents a transient error; otherwise, false.</returns>
    private static bool IsTransientError(int statusCode)
    {
        return statusCode switch
        {
            HttpConstants.StatusCodes.BadGateway => true,
            HttpConstants.StatusCodes.GatewayTimeout => true,
            HttpConstants.StatusCodes.ServiceUnavailable => true,
            HttpConstants.StatusCodes.RequestTimeout => true,
            HttpConstants.StatusCodes.TooManyRequests => true,
            _ => false
        };
    }

}
