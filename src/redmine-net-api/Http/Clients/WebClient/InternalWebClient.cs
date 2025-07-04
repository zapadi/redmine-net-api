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
using System.Net;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Options;

namespace Redmine.Net.Api.Http.Clients.WebClient;

internal sealed class InternalWebClient : System.Net.WebClient
{
    private readonly RedmineWebClientOptions _webClientOptions;

    #pragma warning disable SYSLIB0014
    public InternalWebClient(RedmineManagerOptions redmineManagerOptions)
    {
        _webClientOptions = redmineManagerOptions.WebClientOptions;
        BaseAddress = redmineManagerOptions.BaseAddress.ToString();
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

            httpWebRequest.UserAgent = _webClientOptions.UserAgent;

            AssignIfHasValue(_webClientOptions.DecompressionFormat, value => httpWebRequest.AutomaticDecompression = value);

            AssignIfHasValue(_webClientOptions.AutoRedirect, value => httpWebRequest.AllowAutoRedirect = value);

            AssignIfHasValue(_webClientOptions.MaxAutomaticRedirections, value => httpWebRequest.MaximumAutomaticRedirections = value);

            AssignIfHasValue(_webClientOptions.KeepAlive, value => httpWebRequest.KeepAlive = value);

            AssignIfHasValue(_webClientOptions.Timeout, value => httpWebRequest.Timeout = (int) value.TotalMilliseconds);

            AssignIfHasValue(_webClientOptions.PreAuthenticate, value => httpWebRequest.PreAuthenticate = value);

            AssignIfHasValue(_webClientOptions.UseCookies, value => httpWebRequest.CookieContainer = _webClientOptions.CookieContainer);

            AssignIfHasValue(_webClientOptions.UnsafeAuthenticatedConnectionSharing, value => httpWebRequest.UnsafeAuthenticatedConnectionSharing = value);

            AssignIfHasValue(_webClientOptions.MaxResponseContentBufferSize, value => { });

            if (_webClientOptions.DefaultHeaders != null)
            {
                httpWebRequest.Headers = new WebHeaderCollection();
                foreach (var defaultHeader in _webClientOptions.DefaultHeaders)
                {
                    httpWebRequest.Headers.Add(defaultHeader.Key, defaultHeader.Value);
                }
            }

            httpWebRequest.CachePolicy = _webClientOptions.RequestCachePolicy;

            httpWebRequest.Proxy = _webClientOptions.Proxy;

            httpWebRequest.Credentials = _webClientOptions.Credentials;

            #if NET40_OR_GREATER || NET
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
            throw new RedmineException(webException.GetBaseException().Message, webException);
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

    private static void AssignIfHasValue<T>(T? nullableValue, Action<T> assignAction) where T : struct
    {
        if (nullableValue.HasValue)
        {
            assignAction(nullableValue.Value);
        }
    }
}