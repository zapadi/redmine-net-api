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
using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Serialization;

namespace Padi.RedmineApi.Builders;

/// <summary>
/// 
/// </summary>
public sealed class RedmineManagerOptionsBuilder
{
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
    public int PageSize { get; private set; }
        
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
    public IRedmineAuthentication Authentication { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public RedmineManagerOptionsBuilder WithRedmineVersion(Version version)
    {
        Version = version;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    public Version Version { get; set; }

    private IRedmineClientFactory _clientFactory;

    /// <summary>
    /// Configures the client factory to be used for creating API clients
    /// </summary>
    /// <param name="factory">The factory implementation</param>
    /// <returns>The builder instance for method chaining</returns>
    public RedmineManagerOptionsBuilder WithClientFactory(IRedmineClientFactory factory)
    {
        _clientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        return this;
    }

    private IRedmineSerializerConfiguration _serializerConfiguration;
    
    /// <summary>
    /// Configures the serializer to use for Redmine API communication
    /// </summary>
    /// <param name="configuration">The serializer configuration</param>
    /// <returns>This builder instance for method chaining</returns>
    public RedmineManagerOptionsBuilder WithSerializer(IRedmineSerializerConfiguration configuration)
    {
        _serializerConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal RedmineManagerOptions Build()
    {
        if (_serializerConfiguration == null)
        {
            throw new ArgumentException("A serializer configuration must be specified.");
        }

        if (_clientFactory == null)
        {
            throw new ArgumentException("A client factory must be specified.");
        }

        var serializer = _serializerConfiguration.CreateSerializer();
        var auth = Authentication ?? new RedmineNoAuthentication();
        var options = new RedmineManagerOptions()
        {
            PageSize = PageSize > 0 ? PageSize : RedmineConstants.DEFAULT_PAGE_SIZE_VALUE,
            Serializer = serializer,
            RedmineVersion = Version,
            Authentication = auth,
            ApiClient = _clientFactory.CreateClient(auth, serializer.Format)
        };
            
        return options;
    }
    
    private static readonly char[] DotCharArray = ['.'];
        
    internal static void EnsureDomainNameIsValid(string domainName)
    {
        if (domainName.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Domain name cannot be null or empty.", nameof(domainName));
        }
    
        if (domainName.Length > 255)
        {
            throw new ArgumentException("Domain name cannot be longer than 255 characters.", nameof(domainName));
        }

        var labels = domainName.Split(DotCharArray);
        if (labels.Length == 1)
        {
            throw new ArgumentException("Domain name is not valid.", nameof(domainName));
        }
        foreach (var label in labels)
        {
            if (label.IsNullOrWhiteSpace() || label.Length > 63)
            {
                throw new ArgumentException("Domain name must be between 1 and 63 characters.", nameof(domainName));
            }
                
            if (!char.IsLetterOrDigit(label[0]) || !char.IsLetterOrDigit(label[label.Length - 1]))
            {
                throw new ArgumentException("Domain name starts or ends with a hyphen.", nameof(domainName));
            }
                
            for (var i = 0; i < label.Length; i++)
            {
                var c = label[i];

                if (!char.IsLetterOrDigit(c) && c != '-')
                {
                    throw new ArgumentException("Domain name contains an invalid character.", nameof(domainName));
                }

                if (c != '-')
                {
                    continue;
                }
                    
                if (i + 1 < label.Length && (c ^ label[i+1]) == 0)
                {
                    throw new ArgumentException("Domain name contains consecutive hyphens.", nameof(domainName));
                }
            }
        }
    }
    
    private static bool IsSchemaHttpOrHttps(string scheme)
    {
        return  scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeHttps;
    }
}