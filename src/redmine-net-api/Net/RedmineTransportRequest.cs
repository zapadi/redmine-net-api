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
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Net;

/// <summary>
/// An HTTP request sent through an <see cref="IRedmineTransport"/>.
/// </summary>
public sealed class RedmineTransportRequest
{
    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="method">The HTTP method (for example GET, POST, PUT, PATCH or DELETE).</param>
    /// <param name="uri">The absolute request URI, including the query string.</param>
    /// <param name="headers">The request headers; <c>null</c> for none.</param>
    /// <param name="contentType">The content type of <paramref name="body"/>; <c>null</c> when there is no body.</param>
    /// <param name="body">The request body; <c>null</c> when there is no body.</param>
    /// <param name="isDownload">Whether the request downloads a file.</param>
    /// <param name="progress">Receives the download progress percentage; <c>null</c> for none.</param>
    /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="uri"/> is <c>null</c>.</exception>
    /// <exception cref="RedmineException"><paramref name="uri"/> is not absolute.</exception>
    public RedmineTransportRequest(string method, Uri uri, NameValueCollection? headers = null, string? contentType = null, byte[]? body = null, bool isDownload = false, IProgress<int>? progress = null)
    {
        ArgumentNullThrowHelper.ThrowIfNull(method, nameof(method));
        ArgumentNullThrowHelper.ThrowIfNull(uri, nameof(uri));
        if (!uri.IsAbsoluteUri)
        {
            throw new RedmineException("The request URI must be absolute.");
        }

        Method = method;
        Uri = uri;
        Address = uri.AbsoluteUri;
        Headers = headers ?? new NameValueCollection();
        ContentType = contentType;
        Body = body;
        IsDownload = isDownload;
        Progress = progress;
    }

    internal RedmineTransportRequest(string method, string address, Uri uri)
    {
        Method = method;
        Address = address;
        Uri = uri;
    }

    /// <summary>
    /// Gets the HTTP method. Downloads use GET.
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// Gets the absolute request URI, including the query string.
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// Gets the request headers: authentication, impersonation and the <see cref="RequestOptions"/> headers, already merged.
    /// A transport must not override them with its own defaults.
    /// </summary>
    public NameValueCollection Headers { get; internal init; } = new NameValueCollection();

    /// <summary>
    /// Gets the content type of <see cref="Body"/>, or <c>null</c> when the request has no body.
    /// </summary>
    /// <remarks>
    /// This value is authoritative for the body: a transport must send it as the body content type
    /// and must not use a Content-Type entry in <see cref="Headers"/> for the body.
    /// </remarks>
    public string? ContentType { get; internal init; }

    /// <summary>
    /// Gets the request body, or <c>null</c> when the request has no body.
    /// </summary>
    public byte[]? Body { get; internal init; }

    /// <summary>
    /// Gets a value indicating whether the request downloads a file; the response body is returned as is.
    /// </summary>
    public bool IsDownload { get; internal init; }

    /// <summary>
    /// Gets the receiver of the download progress percentage, or <c>null</c>.
    /// </summary>
    public IProgress<int>? Progress { get; internal init; }

    internal string Address { get; }

    internal NameValueCollection? QueryString { get; init; }

    internal string? BaseAddress { get; set; }
}
