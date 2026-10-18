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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
#if !(NET20)
using System.Threading.Tasks;
#endif
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Net.WebClient.MessageContent;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net
{
    internal sealed class RedmineApiClient : IRedmineApiClient
    {
        private readonly IRedmineTransport _transport;
        private readonly IRedmineAuthentication _credentials;
        private readonly IRedmineSerializer _serializer;

        public RedmineApiClient(IRedmineTransport transport, IRedmineAuthentication authentication, IRedmineSerializer serializer)
        {
            _transport = transport;
            _credentials = authentication;
            _serializer = serializer;
        }

        public ApiResponseMessage Get(string address, RequestOptions? requestOptions = null)
        {
            return HandleRequest(address, HttpVerbs.GET, requestOptions);
        }

        public ApiResponseMessage GetPaged(string address, RequestOptions? requestOptions = null)
        {
            return Get(address, requestOptions);
        }

        public ApiResponseMessage Create(string address, string payload, RequestOptions? requestOptions = null)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return HandleRequest(address, HttpVerbs.POST, requestOptions, content);
        }

        public ApiResponseMessage Update(string address, string payload, RequestOptions? requestOptions = null)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return HandleRequest(address, HttpVerbs.PUT, requestOptions, content);
        }

        public ApiResponseMessage Patch(string address, string payload, RequestOptions? requestOptions = null)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return HandleRequest(address, HttpVerbs.PATCH, requestOptions, content);
        }

        public ApiResponseMessage Delete(string address, RequestOptions? requestOptions = null)
        {
            return HandleRequest(address, HttpVerbs.DELETE, requestOptions);
        }

        public ApiResponseMessage Download(string address, RequestOptions? requestOptions = null, IProgress<int>? progress = null)
        {
            return HandleRequest(address, HttpVerbs.DOWNLOAD, requestOptions, progress: progress);
        }

        public ApiResponseMessage Upload(string address, byte[] data, RequestOptions? requestOptions = null)
        {
            var content = new StreamApiRequestMessageContent(data);
            return HandleRequest(address, HttpVerbs.POST, requestOptions, content);
        }

