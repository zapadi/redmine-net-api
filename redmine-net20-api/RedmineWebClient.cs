/*
   Copyright 2011 - 2016 Adrian Popescu.

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

namespace Redmine.Net.Api
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.Net.WebClient" />
    public class RedmineWebClient : WebClient
    {
        private const string UA = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:8.0) Gecko/20100101 Firefox/8.0";

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
        ///     in miliseconds
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
        ///     Returns a <see cref="T:System.Net.WebRequest" /> object for the specified resource.
        /// </summary>
        /// <param name="address">A <see cref="T:System.Uri" /> that identifies the resource to request.</param>
        /// <returns>
        ///     A new <see cref="T:System.Net.WebRequest" /> object for the specified resource.
        /// </returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);
            var httpWebRequest = wr as HttpWebRequest;

            if (httpWebRequest != null)
            {
                if (UseCookies)
                {
                    httpWebRequest.Headers.Add(HttpRequestHeader.Cookie, "redmineCookie");
                    httpWebRequest.CookieContainer = CookieContainer;
                }
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate |
                                                        DecompressionMethods.None;
                httpWebRequest.PreAuthenticate = PreAuthenticate;
                httpWebRequest.KeepAlive = KeepAlive;
                httpWebRequest.UseDefaultCredentials = UseDefaultCredentials;
                httpWebRequest.Credentials = Credentials;
                httpWebRequest.UserAgent = UA;
                httpWebRequest.CachePolicy = CachePolicy;

                if (UseProxy)
                {
                    if (Proxy != null)
                    {
                        Proxy.Credentials = Credentials;
                    }
                    httpWebRequest.Proxy = Proxy;
                }

                if (Timeout != null)
                    httpWebRequest.Timeout = Timeout.Value.Milliseconds;

                return httpWebRequest;
            }

            return base.GetWebRequest(address);
        }
    }
}