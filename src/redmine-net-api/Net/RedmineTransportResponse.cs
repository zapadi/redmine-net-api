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

namespace Redmine.Net.Api.Net;

/// <summary>
/// An HTTP response returned by an <see cref="IRedmineTransport"/>.
/// </summary>
public sealed class RedmineTransportResponse
{
    /// <summary>
    /// Creates a response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="headers">The response headers; <c>null</c> for none.</param>
    /// <param name="body">The response body; <c>null</c> for none. Text is decoded with the charset of the Content-Type header, or UTF-8.</param>
    public RedmineTransportResponse(int statusCode, NameValueCollection? headers, byte[]? body)
    {
        StatusCode = statusCode;
        Headers = headers;
        Body = body;
        IsSuccessStatusCode = statusCode is >= 200 and <= 299;
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the response headers, or <c>null</c>.
    /// </summary>
    public NameValueCollection? Headers { get; }

    /// <summary>
    /// Gets the response body, or <c>null</c>.
    /// </summary>
    public byte[]? Body { get; }

    /// <summary>
    /// Gets a value indicating whether <see cref="StatusCode"/> is in the 2xx range.
    /// </summary>
    public bool IsSuccessStatusCode { get; internal init; }

    internal string? Content { get; init; }

    internal Exception? Error { get; init; }

    internal Exception? BodyError { get; init; }
}
