using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Cache;

namespace Redmine.Net.Api.Types
{
     interface IRedmineWebClient{
        Uri BaseAddress { get; set; }
        NameValueCollection QueryString { get; set; }

        bool UseDefaultCredentials { get; set; }
        ICredentials Credentials { get; set; }

        bool UseProxy { get; set; }
        IWebProxy Proxy { get; set; }

        TimeSpan Timeout { get; set; }

        bool UseCookies { get; set; }
        CookieContainer CookieContainer { get; set; }
        
        bool PreAuthenticate { get; set; }

        RequestCachePolicy CachePolicy { get; set; }

        bool KeepAlive { get; set; }
    }
}