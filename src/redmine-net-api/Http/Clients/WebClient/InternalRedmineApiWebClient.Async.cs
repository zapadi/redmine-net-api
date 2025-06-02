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
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Http.Messages;

namespace Redmine.Net.Api.Http.Clients.WebClient
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed partial class InternalRedmineApiWebClient
    {
        protected override async Task<RedmineApiResponse> HandleRequestAsync(string address, string verb, RequestOptions requestOptions = null, object content = null,
            IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            LogRequest(verb, address, requestOptions);
            
            var response = await SendAsync(CreateRequestMessage(address, verb, Serializer, requestOptions, content as RedmineApiRequestContent), progress, cancellationToken: cancellationToken).ConfigureAwait(false);
            
            LogResponse((int)response.StatusCode);

            return response;
        }
        
        private async Task<RedmineApiResponse> SendAsync(RedmineApiRequest requestMessage, IProgress<int> progress = null, CancellationToken cancellationToken = default)
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
                        static state => ((System.Net.WebClient)state).CancelAsync(),
                        webClient
                    );

                cancellationToken.ThrowIfCancellationRequested();

                if (progress != null)
                {
                    webClient.DownloadProgressChanged += (_, e) => { progress.Report(e.ProgressPercentage); };
                }

                if (requestMessage.QueryString != null)
                {
                    webClient.QueryString = requestMessage.QueryString;
                }
                
                webClient.ApplyHeaders(requestMessage, Credentials);

                if (IsGetOrDownload(requestMessage.Method))
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

                    response = await webClient
                        .UploadDataTaskAsync(requestMessage.RequestUri, requestMessage.Method, payload)
                        .ConfigureAwait(false);
                }

                responseHeaders = webClient.ResponseHeaders;
                statusCode = webClient.GetStatusCode();
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new RedmineOperationCanceledException(ex.Message, requestMessage.RequestUri, ex);
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                throw new RedmineTimeoutException(ex.Message, requestMessage.RequestUri, ex);
            }
            catch (WebException webException)when (webException.Status == WebExceptionStatus.ProtocolError)
            {
                HandleResponseException(webException, requestMessage.RequestUri, Serializer);
            }
            catch (OperationCanceledException ex)
            {
                throw new RedmineOperationCanceledException(ex.Message, requestMessage.RequestUri, ex);
            }
            catch (Exception ex)
            {
                throw new RedmineApiException(ex.Message, requestMessage.RequestUri, null, ex);
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

            return new RedmineApiResponse()
            {
                Headers = responseHeaders,
                Content = response,
                StatusCode = statusCode ?? HttpStatusCode.OK,
            };
        }
    }
}

#endif