/*
   Copyright 2011 - 2017 Adrian Popescu.

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
    public interface IRedmineWebClient
    {
        string UserAgent { get; set; }

        bool UseProxy { get; set; }

        bool UseCookies { get; set; }

        TimeSpan? Timeout { get; set; }

        CookieContainer CookieContainer { get; set; }

        bool PreAuthenticate { get; set; }

        bool KeepAlive { get; set; }

        NameValueCollection QueryString { get; set; }

        bool UseDefaultCredentials { get; set; }

        ICredentials Credentials { get; set; }

        IWebProxy Proxy { get; set; }

        RequestCachePolicy CachePolicy { get; set; }
    }
}