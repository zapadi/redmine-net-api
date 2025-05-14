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

#if!(NET20)
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net.WebClient.MessageContent;

namespace Redmine.Net.Api.Net.WebClient
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed partial class InternalRedmineApiWebClient
    {
        public async Task<ApiResponseMessage> GetAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.GET, requestOptions, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default) 
        {
            return await GetAsync(address, requestOptions, cancellationToken).ConfigureAwait(false);
        }
        
        public async Task<ApiResponseMessage> CreateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, _serializer.ContentType);
            return await HandleRequestAsync(address, HttpVerbs.POST, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> UpdateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, _serializer.ContentType);
            return await HandleRequestAsync(address, HttpVerbs.PUT, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        
        public async Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StreamApiRequestMessageContent(data);
            return await HandleRequestAsync(address, HttpVerbs.POST, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> PatchAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, _serializer.ContentType);
            return await HandleRequestAsync(address, HttpVerbs.PATCH, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.DELETE, requestOptions, cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        
        public async Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions requestOptions = null, IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.DOWNLOAD, requestOptions, cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        
        private async Task<ApiResponseMessage> HandleRequestAsync(string address, string verb, RequestOptions requestOptions = null, ApiRequestMessageContent content = null, IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            return await SendAsync(CreateRequestMessage(address, verb, requestOptions, content), progress, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<ApiResponseMessage> SendAsync(ApiRequestMessage requestMessage, IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            System.Net.WebClient webClient = null;
            byte[] response = null;
            HttpStatusCode? statusCode = null;
            NameValueCollection responseHeaders = null;
            CancellationTokenRegistration cancellationTokenRegistration = default;
                
            try
            {
                webClient = _webClientFunc();
                cancellationTokenRegistration =
                    cancellationToken.Register(
                        static state => ((System.Net.WebClient)state!).CancelAsync(),
                        webClient
                    );
                
                cancellationToken.ThrowIfCancellationRequested();

                if (progress != null)
                {
                    webClient.DownloadProgressChanged += (_, e) =>
                    {
                        progress.Report(e.ProgressPercentage);
                    };
                }
                
                SetWebClientHeaders(webClient, requestMessage);

                if(IsGetOrDownload(requestMessage.Method))
                {
                    response = await webClient.DownloadDataTaskAsync(requestMessage.RequestUri)
                        .ConfigureAwait(false);
                }
                else
                {
                    byte[] payload;
                    if (requestMessage.Content != null)
                    {
                        webClient.Headers.Add(HttpRequestHeader.ContentType, requestMessage.Content.ContentType);
                        payload = requestMessage.Content.Body;
                    }
                    else
                    {
                        payload = EmptyBytes;
                    }

                    response = await webClient.UploadDataTaskAsync(requestMessage.RequestUri, requestMessage.Method, payload)
                        .ConfigureAwait(false);
                }
                
                responseHeaders = webClient.ResponseHeaders;
                if (webClient is InternalWebClient iwc)
                {
                    statusCode = iwc.StatusCode;
                }
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new RedmineApiException("The operation was canceled by the user.", ex);
                }
            }
            catch (WebException webException)
            {
                webException.HandleWebException(_serializer);
            }
            finally
            {
                #if NETFRAMEWORK
                cancellationTokenRegistration.Dispose();
                #else
                await cancellationTokenRegistration.DisposeAsync().ConfigureAwait(false);
                #endif
                
                webClient?.Dispose();
            }

            return new ApiResponseMessage()
            {
                Headers = responseHeaders,
                Content = response,
                StatusCode = statusCode ?? HttpStatusCode.OK,
            };
        }
    }
}

#endif