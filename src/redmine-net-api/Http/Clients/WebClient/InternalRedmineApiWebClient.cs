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

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Http.Extensions;
using Redmine.Net.Api.Http.Helpers;
using Redmine.Net.Api.Http.Messages;
using Redmine.Net.Api.Logging;
using Redmine.Net.Api.Options;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Http.Clients.WebClient
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed partial class InternalRedmineApiWebClient : RedmineApiClient
    {
        private static readonly byte[] EmptyBytes = Encoding.UTF8.GetBytes(string.Empty);
        private readonly Func<System.Net.WebClient> _webClientFunc;

        public InternalRedmineApiWebClient(RedmineManagerOptions redmineManagerOptions)
            : this(() => new InternalWebClient(redmineManagerOptions), redmineManagerOptions)
        {
        }

        public InternalRedmineApiWebClient(Func<System.Net.WebClient> webClientFunc, RedmineManagerOptions redmineManagerOptions)
            : base(redmineManagerOptions)
        {
            _webClientFunc = webClientFunc;
        }

        protected override object CreateContentFromPayload(string payload)
        {
            return RedmineApiRequestContent.CreateString(payload, Serializer.ContentType);
        }

        protected override object CreateContentFromBytes(byte[] data)
        {
            return RedmineApiRequestContent.CreateBinary(data);
        }
        
        protected override RedmineApiResponse HandleRequest(string address, string verb, RequestOptions requestOptions = null, object content = null, IProgress<int> progress = null)
        {
            var requestMessage = CreateRequestMessage(address, verb, requestOptions, content as RedmineApiRequestContent);
            
            if (Options.LoggingOptions?.IncludeHttpDetails == true)
            {
                Options.Logger.Debug($"Request HTTP {verb} {address}");
                
                if (requestOptions?.QueryString != null)
                {
                    Options.Logger.Debug($"Query parameters: {requestOptions.QueryString.ToQueryString()}");
                }
            }

            var responseMessage = Send(requestMessage, progress);
            
            if (Options.LoggingOptions?.IncludeHttpDetails == true)
            {
                Options.Logger.Debug($"Response status: {responseMessage.StatusCode}");
            }
            
            return responseMessage;
        }

        private static RedmineApiRequest CreateRequestMessage(string address, string verb, RequestOptions requestOptions = null, RedmineApiRequestContent content = null)
        {
            var req = new RedmineApiRequest()
            {
                RequestUri = address,
                Method = verb,
            };

            if (requestOptions != null)
            {
                req.QueryString = requestOptions.QueryString;
                req.ImpersonateUser = requestOptions.ImpersonateUser;
            }

            if (content != null)
            {
                req.Content = content;
            }

            return req;
        }

        private RedmineApiResponse Send(RedmineApiRequest requestMessage, IProgress<int> progress = null)
        {
            System.Net.WebClient webClient = null;
            byte[] response = null;
            HttpStatusCode? statusCode = null;
            NameValueCollection responseHeaders = null;

            try
            {
                webClient = _webClientFunc();
                
                SetWebClientHeaders(webClient, requestMessage);

                if (IsGetOrDownload(requestMessage.Method))
                {
                    response = requestMessage.Method == HttpConstants.HttpVerbs.DOWNLOAD 
                        ? DownloadWithProgress(requestMessage.RequestUri, webClient, progress) 
                        : webClient.DownloadData(requestMessage.RequestUri);
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

                    response = webClient.UploadData(requestMessage.RequestUri, requestMessage.Method, payload);
                }

                responseHeaders = webClient.ResponseHeaders;
                if (webClient is InternalWebClient iwc)
                {
                    statusCode = iwc.StatusCode;
                }
            }
            catch (WebException webException)
            {
                HandleWebException(webException, Serializer);
            }
            finally
            {
                webClient?.Dispose();
            }

            return new RedmineApiResponse()
            {
                Headers = responseHeaders,
                Content = response,
                StatusCode = statusCode ?? HttpStatusCode.OK,
            };
        }

        private void SetWebClientHeaders(System.Net.WebClient webClient, RedmineApiRequest requestMessage)
        {
            if (requestMessage.QueryString != null)
            {
                webClient.QueryString = requestMessage.QueryString;
            }

            switch (Credentials)
            {
                case RedmineApiKeyAuthentication:
                    webClient.Headers.Add(RedmineConstants.API_KEY_AUTHORIZATION_HEADER_KEY,Credentials.Token);
                    break;
                case RedmineBasicAuthentication:
                    webClient.Headers.Add(RedmineConstants.AUTHORIZATION_HEADER_KEY, Credentials.Token);
                    break;
            }

            if (!requestMessage.ImpersonateUser.IsNullOrWhiteSpace())
            {
                webClient.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, requestMessage.ImpersonateUser);
            }
        }

        private static byte[] DownloadWithProgress(string url, System.Net.WebClient webClient, IProgress<int> progress)
        {
            var contentLength = GetContentLength(webClient);
            byte[] data;
            if (contentLength > 0)
            {
                using (var respStream = webClient.OpenRead(url))
                {
                    data = new byte[contentLength];
                    var buffer = new byte[4096];
                    int bytesRead;
                    var totalBytesRead = 0;

                    while ((bytesRead = respStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Buffer.BlockCopy(buffer, 0, data, totalBytesRead, bytesRead);
                        totalBytesRead += bytesRead;
                                
                        ReportProgress(progress, contentLength, totalBytesRead);
                    }
                }
            }
            else
            {
                data = webClient.DownloadData(url);
                progress?.Report(100);    
            }
            
            return data;
        }

        private static int GetContentLength(System.Net.WebClient webClient)
        {
            var total = -1;
            if (webClient.ResponseHeaders == null)
            {
                return total;
            }
            
            var contentLengthAsString = webClient.ResponseHeaders[HttpRequestHeader.ContentLength];
            total = Convert.ToInt32(contentLengthAsString, CultureInfo.InvariantCulture);

            return total;
        }

        /// <summary>
        /// Handles the web exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="serializer"></param>
        /// <exception cref="RedmineTimeoutException">Timeout!</exception>
        /// <exception cref="NameResolutionFailureException">Bad domain name!</exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException">The page that you are trying to update is staled!</exception>
        /// <exception cref="RedmineException"> </exception>
        /// <exception cref="NotAcceptableException"></exception>
        public static void HandleWebException(WebException exception, IRedmineSerializer serializer)
        {
            if (exception == null)
            {
                return;
            }

            var innerException = exception.InnerException ?? exception;

            switch (exception.Status)
            {
                case WebExceptionStatus.Timeout:
                    throw new RedmineTimeoutException(nameof(WebExceptionStatus.Timeout), innerException);
                case WebExceptionStatus.NameResolutionFailure:
                    throw new NameResolutionFailureException("Bad domain name.", innerException);
                case WebExceptionStatus.ProtocolError:
                    if (exception.Response != null)
                    {
                        var statusCode = exception.Response is HttpWebResponse httpResponse 
                            ? (int)httpResponse.StatusCode 
                            : (int)HttpStatusCode.InternalServerError;
                
                        using var responseStream = exception.Response.GetResponseStream();
                        RedmineExceptionHelper.MapStatusCodeToException(statusCode, responseStream, innerException, serializer);
                    }

                    break;
            }
            throw new RedmineException(exception.Message, innerException);
        }
    }
}
