#if NET45_OR_GREATER || NETCOREAPP
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net.HttpClient.Extensions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net.HttpClient;

internal sealed partial class InternalRedmineApiHttpClient
{
    private static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");
    private static readonly HttpMethod DownloadMethod = new HttpMethod("DOWNLOAD");
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;
    
    private readonly IRedmineAuthentication _credentials;
    private readonly IRedmineSerializer _serializer;
    private readonly System.Net.Http.HttpClient _httpClient;

    public InternalRedmineApiHttpClient(RedmineManagerOptions redmineManagerOptions)
        : this(() => HttpClientProvider.Client(redmineManagerOptions), 
            redmineManagerOptions.Authentication, 
            redmineManagerOptions.Serializer)
    {
    }

    public InternalRedmineApiHttpClient(
        Func<System.Net.Http.HttpClient> httpClientFunc, 
        IRedmineAuthentication authentication, 
        IRedmineSerializer serializer)
    {
        _credentials = authentication;
        _serializer = serializer;
        _httpClient = httpClientFunc();
    }
    
    public async Task<ApiResponseMessage> GetAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpMethod.Get, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await GetAsync(address, requestOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApiResponseMessage> CreateAsync(string address, string payload, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var content = new StringContent(payload, DefaultEncoding, GetContentType(_serializer));
        return await HandleRequestAsync(address, HttpMethod.Post, requestOptions, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApiResponseMessage> UpdateAsync(string address, string payload, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var content = new StringContent(payload, DefaultEncoding, GetContentType(_serializer));
        return await HandleRequestAsync(address, HttpMethod.Put, requestOptions, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApiResponseMessage> PatchAsync(string address, string payload, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var content = new StringContent(payload, DefaultEncoding, GetContentType(_serializer));
        return await HandleRequestAsync(address, PatchMethod, requestOptions, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpMethod.Delete, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var content = new StreamContent(stream))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue(RedmineConstants.CONTENT_TYPE_APPLICATION_STREAM);
                return await HandleRequestAsync(address, HttpMethod.Post, requestOptions, content, cancellationToken).ConfigureAwait(false);
            }
        }
    }
    
    public async Task<ApiResponseMessage> UploadFileAsync(string address, Stream content, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using (var streamContent = new StreamContent(content))
        {
            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue(RedmineConstants.CONTENT_TYPE_APPLICATION_STREAM);
            return await HandleRequestAsync(address, HttpMethod.Post, requestOptions, streamContent, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, DownloadMethod, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    private async Task<ApiResponseMessage> HandleRequestAsync(string address, HttpMethod method, RequestOptions requestOptions = null, HttpContent content = null, CancellationToken cancellationToken = default)
    {
        using (var requestMessage = CreateRequestMessage(address, method, requestOptions, content))
        {
            return await SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<ApiResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            using (var httpResponseMessage = await _httpClient
                       .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                       .ConfigureAwait(false))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (requestMessage.Method != DownloadMethod)
                    {
                        var contentLength = httpResponseMessage.Content.Headers.ContentLength;
                        if (contentLength.HasValue)
                        {
                            using var stream = await httpResponseMessage.Content.ReadAsStreamAsync(
#if !NETFRAMEWORK
                                cancellationToken
#endif
                            ).ConfigureAwait(false);
                            var buffer = new byte[(int)contentLength.Value];
                            var _ = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                            return new ApiResponseMessage()
                            {
                                Content = buffer,
                                // Headers = httpResponseMessage.Headers,
                            };
                        }
                        var data = await httpResponseMessage.Content.ReadAsByteArrayAsync(
#if !NETFRAMEWORK
                                cancellationToken
#endif
                        ).ConfigureAwait(false);
                        return new ApiResponseMessage()
                        {
                            Content = data,
                            // Headers = httpResponseMessage.Headers,
                        };
                    }
                    else
                    {
                        //download
                    }
                }
                else
                {
#if NET5_0_OR_GREATER
                    if (httpResponseMessage.StatusCode == HttpStatusCode.UnprocessableEntity)
#else
        if ((int)httpResponseMessage.StatusCode == 422)
#endif
        {
            await httpResponseMessage.HandleUnprocessableResponse(_serializer, cancellationToken).ConfigureAwait(false);
        }
                    else
                    {
                        throw new RedmineApiException(httpResponseMessage.ReasonPhrase, httpResponseMessage.StatusCode.ToString(),  IsTransient(httpResponseMessage));
                    }
                }
            }
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            // When the token has been canceled, it is not a timeout.
            throw new RedmineApiException(ex.Message, ex);
        }
        catch (OperationCanceledException ex) when (ex.InnerException is TimeoutException tex)
        {
            throw new RedmineApiException(tex.Message, ex);
        }
        catch (TaskCanceledException tcex) when (cancellationToken.IsCancellationRequested)
        {
            // User canceled request
            throw new RedmineApiException(tcex.Message, tcex);
        }
        catch (TaskCanceledException tce)
        {
            throw new RedmineApiException(tce.Message, tce);
        }
        catch (HttpRequestException ex)
        {
            ex.HandleApiRequestException(_serializer);
        }
        catch (Exception ex) when (!(ex is RedmineException))
        {
            throw new RedmineApiException(ex.Message, ex);
        }

        return null;
    }
    
    private HttpRequestMessage CreateRequestMessage(string address, HttpMethod method, RequestOptions requestOptions = null, HttpContent content = null)
    {
        var httpRequest = new HttpRequestMessage(method, address);
        
        switch (_credentials)
        {
            case RedmineApiKeyAuthentication:
                httpRequest.Headers.Add(_credentials.AuthenticationType,_credentials.Token);
                break;
            case RedmineBasicAuthentication:
                httpRequest.Headers.Add(RedmineConstants.AUTHORIZATION_HEADER_KEY, $"{_credentials.AuthenticationType} {_credentials.Token}");
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
                var x = sb.ToString();
                
                httpRequest.RequestUri = new Uri(x, UriKind.RelativeOrAbsolute);
            }
        
            if (!requestOptions.ImpersonateUser.IsNullOrWhiteSpace())
            {
                httpRequest.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, requestOptions.ImpersonateUser);
            }
        }

        if (content != null)
        {
            httpRequest.Content = content;
        }

        return httpRequest;
    }

    private static string GetContentType(IRedmineSerializer serializer)
    {
        return serializer.Format == RedmineConstants.XML 
            ? RedmineConstants.CONTENT_TYPE_APPLICATION_XML 
            : RedmineConstants.CONTENT_TYPE_APPLICATION_JSON;
    }

    private static bool IsTransient(HttpResponseMessage httpResponseMessage)
    {
        return (int)httpResponseMessage.StatusCode switch
        {
            // 502 Bad Gateway
            (int)HttpStatusCode.BadGateway => true,
            // 504 Gateway Timeout
            (int)HttpStatusCode.GatewayTimeout => true,
            // 503 Service Unavailable
            (int)HttpStatusCode.ServiceUnavailable => true,
            // 408 Request Timeout
            (int)HttpStatusCode.RequestTimeout => true,
            // 429 Too Many Requests
            429 => true,
            _ => false
        };
    }
}
#endif