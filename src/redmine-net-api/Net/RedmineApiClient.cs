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
using System.Text;
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
        private const string CanceledByUserMessage = "The operation was canceled by the user.";

        private readonly IRedmineTransport _transport;
        private readonly IRedmineAuthentication _credentials;
        private readonly IRedmineSerializer _serializer;
        private readonly bool _encodeQueryString;
        private readonly Uri? _baseAddress;
        private readonly bool _isCustomTransport;

        public RedmineApiClient(IRedmineTransport transport, IRedmineAuthentication authentication, IRedmineSerializer serializer, bool encodeQueryString = false, Uri? baseAddress = null, bool isCustomTransport = false)
        {
            _transport = transport;
            _credentials = authentication;
            _serializer = serializer;
            _encodeQueryString = encodeQueryString;
            _baseAddress = baseAddress;
            _isCustomTransport = isCustomTransport;
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
            return HandleRequest(address, HttpVerbs.GET, requestOptions, isDownload: true, progress: progress);
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
            return await HandleRequestAsync(address, HttpVerbs.GET, requestOptions, isDownload: true, progress: progress, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private Task<ApiResponseMessage> HandleRequestAsync(string address, string verb, RequestOptions? requestOptions = null, ApiRequestMessageContent? content = null, bool isDownload = false, IProgress<int>? progress = null,
            CancellationToken cancellationToken = default)
        {
            return SendAsync(CreateTransportRequest(CreateRequestMessage(address, verb, requestOptions, content), isDownload, progress), cancellationToken);
        }

        private async Task<ApiResponseMessage> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            if (_isCustomTransport)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            RedmineTransportResponse response;
            try
            {
                response = await _transport.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (ex.InnerException is WebException { Status: WebExceptionStatus.RequestCanceled })
            {
                throw RedmineApiExceptionHelper.Create(CanceledByUserMessage, ex.InnerException, null, GetMethod(request), GetEndpoint(request), null, errorCode: RedmineApiExceptionHelper.UnknownErrorCode);
            }
            catch (WebException webException)
            {
                throw HandleWebException(webException, request);
            }
            catch (Exception exception) when (_isCustomTransport)
            {
                throw HandleCustomTransportException(exception, request, cancellationToken.IsCancellationRequested);
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

        private RedmineTransportRequest CreateTransportRequest(ApiRequestMessage requestMessage, bool isDownload, IProgress<int>? progress)
        {
            var address = requestMessage.RequestUri;
            var queryString = requestMessage.QueryString;
            if (queryString is { Count: > 0 } && requestMessage.RequestUri is { } requestUri && (_encodeQueryString || HasQuery(requestUri)))
            {
                address = AppendQueryString(requestUri, queryString);
                queryString = null;
            }

            return new RedmineTransportRequest(requestMessage.Method, address, CreateUri(address, queryString))
            {
                QueryString = queryString,
                Headers = CreateHeaders(requestMessage),
                ContentType = requestMessage.Content != null ? requestMessage.ContentType.ValueOrFallback(requestMessage.Content.ContentType) : null,
                Body = requestMessage.Content?.Body,
                IsDownload = isDownload,
                Progress = progress,
                BaseAddress = _isCustomTransport ? _baseAddress?.ToString() : null,
            };
        }

        private Uri CreateUri(string? address, NameValueCollection? queryString)
        {
            var value = address == null ? string.Empty : queryString is { Count: > 0 } ? AppendQueryString(address, queryString) : address;
            if (_baseAddress != null && Uri.TryCreate(_baseAddress, value, out var uri))
            {
                return uri;
            }

            return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri) ? uri : new Uri(string.Empty, UriKind.Relative);
        }

        private string AppendQueryString(string address, NameValueCollection queryString)
        {
            var builder = new StringBuilder(address);
            var queryStart = address.IndexOf('?');
            var delimiter = queryStart >= 0 ? '&' : '?';
            var addressQuery = queryStart >= 0 ? address.Substring(queryStart + 1).Split('&') : null;
            for (var i = 0; i < queryString.Count; i++)
            {
                var key = EncodeQueryValue(queryString.GetKey(i));
                if (addressQuery != null && key != null && HasQueryKey(addressQuery, key))
                {
                    continue;
                }

                builder.Append(delimiter).Append(key).Append('=').Append(EncodeQueryValue(queryString.Get(i)));
                delimiter = '&';
            }

            return builder.ToString();
        }

        private static bool HasQueryKey(string[] query, string key)
        {
            foreach (var pair in query)
            {
                var separator = pair.IndexOf('=');
                if (string.Equals(separator >= 0 ? pair.Substring(0, separator) : pair, key, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasQuery(string address)
        {
#if NETCOREAPP
            return address.Contains('?', StringComparison.Ordinal);
#else
            return address.IndexOf('?') >= 0;
#endif
        }

        private string? EncodeQueryValue(string? value)
        {
            return _encodeQueryString && value is { Length: > 0 } ? Uri.EscapeDataString(value) : value;
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

        private ApiResponseMessage HandleRequest(string address, string verb, RequestOptions? requestOptions = null, ApiRequestMessageContent? content = null, bool isDownload = false, IProgress<int>? progress = null)
        {
            return Send(CreateTransportRequest(CreateRequestMessage(address, verb, requestOptions, content), isDownload, progress));
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
            catch (Exception exception) when (_isCustomTransport)
            {
                throw HandleCustomTransportException(exception, request, false);
            }

            return CreateResponseMessage(response, request);
        }

        private ApiResponseMessage CreateResponseMessage(RedmineTransportResponse? response, RedmineTransportRequest request)
        {
            if (response == null)
            {
                throw RedmineApiExceptionHelper.Create("The transport returned no response.", null, null, GetMethod(request), GetEndpoint(request), null, errorCode: RedmineApiExceptionHelper.UnknownErrorCode);
            }

            if (!response.IsSuccessStatusCode)
            {
                ThrowHttpException(response, request, _serializer, _isCustomTransport);
            }

            return new ApiResponseMessage
            {
                IsSuccessful = true,
                Headers = response.Headers,
                Content = response.Content ?? (_isCustomTransport && !request.IsDownload ? DecodeBody(response.Body, response.Headers) : null),
                RawContent = response.Body,
                StatusCode = response.StatusCode,
            };
        }

        private static string? DecodeBody(byte[]? body, NameValueCollection? headers)
        {
            if (body == null)
            {
                return null;
            }

            if (body.Length >= 3 && body[0] == 0xEF && body[1] == 0xBB && body[2] == 0xBF)
            {
                return Encoding.UTF8.GetString(body, 3, body.Length - 3);
            }

            return GetEncoding(headers?["Content-Type"]).GetString(body);
        }

        private static Encoding GetEncoding(string? contentType)
        {
            if (contentType == null)
            {
                return Encoding.UTF8;
            }

            foreach (var parameter in contentType.Split(';'))
            {
                var separator = parameter.IndexOf('=');
                if (separator < 0 || !string.Equals(parameter.Substring(0, separator).Trim(), "charset", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    return Encoding.GetEncoding(parameter.Substring(separator + 1).Trim().Trim('"'));
                }
                catch (ArgumentException)
                {
                    return Encoding.UTF8;
                }
            }

            return Encoding.UTF8;
        }

        private static string GetContentType(IRedmineSerializer serializer)
        {
            return serializer.Format == RedmineConstants.XML ? RedmineConstants.CONTENT_TYPE_APPLICATION_XML : RedmineConstants.CONTENT_TYPE_APPLICATION_JSON;
        }

        private static void ThrowHttpException(RedmineTransportResponse response, RedmineTransportRequest request, IRedmineSerializer serializer, bool isCustomTransport)
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
                    if (isCustomTransport)
                    {
                        throw RedmineApiExceptionHelper.CreateUnprocessableEntityException(DecodeBody(response.Body, response.Headers), null, serializer, method, endpoint, responseHeaders);
                    }

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

        private static RedmineApiException HandleCustomTransportException(Exception exception, RedmineTransportRequest request, bool isCanceledByCaller)
        {
            var method = GetMethod(request);
            var endpoint = GetEndpoint(request);

            return exception switch
            {
                RedmineApiException apiException => new RedmineApiException(apiException.Message, apiException.InnerException, apiException.ErrorCode, apiException.IsTransient, apiException.CorrelationId, method, endpoint, apiException.HttpStatusCode, apiException.ResponseHeaders),
                OperationCanceledException when isCanceledByCaller => RedmineApiExceptionHelper.Create(CanceledByUserMessage, exception, null, method, endpoint, null, errorCode: RedmineApiExceptionHelper.UnknownErrorCode),
                TimeoutException or OperationCanceledException { InnerException: TimeoutException } => RedmineApiExceptionHelper.Create(nameof(WebExceptionStatus.Timeout), exception, null, method, endpoint, null, isTransient: true, errorCode: RedmineApiExceptionHelper.UnknownErrorCode),
                _ => RedmineApiExceptionHelper.Create(exception.Message, exception, null, method, endpoint, null, errorCode: RedmineApiExceptionHelper.UnknownErrorCode),
            };
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
            return request.Method;
        }

        private static string GetEndpoint(RedmineTransportRequest request)
        {
            var baseAddress = request.BaseAddress;
            var address = request.Address;
            return baseAddress != null && !baseAddress.IsNullOrWhiteSpace() && Uri.TryCreate(new Uri(baseAddress), address, out var uri)
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
                if (key == null)
                {
                    continue;
                }

                headers[key] = responseHeaders[key] ?? string.Empty;
            }

            return headers;
        }
    }
}
