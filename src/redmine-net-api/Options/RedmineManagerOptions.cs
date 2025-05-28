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
using System.Net;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Http.Clients.WebClient;
using Redmine.Net.Api.Logging;
using Redmine.Net.Api.Serialization;
#if !NET20
using System.Net.Http;
using Redmine.Net.Api.Http.Clients.HttpClient;
#endif

namespace Redmine.Net.Api.Options
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RedmineManagerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public Uri BaseAddress { get; init; }
        
        /// <summary>
        /// Gets or sets the page size for paginated Redmine API responses.
        /// The default page size is 25, but you can customize it as needed. 
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Gets or sets the desired MIME format for Redmine API responses, which represents the way of serialization.
        /// Supported formats include XML and JSON. The default format is XML.
        /// </summary>
        public IRedmineSerializer Serializer { get; init; }
        
        /// <summary>
        /// Gets or sets the authentication method to be used when connecting to the Redmine server.
        /// The available authentication types include API token-based authentication and basic authentication
        /// (using a username and password). You can set an instance of the corresponding authentication class
        /// to use the desired authentication method.
        /// </summary>
        public IRedmineAuthentication Authentication { get; init; }

        /// <summary>
        /// Gets or sets the version of the Redmine server to which this client will connect.
        /// </summary>
        public Version RedmineVersion { get; init; }
        
        public IRedmineLogger Logger { get; init; }
        
        /// <summary>
        /// Gets or sets additional logging configuration options
        /// </summary>
        public RedmineLoggingOptions LoggingOptions { get; init; } = new RedmineLoggingOptions();
        
        /// <summary>
        /// Gets or sets the settings for configuring the Redmine http client.
        /// </summary>
        public IRedmineApiClientOptions ApiClientOptions { get; set; }
        
        /// <summary>
        /// Gets or sets a custom function that creates and returns a specialized instance of the WebClient class.
        /// </summary>
        public Func<WebClient> ClientFunc { get; init; }
        
        /// <summary>
        /// Gets or sets the settings for configuring the Redmine web client.
        /// </summary>
        public RedmineWebClientOptions WebClientOptions {
            get => (RedmineWebClientOptions)ApiClientOptions;
            set => ApiClientOptions = value;
        }
        
        #if !NET20
        /// <summary>
        /// 
        /// </summary>
        public HttpClient HttpClient { get; init; }
        
        /// <summary>
        /// Gets or sets the settings for configuring the Redmine http client.
        /// </summary>
        public RedmineHttpClientOptions HttpClientOptions {
            get => (RedmineHttpClientOptions)ApiClientOptions;
            set => ApiClientOptions = value;
        }
        #endif
    }
}