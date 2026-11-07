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
using System.Net.Http;

namespace Redmine.Net.Api.Http;

/// <summary>
/// Extensions that plug the <see cref="HttpClientTransport"/> into a <see cref="RedmineManagerOptionsBuilder"/>.
/// </summary>
public static class RedmineManagerOptionsBuilderExtensions
{
    /// <summary>
    /// Sends the requests with <paramref name="httpClient"/> through a <see cref="HttpClientTransport"/>.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="httpClient">The HTTP client; it belongs to the caller and is not disposed by the library.</param>
    /// <returns>The current builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="httpClient"/> is <c>null</c>.</exception>
    /// <remarks>Cannot be combined with <c>WithWebClient</c> or <c>WithWebClientOptions</c>.</remarks>
    public static RedmineManagerOptionsBuilder WithHttpClient(this RedmineManagerOptionsBuilder builder, HttpClient httpClient)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithTransport(new HttpClientTransport(httpClient));
    }
}
