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

using System.Net;
#if (NET45_OR_GREATER || NET)
using System.Net.Security;
#endif
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Http.Clients.WebClient;
/// <summary>
/// 
/// </summary>
public sealed class RedmineWebClientOptions: RedmineApiClientOptions
{
    
    /// <summary>
    /// 
    /// </summary>
    public bool? KeepAlive { get; set; }

   /// <summary>
    /// 
    /// </summary>
    public bool? UnsafeAuthenticatedConnectionSharing { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? DefaultConnectionLimit { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? DnsRefreshTimeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? EnableDnsRoundRobin { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxServicePoints { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxServicePointIdleTime { get; set; }

    #if(NET46_OR_GREATER || NET)
    /// <summary>
    /// 
    /// </summary>
    public bool? ReusePort { get; set; }
    #endif

    /// <summary>
    /// 
    /// </summary>
    public SecurityProtocolType? SecurityProtocolType { get; set; }
    
#if (NET45_OR_GREATER || NET)
    /// <summary>
    /// 
    /// </summary>
    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
    #endif
}