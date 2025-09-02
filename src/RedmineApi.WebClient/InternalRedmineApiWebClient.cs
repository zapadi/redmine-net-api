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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.WebClient;

/// <summary>
/// 
/// </summary>
internal sealed partial class InternalRedmineApiWebClient : RedmineApiClient
{
    private static readonly byte[] EmptyBytes = Encoding.UTF8.GetBytes(string.Empty);
    private readonly Func<System.Net.WebClient> _webClientFunc;
    private readonly IRedmineAuthentication _credentials;

    public InternalRedmineApiWebClient(IRedmineAuthentication auth, string serializerType, WebClientApiOptions redmineManagerOptions)
        : this(() => new InternalWebClient(redmineManagerOptions), auth, serializerType)
    {
        ApplyServiceManagerSettings(redmineManagerOptions.ClientOptions);
    }

    public InternalRedmineApiWebClient(Func<System.Net.WebClient> webClientFunc, IRedmineAuthentication auth, string serializerType)
        : base(serializerType)
    {
        _credentials = auth;
        _webClientFunc = webClientFunc;
    }

    protected override object CreateContentFromPayload(string payload, string contentType)
    {
        return ApiRequestContent.CreateString(payload, contentType);
    }

    protected override object CreateContentFromBytes(byte[] data)
    {
        return ApiRequestContent.CreateBinary(data);
    }

