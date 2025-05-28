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
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Http
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedmineApiClientOptions
    {
        /// <summary>
        /// 
        /// </summary>
        bool? AutoRedirect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DecompressionMethods? DecompressionFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, string> DefaultHeaders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IWebProxy Proxy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? MaxAutomaticRedirections { get; set; }
       
#if NET471_OR_GREATER || NET
        /// <summary>
        /// 
        /// </summary>
        int? MaxConnectionsPerServer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? MaxResponseHeadersLength { get; set; }
#endif
        /// <summary>
        /// 
        /// </summary>
        bool? PreAuthenticate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        RequestCachePolicy RequestCachePolicy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Scheme { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool? UseCookies { get; set; }

#if NETFRAMEWORK

        /// <summary>
        /// 
        /// </summary>
        bool CheckCertificateRevocationList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        long? MaxRequestContentBufferSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        long? MaxResponseContentBufferSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool? UseDefaultCredentials { get; set; }
#endif
        /// <summary>
        /// 
        /// </summary>
        bool? UseProxy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Only HTTP/1.0 and HTTP/1.1 version requests are currently supported.</remarks>
        Version ProtocolVersion { get; set; }

     
#if NET40_OR_GREATER || NET
        /// <summary>
        /// 
        /// </summary>
        public X509CertificateCollection ClientCertificates { get; set; }
#endif
    }
}