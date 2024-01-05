/*
   Copyright 2011 - 2023 Adrian Popescu

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
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Net.WebClient;

internal sealed class InternalWebClient : System.Net.WebClient
{
    private readonly IRedmineApiClientOptions _webClientSettings;

    #pragma warning disable SYSLIB0014
    public InternalWebClient(RedmineManagerOptions redmineManagerOptions)
    {
        _webClientSettings = redmineManagerOptions.ClientOptions;
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

            httpWebRequest.UserAgent = _webClientSettings.UserAgent.ValueOrFallback("RedmineDotNetAPIClient");

            httpWebRequest.AutomaticDecompression = _webClientSettings.DecompressionFormat ?? DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;

            AssignIfHasValue(_webClientSettings.AutoRedirect, value => httpWebRequest.AllowAutoRedirect = value);

            AssignIfHasValue(_webClientSettings.MaxAutomaticRedirections, value => httpWebRequest.MaximumAutomaticRedirections = value);

            AssignIfHasValue(_webClientSettings.KeepAlive, value => httpWebRequest.KeepAlive = value);

            AssignIfHasValue(_webClientSettings.Timeout, value => httpWebRequest.Timeout = (int) value.TotalMilliseconds);

            AssignIfHasValue(_webClientSettings.PreAuthenticate, value => httpWebRequest.PreAuthenticate = value);

            AssignIfHasValue(_webClientSettings.UseCookies, value => httpWebRequest.CookieContainer = _webClientSettings.CookieContainer);

            AssignIfHasValue(_webClientSettings.UnsafeAuthenticatedConnectionSharing, value => httpWebRequest.UnsafeAuthenticatedConnectionSharing = value);

            AssignIfHasValue(_webClientSettings.MaxResponseContentBufferSize, value => { });

            if (_webClientSettings.DefaultHeaders != null)
            {
                httpWebRequest.Headers = new WebHeaderCollection();
                foreach (var defaultHeader in _webClientSettings.DefaultHeaders)
                {
                    httpWebRequest.Headers.Add(defaultHeader.Key, defaultHeader.Value);
                }
            }

            httpWebRequest.CachePolicy = _webClientSettings.RequestCachePolicy;

            httpWebRequest.Proxy = _webClientSettings.Proxy;

            httpWebRequest.Credentials = _webClientSettings.Credentials;

            #if !(NET20)
                if (_webClientSettings.ClientCertificates != null)
                {
                    httpWebRequest.ClientCertificates = _webClientSettings.ClientCertificates;
                }
            #endif

            #if (NET45_OR_GREATER || NETCOREAPP)
                httpWebRequest.ServerCertificateValidationCallback = _webClientSettings.ServerCertificateValidationCallback;
            #endif

            if (_webClientSettings.ProtocolVersion != default)
            {
                httpWebRequest.ProtocolVersion = _webClientSettings.ProtocolVersion;
            }

            return httpWebRequest;
        }
        catch (Exception webException)
        {
            throw new RedmineException(webException.GetBaseException().Message, webException);
        }
    }

    private static void AssignIfHasValue<T>(T? nullableValue, Action<T> assignAction) where T : struct
    {
        if (nullableValue.HasValue)
        {
            assignAction(nullableValue.Value);
        }
    }
}