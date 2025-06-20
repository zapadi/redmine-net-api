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
using System.Collections.Specialized;
using System.Net;
using Redmine.Net.Api.Common;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Http.Clients.WebClient;
using Redmine.Net.Api.Http.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Logging;
#if NET40_OR_GREATER || NET
using Redmine.Net.Api.Http.Clients.HttpClient;
#endif
using Redmine.Net.Api.Net.Internal;
using Redmine.Net.Api.Options;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
{
    /// <summary>
    ///     The main class to access Redmine API.
    /// </summary>
    public partial class RedmineManager : IRedmineManager
    {
        private readonly RedmineManagerOptions _redmineManagerOptions;

        internal IRedmineSerializer Serializer { get; }
        internal RedmineApiUrls RedmineApiUrls { get; }
        internal IRedmineApiClient ApiClient { get; }
        internal IRedmineLogger Logger { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RedmineManager(RedmineManagerOptionsBuilder optionsBuilder)
        {
            ArgumentNullThrowHelper.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));
            
            _redmineManagerOptions = optionsBuilder.Build();
   
            Logger = _redmineManagerOptions.Logger;
            Serializer = _redmineManagerOptions.Serializer;
            RedmineApiUrls = new RedmineApiUrls(_redmineManagerOptions.Serializer.Format);
           
            ApiClient =
#if NET40_OR_GREATER || NET
             _redmineManagerOptions.ApiClientOptions switch
            {
                RedmineWebClientOptions => CreateWebClient(_redmineManagerOptions),
                RedmineHttpClientOptions => CreateHttpClient(_redmineManagerOptions),
            };
#else
            CreateWebClient(_redmineManagerOptions);
#endif
        }

        private static InternalRedmineApiWebClient CreateWebClient(RedmineManagerOptions options)
        {
            if (options.ClientFunc != null)
            {
                return new InternalRedmineApiWebClient(options.ClientFunc, options);
            }

            ApplyServiceManagerSettings(options.WebClientOptions);
#pragma warning disable SYSLIB0014
            options.WebClientOptions.SecurityProtocolType ??= ServicePointManager.SecurityProtocol;
#pragma warning restore SYSLIB0014

            return new InternalRedmineApiWebClient(options);
        }
#if NET40_OR_GREATER || NET
        private InternalRedmineApiHttpClient CreateHttpClient(RedmineManagerOptions options)
        {
            return options.HttpClient != null 
                ? new InternalRedmineApiHttpClient(options.HttpClient, options) 
                : new InternalRedmineApiHttpClient(_redmineManagerOptions);
        }
#endif

        private static void ApplyServiceManagerSettings(RedmineWebClientOptions options)
        {
            if (options == null)
            {
                return;
            }
        
            if (options.SecurityProtocolType.HasValue)
            {
                ServicePointManager.SecurityProtocol = options.SecurityProtocolType.Value;
            }
        
            if (options.DefaultConnectionLimit.HasValue)
            {
                ServicePointManager.DefaultConnectionLimit = options.DefaultConnectionLimit.Value;
            }
        
            if (options.DnsRefreshTimeout.HasValue)
            {
                ServicePointManager.DnsRefreshTimeout = options.DnsRefreshTimeout.Value;
            }
        
            if (options.EnableDnsRoundRobin.HasValue)
            {
                ServicePointManager.EnableDnsRoundRobin = options.EnableDnsRoundRobin.Value;
            }
        
            if (options.MaxServicePoints.HasValue)
            {
                ServicePointManager.MaxServicePoints = options.MaxServicePoints.Value;
            }
        
            if (options.MaxServicePointIdleTime.HasValue)
            {
                ServicePointManager.MaxServicePointIdleTime = options.MaxServicePointIdleTime.Value;
            }
        
#if(NET46_OR_GREATER || NET)
            if (options.ReusePort.HasValue)
            {
                ServicePointManager.ReusePort = options.ReusePort.Value;
            }
#endif
        #if NEFRAMEWORK
            if (options.CheckCertificateRevocationList)
            {
                ServicePointManager.CheckCertificateRevocationList = true;
            }
#endif
        }
        
        /// <inheritdoc />
        public int Count<T>(RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var totalCount = 0;
            const int PAGE_SIZE = 1;
            const int OFFSET = 0;

            requestOptions ??= new RequestOptions();
            
            requestOptions.QueryString = requestOptions.QueryString.AddPagingParameters(PAGE_SIZE, OFFSET);
            
            var tempResult = GetPaginated<T>(requestOptions);

            if (tempResult != null)
            {
                totalCount = tempResult.TotalItems;
            }

            return totalCount;
        }

        /// <inheritdoc />
        public T Get<T>(string id, RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var url = RedmineApiUrls.GetFragment<T>(id);

            var response = ApiClient.Get(url, requestOptions);
            
            return response.DeserializeTo<T>(Serializer);
        }

        /// <inheritdoc />
        public List<T> Get<T>(RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var uri = RedmineApiUrls.GetListFragment<T>(requestOptions);
            
            return GetInternal<T>(uri, requestOptions);
        }

        /// <inheritdoc />
        public PagedResults<T> GetPaginated<T>(RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var url = RedmineApiUrls.GetListFragment<T>(requestOptions);

            return GetPaginatedInternal<T>(url, requestOptions);
        }

        /// <inheritdoc />
        public T Create<T>(T entity, string ownerId = null, RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var url = RedmineApiUrls.CreateEntityFragment<T>(ownerId);

            var payload = Serializer.Serialize(entity);
            
            var response = ApiClient.Create(url, payload, requestOptions);

            return response.DeserializeTo<T>(Serializer);
        }

        /// <inheritdoc />
        public void Update<T>(string id, T entity, string projectId = null, RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var url = RedmineApiUrls.UpdateFragment<T>(id);

            var payload = Serializer.Serialize(entity);
            
            payload = payload.ReplaceEndings();
            
            ApiClient.Update(url, payload, requestOptions);
        }

        /// <inheritdoc />
        public void Delete<T>(string id, RequestOptions requestOptions = null) 
            where T : class, new()
        {
            var url = RedmineApiUrls.DeleteFragment<T>(id);

            ApiClient.Delete(url, requestOptions);
        }

        /// <inheritdoc />
        public Upload UploadFile(byte[] data, string fileName = null)
        {
            var url = RedmineApiUrls.UploadFragment(fileName);

            var response = ApiClient.Upload(url, data);
            
            return response.DeserializeTo<Upload>(Serializer);
        }
        
        /// <inheritdoc />
        public byte[] DownloadFile(string address, IProgress<int> progress = null)
        {
            var response = ApiClient.Download(address, progress: progress);
            
            return response.Content;
        }
        
         /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal List<T> GetInternal<T>(string uri, RequestOptions requestOptions = null) 
             where T : class, new()
        {
            int pageSize = 0, offset = 0;
            var isLimitSet = false;
            List<T> resultList = null;

            requestOptions ??= new RequestOptions();
            
            if (requestOptions.QueryString == null)
            {
                requestOptions.QueryString = new NameValueCollection();
            }
            else
            {
                isLimitSet = int.TryParse(requestOptions.QueryString[RedmineKeys.LIMIT], out pageSize);
                int.TryParse(requestOptions.QueryString[RedmineKeys.OFFSET], out offset);
            }
            
            if (pageSize == default)
            {
                pageSize = _redmineManagerOptions.PageSize > 0 ? _redmineManagerOptions.PageSize : RedmineConstants.DEFAULT_PAGE_SIZE_VALUE;
                requestOptions.QueryString.Set(RedmineKeys.LIMIT, pageSize.ToInvariantString());
            }
            
            var hasOffset = TypesWithOffset.ContainsKey(typeof(T));
            if (hasOffset)
            {
                int totalCount;
                do
                {
                    requestOptions.QueryString.Set(RedmineKeys.OFFSET, offset.ToInvariantString());

                    var tempResult = GetPaginatedInternal<T>(uri, requestOptions);

                    totalCount = isLimitSet ? pageSize : tempResult.TotalItems;

                    if (tempResult?.Items != null)
                    {
                        if (resultList == null)
                        {
                            resultList = new List<T>(tempResult.Items);
                        }
                        else
                        {
                            resultList.AddRange(tempResult.Items);
                        }
                    }

                    offset += pageSize;
                }
                while (offset < totalCount);
            }
            else
            {
                var result = GetPaginatedInternal<T>(uri, requestOptions);
                if (result?.Items != null)
                {
                    return new List<T>(result.Items);
                }
            }
            
            return resultList;
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal PagedResults<T> GetPaginatedInternal<T>(string uri = null, RequestOptions requestOptions = null) 
            where T : class, new()
        {
            uri = uri.IsNullOrWhiteSpace() ? RedmineApiUrls.GetListFragment<T>(requestOptions) : uri;
            
            var response= ApiClient.Get(uri, requestOptions);
            
            return response.DeserializeToPagedResults<T>(Serializer);
        }
        
        internal static readonly Dictionary<Type, bool> TypesWithOffset = new Dictionary<Type, bool>{
            {typeof(Issue), true},
            {typeof(Project), true},
            {typeof(User), true},
            {typeof(News), true},
            {typeof(Query), true},
            {typeof(TimeEntry), true},
            {typeof(ProjectMembership), true},
            {typeof(Search), true}
        };
    }
}