#if !(NET20)
        public async Task<ApiResponseMessage> GetAsync(string address, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.GET, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            return GetAsync(address, requestOptions, cancellationToken);
        }

        public async Task<ApiResponseMessage> CreateAsync(string address, string payload, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return await HandleRequestAsync(address, HttpVerbs.POST, requestOptions, content, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> UpdateAsync(string address, string payload, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return await HandleRequestAsync(address, HttpVerbs.PUT, requestOptions, content, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StreamApiRequestMessageContent(data);
            return await HandleRequestAsync(address, HttpVerbs.POST, requestOptions, content, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> PatchAsync(string address, string payload, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            var content = new StringApiRequestMessageContent(payload, GetContentType(_serializer));
            return await HandleRequestAsync(address, HttpVerbs.PATCH, requestOptions, content, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.DELETE, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions? requestOptions = null, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            return await HandleRequestAsync(address, HttpVerbs.DOWNLOAD, requestOptions, progress: progress, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private Task<ApiResponseMessage> HandleRequestAsync(string address, string verb, RequestOptions? requestOptions = null, ApiRequestMessageContent? content = null, IProgress<int>? progress = null,
            CancellationToken cancellationToken = default)
        {
            return SendAsync(CreateTransportRequest(CreateRequestMessage(address, verb, requestOptions, content), progress), cancellationToken);
        }

        private async Task<ApiResponseMessage> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            RedmineTransportResponse response;
            try
            {
                response = await _transport.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (ex.InnerException is WebException { Status: WebExceptionStatus.RequestCanceled })
            {
                throw RedmineApiExceptionHelper.Create("The operation was canceled by the user.", ex.InnerException, null, GetMethod(request), GetEndpoint(request), null, errorCode: RedmineApiExceptionHelper.UnknownErrorCode);
            }
            catch (WebException webException)
            {
                throw HandleWebException(webException, request);
            }

            return CreateResponseMessage(response, request);
        }
#endif

        private static ApiRequestMessage CreateRequestMessage(string address, string verb, RequestOptions? requestOptions = null, ApiRequestMessageContent? content = null)
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
                req.ContentType = requestOptions.ContentType;
                req.Accept = requestOptions.Accept;
                req.UserAgent = requestOptions.UserAgent;
                req.Headers = requestOptions.Headers;
            }

            if (content != null)
            {
                req.Content = content;
            }

            return req;
        }

        private RedmineTransportRequest CreateTransportRequest(ApiRequestMessage requestMessage, IProgress<int>? progress)
        {
            return new RedmineTransportRequest
            {
                Method = requestMessage.Method,
                Address = requestMessage.RequestUri,
                QueryString = requestMessage.QueryString,
                Headers = CreateHeaders(requestMessage),
                ContentType = requestMessage.Content != null ? requestMessage.ContentType.ValueOrFallback(requestMessage.Content.ContentType) : null,
                Body = requestMessage.Content?.Body,
                Progress = progress,
            };
        }

        private NameValueCollection CreateHeaders(ApiRequestMessage requestMessage)
        {
            var headers = new NameValueCollection();

            if (requestMessage.Headers != null)
            {
                foreach (var header in requestMessage.Headers)
                {
                    headers.Add(header.Key, header.Value ?? string.Empty);
                }
            }

            if (!requestMessage.Accept.IsNullOrWhiteSpace())
            {
                headers.Add("Accept", requestMessage.Accept);
            }

            if (!requestMessage.UserAgent.IsNullOrWhiteSpace())
            {
                headers.Add("User-Agent", requestMessage.UserAgent);
            }

            switch (_credentials)
            {
                case RedmineApiKeyAuthentication:
                    headers.Add(_credentials.AuthenticationType, _credentials.Token ?? string.Empty);
                    break;
                case RedmineBasicAuthentication:
                    headers.Add("Authorization", $"{_credentials.AuthenticationType} {_credentials.Token}");
                    break;
            }

            if (!requestMessage.ImpersonateUser.IsNullOrWhiteSpace())
            {
                headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, requestMessage.ImpersonateUser);
            }

            return headers;
        }

        private ApiResponseMessage HandleRequest(string address, string verb, RequestOptions? requestOptions = null, ApiRequestMessageContent? content = null, IProgress<int>? progress = null)
        {
            return Send(CreateTransportRequest(CreateRequestMessage(address, verb, requestOptions, content), progress));
        }

        private ApiResponseMessage Send(RedmineTransportRequest request)
        {
            RedmineTransportResponse response;
            try
            {
                response = _transport.Send(request);
            }
            catch (WebException webException)
            {
                throw HandleWebException(webException, request);
            }

            return CreateResponseMessage(response, request);
        }

        private ApiResponseMessage CreateResponseMessage(RedmineTransportResponse response, RedmineTransportRequest request)
        {
            if (!response.IsSuccessStatusCode)
            {
                ThrowHttpException(response, request, _serializer);
            }

            return new ApiResponseMessage
            {
                IsSuccessful = true,
                Headers = response.Headers,
                Content = response.Content,
                RawContent = response.Body,
                StatusCode = response.StatusCode,
            };
        }

        private static string GetContentType(IRedmineSerializer serializer)
        {
            return serializer.Format == RedmineConstants.XML ? RedmineConstants.CONTENT_TYPE_APPLICATION_XML : RedmineConstants.CONTENT_TYPE_APPLICATION_JSON;
        }

        private static void ThrowHttpException(RedmineTransportResponse response, RedmineTransportRequest request, IRedmineSerializer serializer)
        {
            var statusCode = response.StatusCode;
            var method = GetMethod(request);
            var endpoint = GetEndpoint(request);
            var responseHeaders = GetResponseHeaders(response.Headers);

            if (statusCode == HttpConstants.StatusCodes.UnprocessableEntity)
            {
                if (response.BodyError != null)
                {
                    throw RedmineApiExceptionHelper.Create(response.BodyError.Message, response.BodyError, statusCode, method, endpoint, responseHeaders, errorCode: RedmineApiExceptionHelper.UnknownErrorCode);
                }

                if (response.Body != null)
                {
                    using var reader = new StreamReader(new MemoryStream(response.Body));
                    throw RedmineApiExceptionHelper.CreateUnprocessableEntityException(reader.ReadToEnd(), null, serializer, method, endpoint, responseHeaders);
                }
            }

            RedmineApiExceptionHelper.MapStatusCodeToException(statusCode, response.Error?.InnerException ?? response.Error, method, endpoint, responseHeaders);
            throw RedmineApiExceptionHelper.Create($"{(HttpStatusCode)statusCode}", response.Error?.InnerException ?? response.Error, statusCode, method, endpoint, responseHeaders);
        }

        private static RedmineApiException HandleWebException(WebException exception, RedmineTransportRequest request)
        {
            var innerException = exception.InnerException ?? exception;
            var method = GetMethod(request);
            var endpoint = GetEndpoint(request);

            if (exception.Status == WebExceptionStatus.Timeout)
            {
                return RedmineApiExceptionHelper.Create(nameof(WebExceptionStatus.Timeout), innerException, null, method, endpoint, null, isTransient: true, errorCode: RedmineApiExceptionHelper.UnknownErrorCode);
            }

            return RedmineApiExceptionHelper.Create(exception.Message, innerException, null, method, endpoint, null, IsTransient(exception.Status), RedmineApiExceptionHelper.UnknownErrorCode);
        }

        private static bool IsTransient(WebExceptionStatus status)
        {
            return status is WebExceptionStatus.ConnectFailure
                or WebExceptionStatus.ConnectionClosed
                or WebExceptionStatus.KeepAliveFailure
                or WebExceptionStatus.ReceiveFailure
                or WebExceptionStatus.SendFailure
                or WebExceptionStatus.PipelineFailure
                or WebExceptionStatus.Timeout;
        }

        private static string GetMethod(RedmineTransportRequest request)
        {
            return request.Method == HttpVerbs.DOWNLOAD ? HttpVerbs.GET : request.Method;
        }

        private static string GetEndpoint(RedmineTransportRequest request)
        {
            var baseAddress = request.BaseAddress;
            var address = request.Address;
            return !baseAddress.IsNullOrWhiteSpace() && Uri.TryCreate(new Uri(baseAddress!), address, out var uri)
                ? uri.ToString()
                : address;
        }

        private static IDictionary<string, string>? GetResponseHeaders(NameValueCollection? responseHeaders)
        {
            if (responseHeaders == null)
            {
                return null;
            }

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in responseHeaders.AllKeys)
            {
                headers[key] = responseHeaders[key];
            }

            return headers;
        }
    }
}
