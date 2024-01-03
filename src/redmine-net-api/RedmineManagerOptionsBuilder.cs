using System;
using System.Net;
using System.Xml.Serialization;
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
            None,
            WebClient,
        }
        private ClientType _clientType = ClientType.None;
        
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
        /// 
        /// </summary>
        public SerializationType SerializationType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authentication"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithAuthentication(IRedmineAuthentication authentication)
        {
            this.Authentication = authentication;
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
            if (clientFunc != null)
            {
                _clientType = ClientType.WebClient;
            }

            if (clientFunc == null && _clientType == ClientType.WebClient)
            {
                _clientType = ClientType.None;
            }
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
        public RedmineManagerOptionsBuilder WithClientOptions(IRedmineApiClientOptions clientOptions)
        {
            this.ClientOptions = clientOptions;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRedmineApiClientOptions ClientOptions { get; private set; }

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
            ClientOptions ??= new RedmineWebClientOptions();
            
            var baseAddress = CreateRedmineUri(Host, ClientOptions.Scheme);
            
            var options = new RedmineManagerOptions()
            {
                BaseAddress = baseAddress,
                PageSize = PageSize > 0 ? PageSize : RedmineConstants.DEFAULT_PAGE_SIZE_VALUE,
                VerifyServerCert = VerifyServerCert,
                Serializer = RedmineSerializerFactory.CreateSerializer(SerializationType),
                RedmineVersion = Version,
                Authentication = Authentication,
                ClientOptions = ClientOptions 
            };
            
            return options;
        }
        
        
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

            var labels = domainName.Split('.');
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