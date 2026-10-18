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
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;

namespace Redmine.Net.Api.Http;

/// <summary>
/// An <see cref="IRedmineTransport"/> that sends the requests with an <see cref="HttpClient"/>.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="HttpClient"/> belongs to the caller: the transport neither owns nor disposes it.
/// Timeouts, proxy, certificates, decompression and message handlers are configured on the <see cref="HttpClient"/> and its handler.
/// </para>
/// <para>
/// The request URI is absolute, so <see cref="HttpClient.BaseAddress"/> is not used.
/// <see cref="HttpClient.DefaultRequestHeaders"/> are sent only when the request does not already have a header with the same name.
/// </para>
/// <para>
/// <see cref="HttpClient.Timeout"/> covers a download until its response headers are received, not the reading of its body.
/// </para>
/// <para>
/// On .NET 5 and later, <see cref="Send"/> uses the synchronous <c>HttpClient.Send</c>: message handlers that override only
/// <c>SendAsync</c> are skipped by synchronous calls, and a handler without synchronous support throws.
/// On .NET Framework, <see cref="Send"/> runs the asynchronous send on the thread pool.
/// </para>
/// </remarks>
public sealed class HttpClientTransport : IRedmineTransport
{
    private const string ContentTypeHeader = "Content-Type";
    private const string ContentLengthHeader = "Content-Length";
    private const string UnknownErrorCode = "UNKNOWN";
    private const int BufferSize = 81920;

    private readonly HttpClient _httpClient;

    /// <summary>
    /// Creates a transport that sends the requests with <paramref name="httpClient"/>.
    /// </summary>
    /// <param name="httpClient">The HTTP client; it is not disposed by the transport.</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is <c>null</c>.</exception>
    public HttpClientTransport(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
    public RedmineTransportResponse Send(RedmineTransportRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

#if NET5_0_OR_GREATER
        try
        {
            using var message = CreateRequestMessage(request);
            using var response = _httpClient.Send(message, GetCompletionOption(request));
            return CreateResponse(response, ReadBody(response, request));
        }
        catch (OperationCanceledException exception)
        {
            throw CreateTimeoutException(exception);
        }
        catch (HttpRequestException exception)
        {
            throw MapHttpRequestException(exception);
        }
        catch (IOException exception)
        {
            throw CreateTransientException(exception);
        }
#else
        return Task.Run(() => SendAsync(request, CancellationToken.None)).GetAwaiter().GetResult();
#endif
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
    public async Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            using var message = CreateRequestMessage(request);
            using var response = await _httpClient.SendAsync(message, GetCompletionOption(request), cancellationToken).ConfigureAwait(false);
            var body = await ReadBodyAsync(response, request, cancellationToken).ConfigureAwait(false);
            return CreateResponse(response, body);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw CreateTimeoutException(exception);
        }
        catch (HttpRequestException exception)
        {
            throw MapHttpRequestException(exception);
        }
        catch (IOException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw CreateTransientException(exception);
        }
    }

    private static HttpCompletionOption GetCompletionOption(RedmineTransportRequest request)
    {
        return request.IsDownload ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead;
    }

    private static HttpRequestMessage CreateRequestMessage(RedmineTransportRequest request)
    {
        var message = new HttpRequestMessage(new HttpMethod(request.Method), request.Uri);
        if (request.Body is { } body)
        {
            message.Content = new ByteArrayContent(body);
            if (request.ContentType is { } contentType)
            {
                message.Content.Headers.TryAddWithoutValidation(ContentTypeHeader, contentType);
            }
        }

        foreach (var name in request.Headers.AllKeys)
        {
            if (name is null
                || string.Equals(name, ContentTypeHeader, StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, ContentLengthHeader, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var values = request.Headers.GetValues(name);
            if (values is { Length: > 0 } && values[values.Length - 1] is { } value && !message.Headers.TryAddWithoutValidation(name, value))
            {
                message.Content?.Headers.TryAddWithoutValidation(name, value);
            }
        }

        return message;
    }

    private static RedmineTransportResponse CreateResponse(HttpResponseMessage response, byte[]? body)
    {
        var headers = new NameValueCollection();
        AddHeaders(headers, response.Headers);
        if (response.Content is { } content)
        {
            AddHeaders(headers, content.Headers);
        }

        return new RedmineTransportResponse((int)response.StatusCode, headers, body);
    }

    private static void AddHeaders(NameValueCollection target, HttpHeaders source)
    {
        foreach (var header in source)
        {
            foreach (var value in header.Value)
            {
                target.Add(header.Key, value);
            }
        }
    }

    private static async Task<byte[]?> ReadBodyAsync(HttpResponseMessage response, RedmineTransportRequest request, CancellationToken cancellationToken)
    {
        if (response.Content is not { } content)
        {
            return null;
        }

#if NET5_0_OR_GREATER
        using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
        using var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
        var progress = request.IsDownload ? request.Progress : null;
        var length = content.Headers.ContentLength;
        using var output = new MemoryStream();
        var buffer = new byte[BufferSize];
        int read;
        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            output.Write(buffer, 0, read);
            Report(progress, output.Length, length);
        }

        return output.ToArray();
    }

#if NET5_0_OR_GREATER
    private static byte[]? ReadBody(HttpResponseMessage response, RedmineTransportRequest request)
    {
        if (response.Content is not { } content)
        {
            return null;
        }

        using var stream = content.ReadAsStream();
        var progress = request.IsDownload ? request.Progress : null;
        var length = content.Headers.ContentLength;
        using var output = new MemoryStream();
        var buffer = new byte[BufferSize];
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, read);
            Report(progress, output.Length, length);
        }

        return output.ToArray();
    }
#endif

    private static void Report(IProgress<int>? progress, long received, long? length)
    {
        if (progress != null && length is > 0)
        {
            progress.Report((int)Math.Min(100, received * 100 / length.Value));
        }
    }

    private static TimeoutException CreateTimeoutException(OperationCanceledException exception)
    {
        return new TimeoutException("The request timed out.", exception);
    }

    private static RedmineApiException CreateTransientException(Exception exception)
    {
        return new RedmineApiException(exception.Message, exception, UnknownErrorCode, true);
    }

    private static Exception MapHttpRequestException(HttpRequestException exception)
    {
        var isTransient = false;
        for (var inner = exception.InnerException; inner != null; inner = inner.InnerException)
        {
            switch (inner)
            {
                case WebException webException:
                    return webException;
                case SocketException socketException:
                    return new RedmineApiException(exception.Message, exception, UnknownErrorCode, !IsNameResolutionFailure(socketException.SocketErrorCode));
                case IOException:
                    isTransient = true;
                    break;
            }
        }

        return new RedmineApiException(exception.Message, exception, UnknownErrorCode, isTransient);
    }

    private static bool IsNameResolutionFailure(SocketError error)
    {
        return error is SocketError.HostNotFound or SocketError.NoData;
    }
}
