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
                    webClient.BaseAddress = redmineManagerOptions.BaseAddress?.ToString();
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
            CancellationTokenSource? timeoutTokenSource = null;

            try
            {
                webClient = _webClientFunc();
                request.BaseAddress = webClient.BaseAddress;
                timeoutTokenSource = CreateTimeoutTokenSource(webClient, request, cancellationToken);
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

                var responseTask = request.Method == HttpVerbs.GET
                    ? webClient.DownloadDataTaskAsync(request.Address)
                    : webClient.UploadDataTaskAsync(request.Address, request.Method, GetPayload(webClient, request));

                cancellationTokenRegistration =
                    (timeoutTokenSource?.Token ?? cancellationToken).Register(
                        static state => (state as System.Net.WebClient)?.CancelAsync(),
                        webClient
                    );

                var response = await responseTask.ConfigureAwait(false);

                return request.Method == HttpVerbs.GET && !request.IsDownload
                    ? CreateResponse(webClient, Decode(webClient, response), null)
                    : CreateResponse(webClient, null, response);
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
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is { } errorResponse)
            {
                return CreateErrorResponse(ex, errorResponse);
            }
            finally
            {
#if NET
                await cancellationTokenRegistration.DisposeAsync().ConfigureAwait(false);
#else
                cancellationTokenRegistration.Dispose();
#endif

                timeoutTokenSource?.Dispose();
                webClient?.Dispose();
            }
        }

        private CancellationTokenSource? CreateTimeoutTokenSource(System.Net.WebClient webClient, RedmineTransportRequest request, CancellationToken cancellationToken)
        {
            if (!_timeout.HasValue)
            {
                return null;
            }

            if (!request.IsDownload)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            }

#if NETFRAMEWORK
            // HttpWebRequest.Timeout does not apply to asynchronous requests on .NET Framework, so the download is timed out until its response arrives.
            if (webClient is InternalWebClient internalWebClient)
            {
                var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                internalWebClient.ResponseReceived = () => timeoutTokenSource.CancelAfter(Timeout.Infinite);
                return timeoutTokenSource;
            }
#endif

            return null;
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
                if (request.Method == HttpVerbs.GET)
                {
                    if (request.IsDownload)
                    {
                        responseAsBytes = DownloadFile(webClient, request.Address, request.Progress);
                    }
                    else
                    {
                        response = Decode(webClient, webClient.DownloadData(request.Address));
                    }
                }
                else
                {
                    responseAsBytes = webClient.UploadData(request.Address, request.Method, GetPayload(webClient, request));
                }

                return CreateResponse(webClient, response, responseAsBytes);
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is { } errorResponse)
            {
                return CreateErrorResponse(ex, errorResponse);
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

                    wc.DownloadDataCompleted -= context.CompletedHandler;
                    if (context.ProgressHandler != null)
                    {
                        wc.DownloadProgressChanged -= context.ProgressHandler;
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
                var values = request.Headers.GetValues(key);
                if (values == null)
                {
                    continue;
                }

                foreach (var value in values)
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

            if (request.Body == null)
            {
                throw RedmineApiExceptionHelper.Create("The request body cannot be null when a content type is set.", null, null, request.Method, request.Address, null, errorCode: RedmineApiExceptionHelper.UnknownErrorCode);
            }

            webClient.Headers[HttpRequestHeader.ContentType] = request.ContentType;
            return request.Body;
        }

        private static string? Decode(System.Net.WebClient webClient, byte[] body)
        {
            return RedmineApiClient.DecodeBody(body, webClient.ResponseHeaders, webClient.Encoding);
        }

        private static RedmineTransportResponse CreateResponse(System.Net.WebClient webClient, string? content, byte[]? body)
        {
            var statusCode = webClient is InternalWebClient iwc ? iwc.StatusCode : null;

            return new RedmineTransportResponse((int)(statusCode ?? HttpStatusCode.OK), webClient.ResponseHeaders, body)
            {
                IsSuccessStatusCode = true,
                Content = content,
            };
        }

        private static RedmineTransportResponse CreateErrorResponse(WebException exception, WebResponse response)
        {
            var httpResponse = response as HttpWebResponse;
            var statusCode = httpResponse != null
                ? (int)httpResponse.StatusCode
                : (int)HttpStatusCode.InternalServerError;
            var headers = GetResponseHeaders(httpResponse);

            byte[]? body = null;
            Exception? bodyError = null;
            using (var responseStream = response.GetResponseStream())
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

            return new RedmineTransportResponse(statusCode, headers, body)
            {
                IsSuccessStatusCode = false,
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
    }
}
