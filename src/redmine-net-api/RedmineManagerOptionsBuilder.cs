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
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RedmineManagerOptionsBuilder
    {
        private enum ClientType
        {
            WebClient,
            HttpClient,
        }
        private ClientType _clientType = ClientType.WebClient;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithPageSize(int pageSize)
        {
            this.PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithHost(string baseAddress)
        {
            this.Host = baseAddress;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Host { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeFormat"></param>
        /// <returns></returns>
        internal RedmineManagerOptionsBuilder WithSerializationType(MimeFormat mimeFormat)
        {
            this.SerializationType = mimeFormat == MimeFormat.Xml ? SerializationType.Xml : SerializationType.Json;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationType"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithSerializationType(SerializationType serializationType)
        {
            this.SerializationType = serializationType;
            return this;
        }

        /// <summary>
        /// Gets the current serialization type 
        /// </summary>
        public SerializationType SerializationType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithApiKeyAuthentication(string apiKey)
        {
            this.Authentication = new RedmineApiKeyAuthentication(apiKey);
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
            this.Authentication = new RedmineBasicAuthentication(login, password);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRedmineAuthentication Authentication { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientFunc"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithWebClient(Func<WebClient> clientFunc)
        {
            _clientType = ClientType.WebClient;
            this.ClientFunc = clientFunc;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Func<WebClient> ClientFunc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientOptions"></param>
        /// <returns></returns>
        [Obsolete("Use WithWebClientOptions(IRedmineWebClientOptions clientOptions) instead.")]
        public RedmineManagerOptionsBuilder WithWebClientOptions(IRedmineApiClientOptions clientOptions)
        {
            return WithWebClientOptions((IRedmineWebClientOptions)clientOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientOptions"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithWebClientOptions(IRedmineWebClientOptions clientOptions)
        {
            _clientType = ClientType.WebClient;
            this.WebClientOptions = clientOptions;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Use WebClientOptions instead.")]
        public IRedmineApiClientOptions ClientOptions
        {
            get => WebClientOptions;
            private set { }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public IRedmineWebClientOptions WebClientOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithVersion(Version version)
        {
            this.Version = version;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Version Version { get; set; }

        internal RedmineManagerOptionsBuilder WithVerifyServerCert(bool verifyServerCert)
        {
            this.VerifyServerCert = verifyServerCert;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool VerifyServerCert { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal RedmineManagerOptions Build()
        {
            const string defaultUserAgent = "Redmine.Net.Api.Net";
            var defaultDecompressionFormat = 
            #if NETFRAMEWORK
                DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;
            #else
                DecompressionMethods.All;
            #endif
    #if NET45_OR_GREATER || NETCOREAPP
            WebClientOptions ??= _clientType switch
            {
                ClientType.WebClient => new RedmineWebClientOptions()
                {
                    UserAgent = defaultUserAgent,
                    DecompressionFormat = defaultDecompressionFormat,
                },
                _ => throw new ArgumentOutOfRangeException()
            };
    #else
            WebClientOptions ??= new RedmineWebClientOptions()
                {
                    UserAgent = defaultUserAgent,
                    DecompressionFormat = defaultDecompressionFormat,
                };
    #endif
            var baseAddress = CreateRedmineUri(Host, WebClientOptions.Scheme);
            
            var options = new RedmineManagerOptions()
            {
                BaseAddress = baseAddress,
                PageSize = PageSize > 0 ? PageSize : RedmineConstants.DEFAULT_PAGE_SIZE_VALUE,
                VerifyServerCert = VerifyServerCert,
                Serializer = RedmineSerializerFactory.CreateSerializer(SerializationType),
                RedmineVersion = Version,
                Authentication = Authentication ?? new RedmineNoAuthentication(),
                WebClientOptions = WebClientOptions 
            };
            
            return options;
        }
        
        private static readonly char[] DotCharArray = ['.'];
        
        internal static void EnsureDomainNameIsValid(string domainName)
        {
            if (domainName.IsNullOrWhiteSpace())
            {
                throw new RedmineException("Domain name cannot be null or empty.");
            }
    
            if (domainName.Length > 255)
            {
                throw new RedmineException("Domain name cannot be longer than 255 characters.");
            }

            var labels = domainName.Split(DotCharArray);
            if (labels.Length == 1)
            {
                throw new RedmineException("Domain name is not valid.");
            }
            foreach (var label in labels)
            {
                if (label.IsNullOrWhiteSpace() || label.Length > 63)
                {
                    throw new RedmineException("Domain name must be between 1 and 63 characters.");
                }
                
                if (!char.IsLetterOrDigit(label[0]) || !char.IsLetterOrDigit(label[label.Length - 1]))
                {
                    throw new RedmineException("Domain name starts or ends with a hyphen.");
                }
                
                for (var i = 0; i < label.Length; i++)
                {
                    var c = label[i];

                    if (!char.IsLetterOrDigit(c) && c != '-')
                    {
                        throw new RedmineException("Domain name contains an invalid character.");
                    }

                    if (c != '-')
                    {
                        continue;
                    }
                    
                    if (i + 1 < label.Length && (c ^ label[i+1]) == 0)
                    {
                        throw new RedmineException("Domain name contains consecutive hyphens.");
                    }
                }
            }
        }
        
        internal static Uri CreateRedmineUri(string host, string scheme = null)
        {
            if (host.IsNullOrWhiteSpace() || host.Equals("string.Empty", StringComparison.OrdinalIgnoreCase))
            {
                throw new RedmineException("The host is null or empty.");
            }

            if (!Uri.TryCreate(host, UriKind.Absolute, out var uri))
            {
                host = host.TrimEnd('/', '\\');
                EnsureDomainNameIsValid(host);
                
                if (!host.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) || !host.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    host = $"{scheme ?? Uri.UriSchemeHttps}://{host}";

                    if (!Uri.TryCreate(host, UriKind.Absolute, out uri))
                    {
                        throw new RedmineException("The host is not valid.");
                    }
                }
            }
            
            if (!uri.IsWellFormedOriginalString())
            {
                throw new RedmineException("The host is not well-formed.");
            }

            scheme ??= Uri.UriSchemeHttps;
            var hasScheme = false;
            if (!uri.Scheme.IsNullOrWhiteSpace())
            {
                if (uri.Host.IsNullOrWhiteSpace() && uri.IsAbsoluteUri && !uri.IsFile)
                {
                    if (uri.Scheme.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                    {
                        int port = 0;
                        var portAsString = uri.AbsolutePath.RemoveTrailingSlash();
                        if (!portAsString.IsNullOrWhiteSpace())
                        {
                            int.TryParse(portAsString, out port);
                        }
                             
                        var ub = new UriBuilder(scheme, "localhost", port);
                        return ub.Uri;
                    }
                }
                else
                {
                    if (!IsSchemaHttpOrHttps(uri.Scheme))
                    {
                        throw new RedmineException("Invalid host scheme. Only HTTP and HTTPS are supported.");
                    }

                    hasScheme = true;
                }
            }
            else
            {
                if (!IsSchemaHttpOrHttps(scheme))
                {
                    throw new RedmineException("Invalid host scheme. Only HTTP and HTTPS are supported.");
                }
            }

            var uriBuilder = new UriBuilder();
            
            if (uri.HostNameType == UriHostNameType.IPv6)
            {
                uriBuilder.Scheme = (hasScheme ? uri.Scheme : scheme ?? Uri.UriSchemeHttps);
                uriBuilder.Host = uri.Host;
            }
            else
            {
                if (uri.Authority.IsNullOrWhiteSpace())
                {
                    if (uri.Port == -1)
                    {
                        if (int.TryParse(uri.LocalPath, out var port))
                        {
                            uriBuilder.Port = port;
                        }
                    }

                    uriBuilder.Scheme = scheme ?? Uri.UriSchemeHttps;
                    uriBuilder.Host = uri.Scheme;
                }
                else
                {
                    uriBuilder.Scheme = uri.Scheme;
                    uriBuilder.Port = int.TryParse(uri.LocalPath, out var port) ? port : uri.Port;
                    uriBuilder.Host = uri.Host;
                    if (!uri.LocalPath.IsNullOrWhiteSpace() && !uri.LocalPath.Contains("."))
                    {
                        uriBuilder.Path = uri.LocalPath;
                    }
                }
            }

            try
            {
                return uriBuilder.Uri;
            }
            catch (Exception ex)
            {
                throw new RedmineException(ex.Message);
            }
        }

        private static bool IsSchemaHttpOrHttps(string scheme)
        {
           return  scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeHttps;
        }
    }
}