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
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Redmine.Net.Api.Exceptions;
#if !(NET20)
using System.Threading.Tasks;
#endif
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Net.WebClient
{
    internal sealed class WebClientTransport : IRedmineTransport
    {
        private static readonly byte[] EmptyBytes = Encoding.UTF8.GetBytes(string.Empty);
        private readonly Func<System.Net.WebClient> _webClientFunc;
        private readonly TimeSpan? _timeout;

        public WebClientTransport(RedmineManagerOptions redmineManagerOptions)
        {
            _webClientFunc = CreateWebClientFunc(redmineManagerOptions);
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

#if !(NET20)
        public async Task<RedmineTransportResponse> SendAsync(RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            System.Net.WebClient? webClient = null;
            CancellationTokenRegistration cancellationTokenRegistration = default;
            using var timeoutTokenSource = _timeout.HasValue && request.Method != HttpConstants.HttpVerbs.DOWNLOAD
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;

            try
            {
                webClient = _webClientFunc();
                request.BaseAddress = webClient.BaseAddress;
                timeoutTokenSource?.CancelAfter(_timeout.GetValueOrDefault());

                cancellationToken.ThrowIfCancellationRequested();
                if (timeoutTokenSource?.IsCancellationRequested == true)
                {
                    throw new WebException(nameof(WebExceptionStatus.Timeout), null, WebExceptionStatus.Timeout, null);
                }

                if (request.Progress != null)
                {
                    var progress = request.Progress;
                    webClient.DownloadProgressChanged += (_, e) => { progress.Report(e.ProgressPercentage); };
                }

                SetWebClientHeaders(webClient, request);

                Task<string>? responseTask = null;
                Task<byte[]>? responseAsBytesTask = null;
                if (IsGetOrDownload(request.Method))
                {
                    if (request.Method == HttpConstants.HttpVerbs.DOWNLOAD)
                    {
                        responseAsBytesTask = webClient.DownloadDataTaskAsync(request.Address);
                    }
                    else
                    {
                        responseTask = webClient.DownloadStringTaskAsync(request.Address);
                    }
                }
                else
                {
                    responseAsBytesTask = webClient.UploadDataTaskAsync(request.Address, request.Method, GetPayload(webClient, request));
                }

                cancellationTokenRegistration =
                    (timeoutTokenSource?.Token ?? cancellationToken).Register(
                        static state => (state as System.Net.WebClient)?.CancelAsync(),
                        webClient
                    );

                string? response = null;
                byte[]? responseAsBytes = null;
                if (responseTask != null)
                {
                    response = await responseTask.ConfigureAwait(false);
                }
                else
                {
                    responseAsBytes = await responseAsBytesTask!.ConfigureAwait(false);
                }

                return CreateResponse(webClient, response, responseAsBytes);
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ex.Message, ex, cancellationToken);
                }

                if (timeoutTokenSource?.IsCancellationRequested == true)
                {
                    throw new WebException(nameof(WebExceptionStatus.Timeout), ex, WebExceptionStatus.Timeout, null);
                }

                throw;
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                return CreateErrorResponse(ex);
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
        }
#endif

        public RedmineTransportResponse Send(RedmineTransportRequest request)
        {
            System.Net.WebClient? webClient = null;

            try
            {
                webClient = _webClientFunc();
                request.BaseAddress = webClient.BaseAddress;

                SetWebClientHeaders(webClient, request);

                string? response = null;
                byte[]? responseAsBytes = null;
                if (IsGetOrDownload(request.Method))
                {
                    if (request.Method == HttpConstants.HttpVerbs.GET)
                    {
                        response = webClient.DownloadString(request.Address);
                    }
                    else if (request.Method == HttpConstants.HttpVerbs.DOWNLOAD)
                    {
                        responseAsBytes = DownloadFile(webClient, request.Address, request.Progress);
                    }
                }
                else
                {
                    responseAsBytes = webClient.UploadData(request.Address, request.Method, GetPayload(webClient, request));
                }

                return CreateResponse(webClient, response, responseAsBytes);
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                return CreateErrorResponse(ex);
            }
            finally
            {
                webClient?.Dispose();
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

        private static byte[] DownloadFile(System.Net.WebClient wc, string address, IProgress<int>? progress = null)
        {
            var uri = new Uri(new Uri(wc.BaseAddress), address);
            if (progress == null)
            {
                return wc.DownloadData(uri);
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
                    context.Data = e.Error == null && !e.Cancelled ? e.Result : null;
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

            return context.Data ?? [];
        }

        private static void SetWebClientHeaders(System.Net.WebClient webClient, RedmineTransportRequest request)
        {
            if (request.QueryString != null)
            {
                webClient.QueryString = request.QueryString;
            }

            if (request.Headers == null)
            {
                return;
            }

            foreach (string key in request.Headers)
            {
                foreach (var value in request.Headers.GetValues(key)!)
                {
                    webClient.Headers[key] = value;
                }
            }
        }

        private static byte[] GetPayload(System.Net.WebClient webClient, RedmineTransportRequest request)
        {
            if (request.ContentType == null)
            {
                return EmptyBytes;
            }

            webClient.Headers[HttpRequestHeader.ContentType] = request.ContentType;
            return request.Body!;
        }

        private static RedmineTransportResponse CreateResponse(System.Net.WebClient webClient, string? content, byte[]? body)
        {
            var statusCode = webClient is InternalWebClient iwc ? iwc.StatusCode : null;

            return new RedmineTransportResponse
            {
                StatusCode = (int)(statusCode ?? HttpStatusCode.OK),
                IsSuccessStatusCode = true,
                Headers = webClient.ResponseHeaders,
                Content = content,
                Body = body,
            };
        }

        private static RedmineTransportResponse CreateErrorResponse(WebException exception)
        {
            var httpResponse = exception.Response as HttpWebResponse;
            var statusCode = httpResponse != null
                ? (int)httpResponse.StatusCode
                : (int)HttpStatusCode.InternalServerError;
            var headers = GetResponseHeaders(httpResponse);

            byte[]? body = null;
            Exception? bodyError = null;
            using (var responseStream = exception.Response!.GetResponseStream())
            {
                if (statusCode == HttpConstants.StatusCodes.UnprocessableEntity && responseStream != null)
                {
                    try
                    {
                        body = ReadAllBytes(responseStream);
                    }
                    catch (Exception ex)
                    {
                        bodyError = ex;
                    }
                }
            }

            return new RedmineTransportResponse
            {
                StatusCode = statusCode,
                Headers = headers,
                Body = body,
                BodyError = bodyError,
                Error = exception,
            };
        }

        private static NameValueCollection? GetResponseHeaders(HttpWebResponse? response)
        {
            if (response == null)
            {
                return null;
            }

            var headers = new NameValueCollection();
            foreach (var key in response.Headers.AllKeys)
            {
                headers[key] = response.Headers[key];
            }

            return headers;
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            var buffer = new byte[8192];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, read);
            }

            return memoryStream.ToArray();
        }

        private static bool IsGetOrDownload(string method)
        {
            return method is HttpVerbs.GET or HttpVerbs.DOWNLOAD;
        }
    }
}
