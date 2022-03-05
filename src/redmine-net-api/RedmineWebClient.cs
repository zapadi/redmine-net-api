/*
   Copyright 2011 - 2022 Adrian Popescu

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
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.Net.WebClient" />
    public class RedmineWebClient : WebClient
    {
        private string redirectUrl = string.Empty;
       
        /// <summary>
        /// 
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [use proxy].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use proxy]; otherwise, <c>false</c>.
        /// </value>
        public bool UseProxy { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [use cookies].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use cookies]; otherwise, <c>false</c>.
        /// </value>
        public bool UseCookies { get; set; }

        /// <summary>
        ///     in milliseconds
        /// </summary>
        /// <value>
        ///     The timeout.
        /// </value>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        ///     Gets or sets the cookie container.
        /// </summary>
        /// <value>
        ///     The cookie container.
        /// </value>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [pre authenticate].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [pre authenticate]; otherwise, <c>false</c>.
        /// </value>
        public bool PreAuthenticate { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [keep alive].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [keep alive]; otherwise, <c>false</c>.
        /// </value>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Scheme { get; set; } = "https";

        /// <summary>
        /// 
        /// </summary>
        public RedirectType Redirect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal IRedmineSerializer RedmineSerializer { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.Net.WebRequest" /> object for the specified resource.
        /// </summary>
        /// <param name="address">A <see cref="System.Uri" /> that identifies the resource to request.</param>
        /// <returns>
        ///     A new <see cref="System.Net.WebRequest" /> object for the specified resource.
        /// </returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);

            if (!(wr is HttpWebRequest httpWebRequest))
            {
                return base.GetWebRequest(address);
            }

            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate |
                                                    DecompressionMethods.None;

            if (UseCookies)
            {
                httpWebRequest.Headers.Add(HttpRequestHeader.Cookie, "redmineCookie");
                httpWebRequest.CookieContainer = CookieContainer;
            }
            
            httpWebRequest.KeepAlive = KeepAlive;
            httpWebRequest.CachePolicy = CachePolicy;
          
            if (Timeout != null)
            {
                httpWebRequest.Timeout = (int)Timeout.Value.TotalMilliseconds;
            }
            
            return httpWebRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = null;

            try
            {
                response = base.GetWebResponse(request);
            }
            catch (WebException webException)
            {
                webException.HandleWebException(RedmineSerializer);
            }

            switch (response)
            {
                case null:
                    return null;
                case HttpWebResponse _:
                    HandleRedirect(request, response);
                    HandleCookies(request, response);
                    break;
            }

            return response;
        }

        /// <summary>
        /// Handles redirect response if needed
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        protected void HandleRedirect(WebRequest request, WebResponse response)
        {
            var webResponse = response as HttpWebResponse;

            if (Redirect == RedirectType.None)
            {
                return;
            }

            if (webResponse == null)
            {
                return;
            }

            var code = webResponse.StatusCode;

            if (code == HttpStatusCode.Found || code == HttpStatusCode.SeeOther || code == HttpStatusCode.MovedPermanently || code == HttpStatusCode.Moved)
            {
                redirectUrl = webResponse.Headers["Location"];

                var isAbsoluteUri = new Uri(redirectUrl).IsAbsoluteUri;

                if (!isAbsoluteUri)
                {
                    var webRequest = request as HttpWebRequest;
                    var host = webRequest?.Headers["Host"] ?? string.Empty;

                    if (Redirect == RedirectType.All)
                    {
                        host = $"{host}{webRequest?.RequestUri.AbsolutePath}";

                        host = host.Substring(0, host.LastIndexOf('/'));
                    }

                    // Have to make sure that the "/" symbol is between the "host" and "redirect" strings
                    if (!redirectUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase) && !host.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        redirectUrl = $"/{redirectUrl}";
                    }

                    redirectUrl = $"{host}{redirectUrl}";
                }

                if (!redirectUrl.StartsWith(Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    redirectUrl = $"{Scheme}://{redirectUrl}";
                }
            }
            else
            {
                redirectUrl = string.Empty;
            }
        }
        
        /// <summary>
        /// Handles additional cookies
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        protected void HandleCookies(WebRequest request, WebResponse response)
        {
            if (!(response is HttpWebResponse webResponse)) return;

            var webRequest = request as HttpWebRequest;

            if (webResponse.Cookies.Count <= 0) return;
            
            var col = new CookieCollection();

            foreach (Cookie c in webResponse.Cookies)
            {
                col.Add(new Cookie(c.Name, c.Value, c.Path, webRequest?.Headers["Host"]));
            }

            if (CookieContainer == null)
            {
                CookieContainer = new CookieContainer();
            }

            CookieContainer.Add(col);
        }
    }
}