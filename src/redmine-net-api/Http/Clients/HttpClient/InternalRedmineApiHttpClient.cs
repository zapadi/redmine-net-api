#if !NET20
/*
   Copyright 2011 - 2024 Adrian Popescu

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

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Http.Messages;
using Redmine.Net.Api.Options;

namespace Redmine.Net.Api.Http.Clients.HttpClient;

internal sealed partial class InternalRedmineApiHttpClient : RedmineApiClient
{
    private static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;

    private readonly System.Net.Http.HttpClient _httpClient;

    public InternalRedmineApiHttpClient(RedmineManagerOptions redmineManagerOptions)
        : this(null, redmineManagerOptions)
    {
        _httpClient = HttpClientProvider.GetOrCreateHttpClient(null, redmineManagerOptions);
    }

    public InternalRedmineApiHttpClient(System.Net.Http.HttpClient httpClient,
        RedmineManagerOptions redmineManagerOptions)
        : base(redmineManagerOptions)
    {
        _httpClient = httpClient;
    }

    protected override object CreateContentFromPayload(string payload)
    {
        return new StringContent(payload, DefaultEncoding, Serializer.ContentType);
    }

    protected override object CreateContentFromBytes(byte[] data)
    {
        var content = new ByteArrayContent(data);
        content.Headers.ContentType = new MediaTypeHeaderValue(RedmineConstants.CONTENT_TYPE_APPLICATION_STREAM);
        return content;
    }

    protected override RedmineApiResponse HandleRequest(string address, string verb,
        RequestOptions requestOptions = null,
        object content = null, IProgress<int> progress = null)
    {
        var httpMethod = GetHttpMethod(verb);
        using (var requestMessage = CreateRequestMessage(address, httpMethod, requestOptions, content as HttpContent))
        {
            var response = Send(requestMessage, progress);
            return response;
        }
    }
    
    private RedmineApiResponse Send(HttpRequestMessage requestMessage, IProgress<int> progress = null)
    {
        return TaskExtensions.Synchronize(() => SendAsync(requestMessage, progress));
    }

    private HttpRequestMessage CreateRequestMessage(string address, HttpMethod method,
        RequestOptions requestOptions = null, HttpContent content = null)
    {
        var httpRequest = new HttpRequestMessage(method, address);
        
        switch (Credentials)
        {
            case RedmineApiKeyAuthentication:
                httpRequest.Headers.Add(RedmineConstants.API_KEY_AUTHORIZATION_HEADER_KEY, Credentials.Token);
                break;
            case RedmineBasicAuthentication:
                httpRequest.Headers.Add(RedmineConstants.AUTHORIZATION_HEADER_KEY, Credentials.Token);
                break;
        }

        if (requestOptions != null)
        {
            if (requestOptions.QueryString != null)
            {
                var uriToBeAppended = httpRequest.RequestUri.ToString();
                var queryIndex = uriToBeAppended.IndexOf("?", StringComparison.Ordinal);
                var hasQuery = queryIndex != -1;

                var sb = new StringBuilder();
                sb.Append('\\');
                sb.Append(uriToBeAppended);
                for (var index = 0; index < requestOptions.QueryString.Count; ++index)
                {
                    var value = requestOptions.QueryString[index];

                    if (value == null)
                    {
                        continue;
                    }

                    var key = requestOptions.QueryString.Keys[index];

                    sb.Append(hasQuery ? '&' : '?');
                    sb.Append(Uri.EscapeDataString(key));
                    sb.Append('=');
                    sb.Append(Uri.EscapeDataString(value));
                    hasQuery = true;
                }

                var uriString = sb.ToString();

                httpRequest.RequestUri = new Uri(uriString, UriKind.RelativeOrAbsolute);
            }

            if (!requestOptions.ImpersonateUser.IsNullOrWhiteSpace())
            {
                httpRequest.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, requestOptions.ImpersonateUser);
            }
            
            if (!requestOptions.Accept.IsNullOrWhiteSpace())
            {
                httpRequest.Headers.Accept.ParseAdd(requestOptions.Accept);
            }
        
            if (!requestOptions.UserAgent.IsNullOrWhiteSpace())
            {
                httpRequest.Headers.UserAgent.ParseAdd(requestOptions.UserAgent);
            }
        
            if (requestOptions.Headers != null)
            {
                foreach (var header in requestOptions.Headers)
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        if (content == null)
        {
            return httpRequest;
        }
        
        httpRequest.Content = content;
        if (requestOptions?.ContentType != null)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue(requestOptions.ContentType);
        }

        return httpRequest;
    }

    private static RedmineApiResponse CreateApiResponseMessage(HttpResponseHeaders headers, HttpStatusCode statusCode, byte[] content) => new RedmineApiResponse()
    {
        Content = content,
        Headers = headers.ToNameValueCollection(),
        StatusCode = statusCode,
    };
    
    private static HttpMethod GetHttpMethod(string verb)
    {
        return verb switch
        {
            HttpConstants.HttpVerbs.GET => HttpMethod.Get,
            HttpConstants.HttpVerbs.POST => HttpMethod.Post,
            HttpConstants.HttpVerbs.PUT => HttpMethod.Put,
            HttpConstants.HttpVerbs.PATCH => PatchMethod,
            HttpConstants.HttpVerbs.DELETE => HttpMethod.Delete,
            HttpConstants.HttpVerbs.DOWNLOAD => HttpMethod.Get,
            _ => throw new ArgumentException($"Unsupported HTTP verb: {verb}")
        };
    }
}
#endif