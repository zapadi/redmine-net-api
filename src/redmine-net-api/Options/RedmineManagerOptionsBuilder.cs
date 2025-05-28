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
#if NET462_OR_GREATER || NET
using Microsoft.Extensions.Logging;
#endif
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Http;
#if !NET20
using Redmine.Net.Api.Http.Clients.HttpClient;
#endif
using Redmine.Net.Api.Http.Clients.WebClient;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Logging;
using Redmine.Net.Api.Serialization;
#if NET40_OR_GREATER || NET
using System.Net.Http;
#endif
#if  NET462_OR_GREATER || NET
#endif

namespace Redmine.Net.Api.Options
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RedmineManagerOptionsBuilder
    {
        private IRedmineLogger _redmineLogger = RedmineNullLogger.Instance;
        private Action<RedmineLoggingOptions> _configureLoggingOptions;
        
        private enum ClientType
        {
            WebClient,
            HttpClient,
        }
        private ClientType _clientType = ClientType.HttpClient;
        
        /// <summary>
        /// 
        /// </summary>
        public string Host { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; private set; }
        
        /// <summary>
        /// Gets the current serialization type 
        /// </summary>
        public SerializationType SerializationType { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IRedmineAuthentication Authentication { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IRedmineApiClientOptions ClientOptions { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Func<WebClient> ClientFunc { get; private set; }
        
        /// <summary>
        /// Gets or sets the version of the Redmine server to which this client will connect.
        /// </summary>
        public Version Version { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithPageSize(int pageSize)
        {
            PageSize = pageSize;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithHost(string baseAddress)
        {
            Host = baseAddress;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationType"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithSerializationType(SerializationType serializationType)
        {
            SerializationType = serializationType;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithXmlSerialization()
        {
            SerializationType = SerializationType.Xml;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithJsonSerialization()
        {
            SerializationType = SerializationType.Json;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithApiKeyAuthentication(string apiKey)
        {
            Authentication = new RedmineApiKeyAuthentication(apiKey);
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithBasicAuthentication(string login, string password)
        {
            Authentication = new RedmineBasicAuthentication(login, password);
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithLogger(IRedmineLogger logger, Action<RedmineLoggingOptions> configure = null)
        {
            _redmineLogger = logger ?? RedmineNullLogger.Instance;
            _configureLoggingOptions = configure;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithVersion(Version version)
        {
            Version = version;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientFunc"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithWebClient(Func<WebClient> clientFunc)
        {
            _clientType = ClientType.WebClient;
            ClientFunc = clientFunc;
            return this;
        }
        
        /// <summary>
        /// Configures the client to use WebClient with default settings
        /// </summary>
        /// <returns>This builder instance for method chaining</returns>
        public RedmineManagerOptionsBuilder UseWebClient(RedmineWebClientOptions clientOptions = null)
        {
            _clientType = ClientType.WebClient;
            ClientOptions = clientOptions; 
            return this;
        }

#if NET40_OR_GREATER || NET
        /// <summary>
        /// 
        /// </summary>
        public Func<HttpClient> HttpClientFunc { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientFunc"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithHttpClient(Func<HttpClient> clientFunc)
        {
            _clientType = ClientType.HttpClient;
            this.HttpClientFunc = clientFunc;
            return this;
        }
        
        /// <summary>
        /// Configures the client to use HttpClient with default settings
        /// </summary>
        /// <returns>This builder instance for method chaining</returns>
        public RedmineManagerOptionsBuilder UseHttpClient(RedmineHttpClientOptions clientOptions = null)
        {
            _clientType = ClientType.HttpClient;
            ClientOptions = clientOptions;
            return this;
        }

#endif
        
#if  NET462_OR_GREATER || NET
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithLogger(ILogger logger, Action<RedmineLoggingOptions> configure = null)
        {
            _redmineLogger = new MicrosoftLoggerRedmineAdapter(logger);
            _configureLoggingOptions = configure;
            return this;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal RedmineManagerOptions Build()
        {
#if NET45_OR_GREATER || NET
            ClientOptions ??= _clientType switch
            {
                ClientType.WebClient => new RedmineWebClientOptions(),
                ClientType.HttpClient => new RedmineHttpClientOptions(),
                _ => throw new ArgumentOutOfRangeException()
            };
#else
            ClientOptions ??= new RedmineWebClientOptions();
#endif

            var baseAddress = HostHelper.CreateRedmineUri(Host, ClientOptions.Scheme);
            
            var redmineLoggingOptions = ConfigureLoggingOptions();
            
            var options = new RedmineManagerOptions()
            {
                BaseAddress = baseAddress,
                PageSize = PageSize > 0 ? PageSize : RedmineConstants.DEFAULT_PAGE_SIZE_VALUE,
                Serializer = RedmineSerializerFactory.CreateSerializer(SerializationType),
                RedmineVersion = Version,
                Authentication = Authentication ?? new RedmineNoAuthentication(),
                ApiClientOptions = ClientOptions,
                Logger = _redmineLogger,
                LoggingOptions = redmineLoggingOptions,
            };
            
            return options;
        }
        
        private RedmineLoggingOptions ConfigureLoggingOptions()
        {
            if (_configureLoggingOptions == null)
            {
                return null;
            }
            
            var redmineLoggingOptions = new RedmineLoggingOptions();
            _configureLoggingOptions(redmineLoggingOptions);
            return redmineLoggingOptions;
        }
    }
}