    protected override ApiResponseMessage HandleRequest(string address, string verb, RequestOptions? requestOptions = null, object? content = null, IProgress<int>? progress = null)
    {
        var requestMessage = CreateRequestMessage(address, verb, requestOptions, content as ApiRequestContent);

        var responseMessage = Send(requestMessage, progress);

        return responseMessage;
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
                if (requestMessage.Method == HttpConstants.HttpVerbs.DOWNLOAD)
                {
                    responseAsBytes = DownloadWithProgress(requestMessage.RequestUri, webClient, progress);
                }
                else
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
            Headers = responseHeaders,
            Content = response,
            RawContent = responseAsBytes,
            StatusCode = (int)(statusCode ?? HttpStatusCode.OK),
        };
    }

    private void SetWebClientHeaders(System.Net.WebClient webClient, ApiRequestMessage requestMessage)
    {
        if (!StringExtensions.IsNullOrWhiteSpace(requestMessage.ContentType))
        {
            webClient.Headers.Add(RedmineConstants.CONTENT_TYPE_HEADER_KEY, requestMessage.ContentType);
        }

        switch (_credentials)
        {
            case RedmineApiKeyAuthentication:
                webClient.Headers.Add(RedmineConstants.API_KEY_AUTHORIZATION_HEADER_KEY, _credentials.Token);
                break;
            case RedmineBasicAuthentication:
                webClient.Headers.Add(RedmineConstants.AUTHORIZATION_HEADER_KEY, _credentials.Token);
                break;
        }

        if (!StringExtensions.IsNullOrWhiteSpace(requestMessage.ImpersonateUser))
        {
            webClient.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, requestMessage.ImpersonateUser);
        }

        if (!StringExtensions.IsNullOrWhiteSpace(requestMessage.UserAgent))
        {
            webClient.Headers.Add(RedmineConstants.USER_AGENT_HEADER_KEY, requestMessage.UserAgent);
        }

        if (requestMessage.Headers is not { Count: > 0 })
        {
            return;
        }

        foreach (var header in requestMessage.Headers)
        {
            webClient.Headers.Add((string)header.Key, header.Value);
        }
    }

    private static void ApplyServiceManagerSettings(WebClientApiOptions.TransporterOptions? options)
    {
        if (options == null)
        {
            return;
        }

        if (options.SecurityProtocolType.HasValue)
        {
            ServicePointManager.SecurityProtocol = options.SecurityProtocolType.Value;
        }

        if (options.DefaultConnectionLimit.HasValue)
        {
            ServicePointManager.DefaultConnectionLimit = options.DefaultConnectionLimit.Value;
        }

        if (options.DnsRefreshTimeout.HasValue)
        {
            ServicePointManager.DnsRefreshTimeout = options.DnsRefreshTimeout.Value;
        }

        if (options.EnableDnsRoundRobin.HasValue)
        {
            ServicePointManager.EnableDnsRoundRobin = options.EnableDnsRoundRobin.Value;
        }

        if (options.MaxServicePoints.HasValue)
        {
            ServicePointManager.MaxServicePoints = options.MaxServicePoints.Value;
        }

        if (options.MaxServicePointIdleTime.HasValue)
        {
            ServicePointManager.MaxServicePointIdleTime = options.MaxServicePointIdleTime.Value;
        }

#if(NET46_OR_GREATER || NET)
        if (options.ReusePort.HasValue)
        {
            ServicePointManager.ReusePort = options.ReusePort.Value;
        }
#endif
#if !NET
        if (options.CheckCertificateRevocationList)
        {
            ServicePointManager.CheckCertificateRevocationList = true;
        }
#endif
    }

    private static ApiRequestMessage CreateRequestMessage(string address, string verb, RequestOptions? requestOptions = null, ApiRequestContent? content = null)
    {
        var req = new ApiRequestMessage
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

    private static byte[] DownloadWithProgress(string url, System.Net.WebClient webClient, IProgress<int>? progress = null)
    {
        var contentLength = GetContentLength(webClient);
        byte[] data;
        if (contentLength > 0)
        {
            using var respStream = webClient.OpenRead(url) ?? throw new RedmineApiException(RedmineApiErrorCode.Unknown, "Response stream is null");
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
        else
        {
            data = webClient.DownloadData(url);
            progress?.Report(100);
        }

        return data;
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
                throw new RedmineApiException(RedmineApiErrorCode.Timeout,nameof(WebExceptionStatus.Timeout), innerException);
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
                            throw new RedmineApiException(RedmineApiErrorCode.Unknown,ex.Message, ex);
                        }
                    }

                    RedmineApiExceptionHelper.MapStatusCodeToException(statusCode, innerException);
                }

                break;
        }

        throw new RedmineApiException(RedmineApiErrorCode.Unknown,exception.Message, innerException);
    }

    private sealed class InternalWebClient : System.Net.WebClient
    {
        private readonly RedmineApiClientOptions _redmineManagerOptions;
        private readonly WebClientApiOptions.TransporterOptions? _webClientOptions;

#pragma warning disable SYSLIB0014
        public InternalWebClient(WebClientApiOptions redmineManagerOptions)
        {
            _redmineManagerOptions = redmineManagerOptions;
            _webClientOptions = redmineManagerOptions.ClientOptions;
            BaseAddress = redmineManagerOptions.BaseAddress;
        }
#pragma warning restore SYSLIB0014

        protected override WebRequest GetWebRequest(Uri address)
        {
            try
            {
                var webRequest = base.GetWebRequest(address);

                if (webRequest is not HttpWebRequest httpWebRequest)
                {
                    return base.GetWebRequest(address);
                }

                if (_webClientOptions == null)
                {
                    return httpWebRequest;
                }

                httpWebRequest.UserAgent = _redmineManagerOptions.UserAgent;
                SetIfNotNull<TimeSpan>(_redmineManagerOptions.Timeout, value => httpWebRequest.Timeout = (int)value.TotalMilliseconds);

                SetIfNotNull(_webClientOptions.DecompressionFormat, value => httpWebRequest.AutomaticDecompression = value);
                SetIfNotNull(_webClientOptions.AutoRedirect, value => httpWebRequest.AllowAutoRedirect = value);
                SetIfNotNull(_webClientOptions.MaxAutomaticRedirections, value => httpWebRequest.MaximumAutomaticRedirections = value);
                SetIfNotNull(_webClientOptions.KeepAlive, value => httpWebRequest.KeepAlive = value);
                SetIfNotNull(_webClientOptions.PreAuthenticate, value => httpWebRequest.PreAuthenticate = value);
                SetIfNotNull(_webClientOptions.UseCookies, value => httpWebRequest.CookieContainer = _webClientOptions.CookieContainer);
                SetIfNotNull(_webClientOptions.UnsafeAuthenticatedConnectionSharing, value => httpWebRequest.UnsafeAuthenticatedConnectionSharing = value);
                SetIfNotNull(_webClientOptions.MaxResponseContentBufferSize, value => { });

                if (_webClientOptions.DefaultHeaders != null)
                {
                    httpWebRequest.Headers = [];
                    foreach (var defaultHeader in _webClientOptions.DefaultHeaders)
                    {
                        httpWebRequest.Headers.Add(defaultHeader.Key, defaultHeader.Value);
                    }
                }

                httpWebRequest.CachePolicy = _webClientOptions.RequestCachePolicy;

                if (_webClientOptions.Proxy != null)
                {
                    httpWebRequest.Proxy = _webClientOptions.Proxy;
                }

                httpWebRequest.Credentials = _webClientOptions.Credentials;

#if (NET40_OR_GREATER || NET)
                if (_webClientOptions.ClientCertificates != null)
                {
                    httpWebRequest.ClientCertificates = _webClientOptions.ClientCertificates;
                }
#endif

#if (NET45_OR_GREATER || NET)
                httpWebRequest.ServerCertificateValidationCallback = _webClientOptions.ServerCertificateValidationCallback;
#endif

                if (_webClientOptions.ProtocolVersion != null)
                {
                    httpWebRequest.ProtocolVersion = _webClientOptions.ProtocolVersion;
                }

                return httpWebRequest;
            }
            catch (Exception webException)
            {
                throw new RedmineApiException(RedmineApiErrorCode.Unknown,webException.GetBaseException().Message, webException);
            }
        }

        public HttpStatusCode StatusCode { get; private set; }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            if (response is HttpWebResponse httpResponse)
            {
                StatusCode = httpResponse.StatusCode;
            }

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = base.GetWebResponse(request, result);
            if (response is HttpWebResponse httpResponse)
            {
                StatusCode = httpResponse.StatusCode;
            }

            return response;
        }

        private static void SetIfNotNull<T>(T? nullableValue, Action<T> assignAction) where T : struct
        {
            if (nullableValue.HasValue)
            {
                assignAction(nullableValue.Value);
            }
        }
    }
}