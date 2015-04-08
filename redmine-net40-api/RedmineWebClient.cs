/*
   Copyright 2011 - 2015 Adrian Popescu

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
    /// 
    /// </summary>
    public class RedmineWebClient :WebClient
    {
        private readonly CookieContainer container = new CookieContainer();
       
        protected override WebRequest GetWebRequest(Uri address)
        {

            Headers.Add(HttpRequestHeader.Cookie, "redmineCookie");

            var wr = base.GetWebRequest(address);
            var httpWebRequest = wr as HttpWebRequest;

            if (httpWebRequest != null)
            {
                httpWebRequest.CookieContainer = container;

                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                return httpWebRequest;
            }

            return wr;
        }
    }
}