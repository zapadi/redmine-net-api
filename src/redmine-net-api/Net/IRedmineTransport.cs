/*
   Copyright 2011 - 2025 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

#nullable enable
#if !(NET20)
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Redmine.Net.Api.Net;

/// <summary>
/// Sends the HTTP requests built by <see cref="RedmineManager"/> and returns the raw responses.
/// Plug a custom implementation in with <see cref="RedmineManagerOptionsBuilder.WithTransport(IRedmineTransport)"/>.
/// </summary>
/// <remarks>
/// <para>
/// The request is complete: <see cref="RedmineTransportRequest.Uri"/> is absolute and already contains the query string,
/// and <see cref="RedmineTransportRequest.Headers"/> already contains authentication, impersonation and the
/// <see cref="RequestOptions"/> headers. A transport may add its own default headers only when the request does not
/// already have a header with the same name; it must never override a request header.
/// </para>
/// <para>
/// A transport must never throw for an HTTP status code; it returns every response, including 4xx and 5xx,
/// and the library maps it to a <see cref="Exceptions.RedmineApiException"/>.
/// It throws only for transport failures: a <see cref="Exceptions.RedmineApiException"/> (set
/// <see cref="Exceptions.RedmineApiException.IsTransient"/> for failures that can be retried) or an
/// <see cref="System.OperationCanceledException"/> when the caller cancels the request.
/// A <see cref="System.TimeoutException"/>, or a cancellation caused by one, is reported as a transient timeout;
/// any other exception is wrapped in a <see cref="Exceptions.RedmineApiException"/>.
/// On frameworks where an HttpClient timeout surfaces as a <see cref="System.Threading.Tasks.TaskCanceledException"/>
/// without an inner <see cref="System.TimeoutException"/>, throw a <see cref="System.TimeoutException"/> (or a
/// <see cref="Exceptions.RedmineApiException"/> with <see cref="Exceptions.RedmineApiException.IsTransient"/> set) yourself;
/// otherwise the timeout is reported as a non-transient failure.
/// </para>
/// <para>
/// A transport may ignore <see cref="RedmineTransportRequest.Progress"/>.
/// </para>
/// <para>
/// A transport owns its timeouts, proxy and certificate configuration, and must be thread-safe:
/// the same instance sends requests concurrently (for example when GetAsync fetches pages in parallel).
/// </para>
/// </remarks>
#if NET20
internal interface IRedmineTransport
#else
public interface IRedmineTransport
#endif
{
    /// <summary>
    /// Sends the request and returns the response, whatever its status code.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <returns>The response.</returns>
    RedmineTransportResponse Send(RedmineTransportRequest request);

#if !(NET20)
    /// <summary>
    /// Sends the request asynchronously and returns the response, whatever its status code.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The caller's cancellation token.</param>
    /// <returns>The response.</returns>
    Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken);
#endif
}
