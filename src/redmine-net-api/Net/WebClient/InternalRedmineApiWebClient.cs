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
        private readonly TimeSpan? _timeout;

        public InternalRedmineApiWebClient(RedmineManagerOptions redmineManagerOptions)
            : this(CreateWebClientFunc(redmineManagerOptions), redmineManagerOptions.Authentication, redmineManagerOptions.Serializer)
        {
            _timeout = redmineManagerOptions.WebClientOptions?.Timeout;
            ConfigureServicePointManager(redmineManagerOptions.WebClientOptions);
        }

        private static Func<System.Net.WebClient> CreateWebClientFunc(RedmineManagerOptions redmineManagerOptions)
        {
            if (redmineManagerOptions.ClientFunc == null)
            {
                return () => new InternalWebClient(redmineManagerOptions);
            }

            return () =>
            {
                var webClient = redmineManagerOptions.ClientFunc();
                if (webClient == null)
                {
                    throw new RedmineException("The web client function configured with WithWebClient returned null.");
                }

                if (webClient.BaseAddress.IsNullOrWhiteSpace())
                {
                    webClient.BaseAddress = redmineManagerOptions.BaseAddress.ToString();
                }

                return webClient;
            };
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


        private static void ConfigureServicePointManager(IRedmineWebClientOptions? webClientOptions)
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

            if (webClientOptions.CheckCertificateRevocationList)
            {
                ServicePointManager.CheckCertificateRevocationList = true;
            }

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
            return SendAsync(CreateRequestMessage(address, verb, requestOptions, content), progress: progress, cancellationToken: cancellationToken);
        }

        private async Task<ApiResponseMessage> SendAsync(ApiRequestMessage requestMessage, CancellationToken cancellationToken, IProgress<int>? progress = null)
        {
            System.Net.WebClient? webClient = null;
            string? response = null;
            byte[]? responseAsBytes = null;
            HttpStatusCode? statusCode = null;
            NameValueCollection? responseHeaders = null;
            CancellationTokenRegistration cancellationTokenRegistration = default;
            using var timeoutTokenSource = _timeout.HasValue && requestMessage.Method != HttpConstants.HttpVerbs.DOWNLOAD
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;

            try
            {
                webClient = _webClientFunc();
                timeoutTokenSource?.CancelAfter(_timeout.GetValueOrDefault());

                cancellationToken.ThrowIfCancellationRequested();
                if (timeoutTokenSource?.IsCancellationRequested == true)
                {
                    throw CreateTimeoutException(requestMessage, webClient.BaseAddress);
                }

                if (progress != null)
                {
                    webClient.DownloadProgressChanged += (_, e) => { progress.Report(e.ProgressPercentage); };
                }

                SetWebClientHeaders(webClient, requestMessage);
                if (requestMessage.QueryString != null)
                {
                    webClient.QueryString = requestMessage.QueryString;
                }

                Task<string>? responseTask = null;
                Task<byte[]>? responseAsBytesTask = null;
                if (IsGetOrDownload(requestMessage.Method))
                {
                    if (requestMessage.Method == HttpConstants.HttpVerbs.DOWNLOAD)
                    {
                        responseAsBytesTask = webClient.DownloadDataTaskAsync((string)requestMessage.RequestUri);
                    }
                    else
                    {
                        responseTask = webClient.DownloadStringTaskAsync((string)requestMessage.RequestUri);
                    }
                }
                else
                {
                    byte[] payload;
                    if (requestMessage.Content != null)
                    {
                        webClient.Headers[HttpRequestHeader.ContentType] = requestMessage.ContentType.ValueOrFallback(requestMessage.Content.ContentType);
                        payload = requestMessage.Content.Body;
                    }
                    else
                    {
                        payload = EmptyBytes;
                    }

                    responseAsBytesTask = webClient.UploadDataTaskAsync((string)requestMessage.RequestUri, requestMessage.Method, payload);
                }

                cancellationTokenRegistration =
                    (timeoutTokenSource?.Token ?? cancellationToken).Register(
                        static state => (state as System.Net.WebClient)?.CancelAsync(),
                        webClient
                    );

                if (responseTask != null)
                {
                    response = await responseTask.ConfigureAwait(false);
                }
                else
                {
                    responseAsBytes = await responseAsBytesTask!.ConfigureAwait(false);
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
                    throw RedmineApiExceptionHelper.Create("The operation was canceled by the user.", ex, null, GetMethod(requestMessage), GetEndpoint(webClient?.BaseAddress, requestMessage), null);
                }

                if (timeoutTokenSource?.IsCancellationRequested == true)
                {
                    throw CreateTimeoutException(requestMessage, webClient?.BaseAddress, ex);
                }

                throw HandleWebException(ex, requestMessage, webClient?.BaseAddress, _serializer);
            }
            catch (WebException webException)
            {
                throw HandleWebException(webException, requestMessage, webClient?.BaseAddress, _serializer);
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

        private RedmineApiException CreateTimeoutException(ApiRequestMessage requestMessage, string? baseAddress, Exception? innerException = null)
        {
            return HandleWebException(new WebException(nameof(WebExceptionStatus.Timeout), innerException, WebExceptionStatus.Timeout, null), requestMessage, baseAddress, _serializer);
        }
#endif

        private sealed class DownloadFileResult
        {
            public byte[]? Data { get; }
            public int? StatusCode { get; }
            public string? StatusDescription { get; }

            public DownloadFileResult(byte[]? data, int? statusCode, string? statusDescription = null)
            {
                Data = data;
                StatusCode = statusCode;
                StatusDescription = statusDescription;
            }
        }
        
        private sealed class CallbackContext
        {
            public System.Net.WebClient? Client;

            public DownloadDataCompletedEventHandler? CompletedHandler;
            public DownloadProgressChangedEventHandler? ProgressHandler;

            public byte[]? Data;
            public Exception? Error;
        }

        private static DownloadFileResult DownloadFile(System.Net.WebClient wc, string address,
            IProgress<int>? progress = null)
        {
            var uri = new Uri(new Uri(wc.BaseAddress), address);
            if (progress == null)
            {
                return new DownloadFileResult(wc.DownloadData(uri), (int)HttpStatusCode.OK);
            }

            var done = new ManualResetEvent(initialState: false);
            var context = new CallbackContext
            {
                Client = wc,
            };

            try
            {
                context.CompletedHandler = (sender, e) =>
                {
                    context.Data = e.Cancelled ? null : e.Result;
                    context.Error = e.Error;

                    context.Client.DownloadDataCompleted -= context.CompletedHandler;
                    if (context.ProgressHandler != null)
                    {
                        context.Client.DownloadProgressChanged -= context.ProgressHandler;
                    }

                    done.Set();
                };

                context.ProgressHandler = (sender, e) =>
                {
                    try
                    {
                        progress.Report(e.ProgressPercentage);
                    }
                    catch
                    {
                    }
                };
                wc.DownloadProgressChanged += context.ProgressHandler;

                wc.DownloadDataCompleted += context.CompletedHandler;

                var synchronizationContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    wc.DownloadDataAsync(uri, context);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(synchronizationContext);
                }

                done.WaitOne();
            }
            finally
            {
#if NET40_OR_GREATER
            done.Dispose();
#else
                done.Close();
#endif
            }

            if (context.Error != null)
            {
                throw context.Error as WebException ?? new WebException("Download failed", context.Error);
            }

            return new DownloadFileResult(context.Data ?? [], (int)HttpStatusCode.OK);
        }

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

        private ApiResponseMessage HandleRequest(string address, string verb, RequestOptions? requestOptions = null, ApiRequestMessageContent? content = null, IProgress<int>? progress = null)
        {
            return Send(CreateRequestMessage(address, verb, requestOptions, content), progress);
        }

        private ApiResponseMessage Send(ApiRequestMessage requestMessage, IProgress<int>? progress = null)
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
                    else if (requestMessage.Method == HttpConstants.HttpVerbs.DOWNLOAD)
                    {
                        var downloadResponse = DownloadFile(webClient, (string)requestMessage.RequestUri, progress);

                        responseAsBytes = downloadResponse.Data;
                        // statusCode = downloadResponse.StatusCode ?? HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    byte[] payload;
                    if (requestMessage.Content != null)
                    {
                        webClient.Headers[HttpRequestHeader.ContentType] = requestMessage.ContentType.ValueOrFallback(requestMessage.Content.ContentType);
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
                throw HandleWebException(webException, requestMessage, webClient?.BaseAddress, _serializer);
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

            if (requestMessage.Headers != null)
            {
                foreach (var header in requestMessage.Headers)
                {
                    webClient.Headers[header.Key] = header.Value;
                }
            }

            if (!requestMessage.Accept.IsNullOrWhiteSpace())
            {
                webClient.Headers[HttpRequestHeader.Accept] = requestMessage.Accept;
            }

            if (!requestMessage.UserAgent.IsNullOrWhiteSpace())
            {
                webClient.Headers[HttpRequestHeader.UserAgent] = requestMessage.UserAgent;
            }

            switch (_credentials)
            {
                case RedmineApiKeyAuthentication:
                    webClient.Headers[_credentials.AuthenticationType] = _credentials.Token;
                    break;
                case RedmineBasicAuthentication:
                    webClient.Headers["Authorization"] = $"{_credentials.AuthenticationType} {_credentials.Token}";
                    break;
            }

            if (!requestMessage.ImpersonateUser.IsNullOrWhiteSpace())
            {
                webClient.Headers[RedmineConstants.IMPERSONATE_HEADER_KEY] = requestMessage.ImpersonateUser;
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
        /// <param name="requestMessage">The request that failed.</param>
        /// <param name="baseAddress">The base address of the web client.</param>
        /// <param name="serializer">The serializer.</param>
        /// <exception cref="RedmineException"> </exception>
        private static RedmineApiException HandleWebException(WebException exception, ApiRequestMessage requestMessage, string? baseAddress, IRedmineSerializer serializer)
        {
            var innerException = exception.InnerException ?? exception;
            var method = GetMethod(requestMessage);
            var endpoint = GetEndpoint(baseAddress, requestMessage);

            switch (exception.Status)
            {
                case WebExceptionStatus.Timeout:
                    return RedmineApiExceptionHelper.Create(nameof(WebExceptionStatus.Timeout), innerException, null, method, endpoint, null, isTransient: true);
                case WebExceptionStatus.ProtocolError:
                    if (exception.Response != null)
                    {
                        var statusCode = exception.Response is HttpWebResponse httpResponse
                            ? (int)httpResponse.StatusCode
                            : (int)HttpStatusCode.InternalServerError;
                        var responseHeaders = GetResponseHeaders(exception.Response);

                        using var responseStream = exception.Response.GetResponseStream();
                        if (statusCode == HttpConstants.StatusCodes.UnprocessableEntity && responseStream != null)
                        {
                            string content;
                            try
                            {
                                using var reader = new StreamReader(responseStream);
                                content = reader.ReadToEnd();
                            }
                            catch (Exception ex)
                            {
                                return RedmineApiExceptionHelper.Create(ex.Message, ex, statusCode, method, endpoint, responseHeaders);
                            }

                            return RedmineApiExceptionHelper.CreateUnprocessableEntityException(content, null, serializer, method, endpoint, responseHeaders);
                        }

                        RedmineApiExceptionHelper.MapStatusCodeToException(statusCode, innerException, method, endpoint, responseHeaders);
                    }

                    break;
            }

            return RedmineApiExceptionHelper.Create(exception.Message, innerException, null, method, endpoint, null, IsTransient(exception.Status));
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

        private static string GetMethod(ApiRequestMessage requestMessage)
        {
            return requestMessage.Method == HttpVerbs.DOWNLOAD ? HttpVerbs.GET : requestMessage.Method;
        }

        private static string GetEndpoint(string? baseAddress, ApiRequestMessage requestMessage)
        {
            var address = requestMessage.RequestUri;
            return !baseAddress.IsNullOrWhiteSpace() && Uri.TryCreate(new Uri(baseAddress), address, out var uri)
                ? uri.ToString()
                : address;
        }

        private static IDictionary<string, string>? GetResponseHeaders(WebResponse response)
        {
            if (response is not HttpWebResponse httpResponse)
            {
                return null;
            }

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in httpResponse.Headers.AllKeys)
            {
                headers[key] = httpResponse.Headers[key];
            }

            return headers;
        }
    }
}