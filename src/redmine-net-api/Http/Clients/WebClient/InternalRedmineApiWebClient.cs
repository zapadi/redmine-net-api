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
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Http.Helpers;
using Redmine.Net.Api.Http.Messages;
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
            var requestMessage = CreateRequestMessage(address, verb, Serializer, requestOptions, content as RedmineApiRequestContent);
            
            var responseMessage = Send(requestMessage, progress);
            
            return responseMessage;
        }

        private static RedmineApiRequest CreateRequestMessage(string address, string verb, IRedmineSerializer serializer, RequestOptions requestOptions = null, RedmineApiRequestContent content = null)
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
                if (!requestOptions.Accept.IsNullOrWhiteSpace())
                {
                    req.Accept = requestOptions.Accept;
                }

                if (requestOptions.Headers != null)
                {
                    req.Headers = requestOptions.Headers;
                }

                if (!requestOptions.UserAgent.IsNullOrWhiteSpace())
                {
                    req.UserAgent = requestOptions.UserAgent;
                }
            }

            if (content != null)
            {
                req.Content = content;
                req.ContentType = content.ContentType;
            }
            else
            {
                req.ContentType = serializer.ContentType;
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

                if (requestMessage.QueryString != null)
                {
                    webClient.QueryString = requestMessage.QueryString;
                }

                webClient.ApplyHeaders(requestMessage, Credentials);

                if (IsGetOrDownload(requestMessage.Method))
                {
                    response = requestMessage.Method == HttpConstants.HttpVerbs.DOWNLOAD
                        ? webClient.DownloadWithProgress(requestMessage.RequestUri, progress)
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
                statusCode = webClient.GetStatusCode();
            }
            catch (WebException webException) when (webException.Status == WebExceptionStatus.ProtocolError)
            {
                HandleResponseException(webException, requestMessage.RequestUri, Serializer);
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw new RedmineOperationCanceledException(webException.Message, requestMessage.RequestUri, webException.InnerException);
                }

                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    throw new RedmineTimeoutException(webException.Message, requestMessage.RequestUri, webException.InnerException);
                }
                
                var errStatusCode = GetExceptionStatusCode(webException);
                throw new RedmineApiException(webException.Message, requestMessage.RequestUri, errStatusCode, webException.InnerException);
            }
            catch (Exception ex)
            {
                throw new RedmineApiException(ex.Message, requestMessage.RequestUri, HttpConstants.StatusCodes.Unknown, ex.InnerException);
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
       
        private static void HandleResponseException(WebException exception, string url, IRedmineSerializer serializer)
        {
            var innerException = exception.InnerException ?? exception;

            if (exception.Response == null)
            {
                throw new RedmineApiException(exception.Message, url, null, innerException);                
            }
            
            var statusCode = GetExceptionStatusCode(exception);
        
            using var responseStream = exception.Response.GetResponseStream();
            throw statusCode switch
            {
                HttpConstants.StatusCodes.NotFound => new RedmineNotFoundException(exception.Message, url, innerException),
                HttpConstants.StatusCodes.Unauthorized => new RedmineUnauthorizedException(exception.Message, url, innerException),
                HttpConstants.StatusCodes.Forbidden => new RedmineForbiddenException(exception.Message, url, innerException),
                HttpConstants.StatusCodes.UnprocessableEntity => RedmineExceptionHelper.CreateUnprocessableEntityException(url, responseStream, innerException, serializer),
                HttpConstants.StatusCodes.NotAcceptable => new RedmineNotAcceptableException(exception.Message, innerException),
                _ => new RedmineApiException(exception.Message, url, statusCode, innerException),
            };
        }

        private static int? GetExceptionStatusCode(WebException webException)
        {
            var statusCode = webException.Response is HttpWebResponse httpResponse 
                ? (int)httpResponse.StatusCode 
                : HttpConstants.StatusCodes.Unknown;
            return statusCode;
        }
    }
}
