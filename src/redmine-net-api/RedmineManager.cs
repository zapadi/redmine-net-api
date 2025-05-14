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
using System.Globalization;
using System.Net;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RedmineManager(RedmineManagerOptionsBuilder optionsBuilder)
        {
            ArgumentNullThrowHelper.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));
            
            _redmineManagerOptions = optionsBuilder.Build();
   
            Serializer = _redmineManagerOptions.Serializer;
            RedmineApiUrls = new RedmineApiUrls(_redmineManagerOptions.Serializer.Format);
            
            Host = _redmineManagerOptions.BaseAddress.ToString();
            PageSize = _redmineManagerOptions.PageSize;
            Scheme = _redmineManagerOptions.BaseAddress.Scheme;
            Format = Serializer.Format;
            MimeFormat = RedmineConstants.XML.Equals(Serializer.Format, StringComparison.Ordinal) 
                ? MimeFormat.Xml 
                : MimeFormat.Json;
            
            if (_redmineManagerOptions.Authentication is RedmineApiKeyAuthentication)
            {
                ApiKey = _redmineManagerOptions.Authentication.Token;
            }
            
            ApiClient =
#if NET45_OR_GREATER || NETCOREAPP
             _redmineManagerOptions.WebClientOptions switch
            {
                RedmineWebClientOptions => CreateWebClient(_redmineManagerOptions),
                _ => CreateHttpClient(_redmineManagerOptions)
            };
#else
            CreateWebClient(_redmineManagerOptions);
#endif
        }

        private InternalRedmineApiWebClient CreateWebClient(RedmineManagerOptions options)
        {
            if (options.ClientFunc != null)
            {
                return new InternalRedmineApiWebClient(options.ClientFunc, options.Authentication, options.Serializer);
            }

#pragma warning disable SYSLIB0014
            options.WebClientOptions.SecurityProtocolType ??= ServicePointManager.SecurityProtocol;
#pragma warning restore SYSLIB0014

            Proxy = options.WebClientOptions.Proxy;
            Timeout = options.WebClientOptions.Timeout;
            SecurityProtocolType = options.WebClientOptions.SecurityProtocolType.GetValueOrDefault();

#if NET45_OR_GREATER
            if (options.VerifyServerCert)
            {
                options.WebClientOptions.ServerCertificateValidationCallback = RemoteCertValidate;
            }
#endif
            return new InternalRedmineApiWebClient(options);
        }
        
        private IRedmineApiClient CreateHttpClient(RedmineManagerOptions options)
        {
            throw new NotImplementedException();
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
        public byte[] DownloadFile(string address)
        {
            var response = ApiClient.Download(address);
            
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
    }
}