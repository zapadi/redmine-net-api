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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
#if!(NET20)
using System.Threading.Tasks;
#endif
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Net.WebClient.MessageContent;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net.WebClient
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class InternalRedmineApiWebClient : IRedmineApiClient
    {
        private static readonly byte[] EmptyBytes = Encoding.UTF8.GetBytes(string.Empty);
        private readonly Func<System.Net.WebClient> _webClientFunc;
        private readonly IRedmineAuthentication _credentials;
        private readonly IRedmineSerializer _serializer;

        public InternalRedmineApiWebClient(RedmineManagerOptions redmineManagerOptions)
            : this(() => new InternalWebClient(redmineManagerOptions), redmineManagerOptions.Authentication, redmineManagerOptions.Serializer)
        {
            ConfigureServicePointManager(redmineManagerOptions.WebClientOptions);
        }

        public InternalRedmineApiWebClient(
            Func<System.Net.WebClient> webClientFunc, 
            IRedmineAuthentication authentication, 
            IRedmineSerializer serializer)
        {
            _webClientFunc = webClientFunc;
            _credentials = authentication;
            _serializer = serializer;
        }

        
        private static void ConfigureServicePointManager(IRedmineWebClientOptions webClientOptions)
        {
            if (webClientOptions == null)
            {
                return;
            }
#pragma warning disable SYSLIB0014           
            if (webClientOptions.MaxServicePoints.HasValue)
            {
                ServicePointManager.MaxServicePoints = webClientOptions.MaxServicePoints.Value;
            }

            if (webClientOptions.MaxServicePointIdleTime.HasValue)
            {
                ServicePointManager.MaxServicePointIdleTime = webClientOptions.MaxServicePointIdleTime.Value;
            }

            ServicePointManager.SecurityProtocol = webClientOptions.SecurityProtocolType ?? ServicePointManager.SecurityProtocol;

            if (webClientOptions.DefaultConnectionLimit.HasValue)
            {
                ServicePointManager.DefaultConnectionLimit = webClientOptions.DefaultConnectionLimit.Value;
            }

            if (webClientOptions.DnsRefreshTimeout.HasValue)
            {
                ServicePointManager.DnsRefreshTimeout = webClientOptions.DnsRefreshTimeout.Value;
            }

            ServicePointManager.CheckCertificateRevocationList = webClientOptions.CheckCertificateRevocationList;

            if (webClientOptions.EnableDnsRoundRobin.HasValue)
            {
                ServicePointManager.EnableDnsRoundRobin = webClientOptions.EnableDnsRoundRobin.Value;
            }

            #if(NET46_OR_GREATER || NETCOREAPP)
            if (webClientOptions.ReusePort.HasValue)
            {
                ServicePointManager.ReusePort = webClientOptions.ReusePort.Value;
            }
            #endif
#pragma warning restore SYSLIB0014
        }

        public ApiResponseMessage Get(string address, RequestOptions requestOptions = null)
        {
            return HandleRequest(address, HttpVerbs.GET, requestOptions);
        }

        public ApiResponseMessage GetPaged(string address, RequestOptions requestOptions = null)
        {
            return Get(address, requestOptions);
        }

        public ApiResponseMessage Create(string address, string payload, RequestOptions requestOptions = null)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return HandleRequest(address, HttpVerbs.POST, requestOptions, content);
        }

        public ApiResponseMessage Update(string address, string payload, RequestOptions requestOptions = null)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return HandleRequest(address, HttpVerbs.PUT, requestOptions, content);
        }

        public ApiResponseMessage Patch(string address, string payload, RequestOptions requestOptions = null)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return HandleRequest(address, HttpVerbs.PATCH, requestOptions, content);
        }

        public ApiResponseMessage Delete(string address, RequestOptions requestOptions = null)
        {
            return HandleRequest(address, HttpVerbs.DELETE, requestOptions);
        }

        public ApiResponseMessage Download(string address, RequestOptions requestOptions = null)
        {
            return HandleRequest(address, HttpVerbs.DOWNLOAD, requestOptions);
        }

        public ApiResponseMessage Upload(string address, byte[] data, RequestOptions requestOptions = null)
        {
            var content = new StreamApiRequestMessageContent(data);
            return HandleRequest(address, HttpVerbs.POST, requestOptions, content);
        }

        #if !(NET20)
        public async Task<ApiResponseMessage> GetAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.GET, requestOptions, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default) 
        {
            return GetAsync(address, requestOptions, cancellationToken);
        }
        
        public async Task<ApiResponseMessage> CreateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return await HandleRequestAsync(address, HttpVerbs.POST, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> UpdateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return await HandleRequestAsync(address, HttpVerbs.PUT, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        
        public async Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StreamApiRequestMessageContent(data);
            return await HandleRequestAsync(address, HttpVerbs.POST, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> PatchAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return await HandleRequestAsync(address, HttpVerbs.PATCH, requestOptions, content, cancellationToken:cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.DELETE, requestOptions, cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        
        public async Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.DOWNLOAD, requestOptions, cancellationToken:cancellationToken).ConfigureAwait(false);
        }
        
        private Task<ApiResponseMessage> HandleRequestAsync(string address, string verb, RequestOptions requestOptions = null, ApiRequestMessageContent content = null, CancellationToken cancellationToken = default)
        {
            return SendAsync(CreateRequestMessage(address, verb, requestOptions, content), cancellationToken);
        }

        private async Task<ApiResponseMessage> SendAsync(ApiRequestMessage requestMessage, CancellationToken cancellationToken)
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
                    throw new RedmineApiException("The operation was canceled by the user.", ex);
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
                IsSuccessful = true,
                Headers = responseHeaders,
                Content = response,
                RawContent = responseAsBytes,
                StatusCode = (int)(statusCode ?? HttpStatusCode.OK),
            };
        }
        #endif


        private static ApiRequestMessage CreateRequestMessage(string address, string verb, RequestOptions requestOptions = null, ApiRequestMessageContent content = null)
        {
            var req = new ApiRequestMessage()
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

        private ApiResponseMessage HandleRequest(string address, string verb, RequestOptions requestOptions = null, ApiRequestMessageContent content = null)
        {
            return Send(CreateRequestMessage(address, verb, requestOptions, content));
        }

        private ApiResponseMessage Send(ApiRequestMessage requestMessage)
        {
            System.Net.WebClient? webClient = null;
            string? response = null;
            byte[]? responseAsBytes = null;
            HttpStatusCode? statusCode = null;
            NameValueCollection? responseHeaders = null;

            try
            {
                webClient = _webClientFunc();

                SetWebClientHeaders(webClient, requestMessage);
                if (requestMessage.QueryString != null)
                {
                    webClient.QueryString = requestMessage.QueryString;
                }

                if (IsGetOrDownload(requestMessage.Method))
                {
                    if (requestMessage.Method == HttpConstants.HttpVerbs.GET)
                    {
                        response = webClient.DownloadString((string)requestMessage.RequestUri);
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

                    responseAsBytes = webClient.UploadData((string)requestMessage.RequestUri, requestMessage.Method, payload);
                }

                responseHeaders = webClient.ResponseHeaders;
                if (webClient is InternalWebClient iwc)
                {
                    statusCode = iwc.StatusCode;
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
                webClient?.Dispose();
            }

            return new ApiResponseMessage
            {
                IsSuccessful = true,
                Headers = responseHeaders,
                Content = response,
                RawContent = responseAsBytes,
                StatusCode = (int)(statusCode ?? HttpStatusCode.OK),
            };
        }

        private void SetWebClientHeaders(System.Net.WebClient webClient, ApiRequestMessage requestMessage)
        {
            if (requestMessage.QueryString != null)
            {
                webClient.QueryString = requestMessage.QueryString;
            }

            switch (_credentials)
            {
                case RedmineApiKeyAuthentication:
                    webClient.Headers.Add(_credentials.AuthenticationType,_credentials.Token);
                    break;
                case RedmineBasicAuthentication:
                    webClient.Headers.Add("Authorization", $"{_credentials.AuthenticationType} {_credentials.Token}");
                    break;
            }

            if (!requestMessage.ImpersonateUser.IsNullOrWhiteSpace())
            {
                webClient.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, requestMessage.ImpersonateUser);
            }
        }

        private static bool IsGetOrDownload(string method)
        {
            return method is HttpVerbs.GET or HttpVerbs.DOWNLOAD;
        }

        private static string GetContentType(IRedmineSerializer serializer)
        {
            return serializer.Format == RedmineConstants.XML ? RedmineConstants.CONTENT_TYPE_APPLICATION_XML : RedmineConstants.CONTENT_TYPE_APPLICATION_JSON;
        }
        
        /// <summary>
    /// Handles the web exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="RedmineException"> </exception>
    private static KeyValuePair<HttpStatusCode, string> HandleWebException(WebException exception)
    {
        var innerException = exception.InnerException ?? exception;

        switch (exception.Status)
        {
            case WebExceptionStatus.Timeout:
                throw new RedmineApiException(nameof(WebExceptionStatus.Timeout), innerException);
            case WebExceptionStatus.ProtocolError:
                if (exception.Response != null)
                {
                    var statusCode = exception.Response is HttpWebResponse httpResponse
                        ? (int)httpResponse.StatusCode
                        : (int)HttpStatusCode.InternalServerError;

                    using var responseStream = exception.Response.GetResponseStream();
                    if (statusCode == HttpConstants.StatusCodes.UnprocessableEntity)
                    {
                        try
                        {
                            if (responseStream != null)
                            {
                                using var reader = new StreamReader(responseStream);
                                var content = reader.ReadToEnd();
                                return new KeyValuePair<HttpStatusCode, string>((HttpStatusCode)HttpConstants.StatusCodes.UnprocessableEntity, content);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new RedmineApiException(ex.Message, ex);
                        }
                    }

                    RedmineApiExceptionHelper.MapStatusCodeToException(statusCode, innerException);
                }

                break;
        }

        throw new RedmineApiException(exception.Message, innerException);
    }
    }
}
