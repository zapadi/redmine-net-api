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
#if NET45_OR_GREATER
using System.Net.Security;
#endif

namespace Redmine.Net.Api.Net
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
        /// Gets or sets a value that indicates whether the certificate is checked against the certificate authority revocation list.
        /// </summary>
        bool CheckCertificateRevocationList { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DecompressionMethods? DecompressionFormat { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        int? MaxAutomaticRedirections { get; set; }

        /// <summary>
        /// 
        /// </summary>
        long? MaxRequestContentBufferSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        long? MaxResponseContentBufferSize { get; set; }

    #if NET471_OR_GREATER || NETCOREAPP
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
        IWebProxy Proxy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Scheme { get; set; }

#if NET45_OR_GREATER
        /// <summary>
        /// 
        /// </summary>
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
    
#endif
        
        /// <summary>
        /// 
        /// </summary>
        SecurityProtocolType? SecurityProtocolType { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        bool? UseDefaultCredentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool? UseProxy { get; set; }
    }
}