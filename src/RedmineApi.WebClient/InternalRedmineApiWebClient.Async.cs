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
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.WebClient
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed partial class InternalRedmineApiWebClient
    {
        protected override async Task<ApiResponseMessage> HandleRequestAsync(string address, string verb, RequestOptions? requestOptions = null, object? content = null,
            IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            return await SendAsync(CreateRequestMessage(address, verb, requestOptions, content as ApiRequestContent), progress, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<ApiResponseMessage> SendAsync(ApiRequestMessage requestMessage, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            System.Net.WebClient? webClient = null;
            string? response = null;
            byte[]? responseAsBytes = null;
            HttpStatusCode? statusCode = null;
            NameValueCollection? responseHeaders = null;
            CancellationTokenRegistration cancellationTokenRegistration = default;

            try
            {
                webClient = _webClientFunc();
                cancellationTokenRegistration =
                    cancellationToken.Register(
                        static state => (state as System.Net.WebClient)?.CancelAsync(),
                        webClient
                    );

                cancellationToken.ThrowIfCancellationRequested();

                if (progress != null)
                {
                    webClient.DownloadProgressChanged += (_, e) => { progress.Report(e.ProgressPercentage); };
                }

                SetWebClientHeaders(webClient, requestMessage);
                if (requestMessage.QueryString != null)
                {
                    webClient.QueryString = requestMessage.QueryString;
                }

                if (IsGetOrDownload(requestMessage.Method))
                {
                    if (requestMessage.Method == HttpConstants.HttpVerbs.DOWNLOAD)
                    {
                        responseAsBytes = await webClient.DownloadDataTaskAsync((string)requestMessage.RequestUri)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        response = await webClient.DownloadStringTaskAsync((string)requestMessage.RequestUri)
                            .ConfigureAwait(false);
                    }
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

                    responseAsBytes = await webClient.UploadDataTaskAsync((string)requestMessage.RequestUri, requestMessage.Method, payload)
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
                    throw new RedmineApiException(RedmineApiErrorCode.RequestCanceled,"The operation was canceled by the user.", ex);
                }
            }
            catch (WebException webException)
            {
                var content = HandleWebException(webException);
                return new ApiResponseMessage
                {
                    Headers = responseHeaders,
                    Content = content.Value,
                    StatusCode = (int)content.Key,
                };
            }
            finally
            {
#if NET
                await cancellationTokenRegistration.DisposeAsync().ConfigureAwait(false);
#else
                cancellationTokenRegistration.Dispose();
#endif

                webClient?.Dispose();
            }

            return new ApiResponseMessage()
            {
                Headers = responseHeaders,
                Content = response,
                RawContent = responseAsBytes,
                StatusCode = (int)(statusCode ?? HttpStatusCode.OK),
            };
        }
    }
}

#endif