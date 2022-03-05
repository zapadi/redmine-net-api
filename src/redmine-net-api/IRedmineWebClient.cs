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
using System.Collections.Specialized;
using System.Net;
using System.Net.Cache;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedmineWebClient
    {
        /// <summary>
        /// 
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool UseProxy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool UseCookies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool PreAuthenticate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool KeepAlive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        NameValueCollection QueryString { get;  }

        /// <summary>
        /// 
        /// </summary>
        bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IWebProxy Proxy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        RequestCachePolicy CachePolicy { get; set; }
    }
}