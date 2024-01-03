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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.WebClient;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Group = Redmine.Net.Api.Types.Group;
using Version = Redmine.Net.Api.Types.Version;

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
            #if NET5_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(optionsBuilder);
            #else
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }
            #endif
            _redmineManagerOptions = optionsBuilder.Build();
            if (_redmineManagerOptions.VerifyServerCert)
            {
                _redmineManagerOptions.ClientOptions.ServerCertificateValidationCallback = RemoteCertValidate;
            }

            Serializer = _redmineManagerOptions.Serializer;
            
            Host = _redmineManagerOptions.BaseAddress.ToString();
            PageSize = _redmineManagerOptions.PageSize;
            Format = Serializer.Format;
            Scheme = _redmineManagerOptions.BaseAddress.Scheme;
            Proxy = _redmineManagerOptions.ClientOptions.Proxy;
            Timeout = _redmineManagerOptions.ClientOptions.Timeout;
            MimeFormat = "xml".Equals(Serializer.Format, StringComparison.OrdinalIgnoreCase) ? MimeFormat.Xml : MimeFormat.Json;
            
            _redmineManagerOptions.ClientOptions.SecurityProtocolType ??= ServicePointManager.SecurityProtocol;
                
            SecurityProtocolType = _redmineManagerOptions.ClientOptions.SecurityProtocolType.Value;
            
            if (_redmineManagerOptions.Authentication is RedmineApiKeyAuthentication)
            {
                ApiKey = _redmineManagerOptions.Authentication.Token;
            }
            
            RedmineApiUrls = new RedmineApiUrls(Serializer.Format);
            ApiClient = new RedmineApiClient(_redmineManagerOptions); 
        }
        
        /// <summary>
        ///     Maximum page-size when retrieving complete object lists
        ///     <remarks>
        ///         By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set
        ///         in your Settings -&gt; General, "Objects per page options".By adding (for instance) 9999 there would make you
        ///         able to get that many results per request.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     The size of the page.
        /// </value>
        public int PageSize { get; set; }
        
        /// <summary>
        ///     As of Redmine 2.2.0 you can impersonate user setting user login (eg. jsmith). This only works when using the API
        ///     with an administrator account, this header will be ignored when using the API with a regular user account.
        /// </summary>
        /// <value>
        ///     The impersonate user.
        /// </value>
        public string ImpersonateUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Count<T>(params string[] include) where T : class, new()
        {
            var parameters = NameValueCollectionExtensions.AddParamsIfExist(null, include);

            return Count<T>(parameters);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Count<T>(NameValueCollection parameters) where T : class, new()
        {
            var totalCount = 0;
            const int PAGE_SIZE = 1;
            const int OFFSET = 0;

            parameters.AddPagingParameters(PAGE_SIZE, OFFSET);
            
            var tempResult = GetPaginatedObjects<T>(parameters);

            if (tempResult != null)
            {
                totalCount = tempResult.TotalItems;
            }

            return totalCount;
        }

        /// <summary>
        ///     Gets the redmine object based on id.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>
        ///     Returns the object of type T.
        /// </returns>
        /// <code>
        ///   <example>
        ///         string issueId = "927";
        ///         NameValueCollection parameters = null;
        ///         Issue issue = redmineManager.GetObject&lt;Issue&gt;(issueId, parameters);
        ///   </example>
        /// </code>
        public T GetObject<T>(string id, NameValueCollection parameters) where T : class, new()
        {
            var url = RedmineApiUrls.GetFragment<T>(id);

            var response = ApiClient.Get(url, parameters != null ? new RequestOptions { QueryString = parameters } : null);
            
            return response.DeserializeTo<T>(Serializer);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="include">Optional fetched data.</param>
        /// <remarks>
        /// Optional fetched data:
        ///     Project: trackers, issue_categories, enabled_modules (since Redmine 2.6.0)
        ///     Issue: children, attachments, relations, changesets, journals, watchers (since Redmine 2.3.0)
        ///     Users: memberships, groups (since Redmine 2.1)
        ///     Groups: users, memberships
        /// </remarks>
        /// <returns>Returns the complete list of objects.</returns>
        public List<T> GetObjects<T>(params string[] include) where T : class, new()
        {
            var parameters = NameValueCollectionExtensions.AddParamsIfExist(null, include);

            return GetObjects<T>(parameters);
        }
        
        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">The page size.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="include">Optional fetched data.</param>
        /// <remarks>
        /// Optional fetched data:
        ///     Project: trackers, issue_categories, enabled_modules (since 2.6.0)
        ///     Issue: children, attachments, relations, changesets, journals, watchers - Since 2.3.0
        ///     Users: memberships, groups (added in 2.1)
        ///     Groups: users, memberships
        /// </remarks>
        /// <returns>Returns the complete list of objects.</returns>
        public List<T> GetObjects<T>(int limit, int offset, params string[] include) where T : class, new()
        {
            var parameters = NameValueCollectionExtensions
                .AddParamsIfExist(null, include)
                .AddPagingParameters(limit, offset);

            return GetObjects<T>(parameters);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>
        ///     Returns a complete list of objects.
        /// </returns>
        public List<T> GetObjects<T>(NameValueCollection parameters = null) where T : class, new()
        {
            var uri = RedmineApiUrls.GetListFragment<T>();
            
            return GetObjects<T>(uri, parameters != null ? new RequestOptions { QueryString = parameters } : null);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal List<T> GetObjects<T>(string uri, RequestOptions requestOptions = null) where T : class, new()
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
                requestOptions.QueryString.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }
            
            var hasOffset = TypesWithOffset.ContainsKey(typeof(T));
            if (hasOffset)
            {
                int totalCount;
                do
                {
                    requestOptions.QueryString.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

                    var tempResult = GetPaginatedObjects<T>(uri, requestOptions);

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
                var result = GetPaginatedObjects<T>(uri, requestOptions);
                if (result?.Items != null)
                {
                    return new List<T>(result.Items);
                }
            }
            
            return resultList;
        }
        
        /// <summary>
        ///     Gets the paginated objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public PagedResults<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            var url = RedmineApiUrls.GetListFragment<T>();

            return GetPaginatedObjects<T>(url, parameters != null ? new RequestOptions { QueryString = parameters } : null);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal PagedResults<T> GetPaginatedObjects<T>(string uri = null, RequestOptions requestOptions = null) where T : class, new()
        {
            uri = uri.IsNullOrWhiteSpace() ? RedmineApiUrls.GetListFragment<T>() : uri;
            
            var response= ApiClient.Get(uri, requestOptions);
            
            return response.DeserializeToPagedResults<T>(Serializer);
        }

        /// <summary>
        ///     Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="entity">The object to create.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable
        ///     Entity response. That means that the object could not be created.
        /// </remarks>
        public T CreateObject<T>(T entity) where T : class, new()
        {
            return CreateObject(entity, null);
        }

        /// <summary>
        ///     Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="entity">The object to create.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable
        ///     Entity response. That means that the object could not be created.
        /// </remarks>
        /// <code>
        ///   <example>
        ///         var project = new Project();
        ///         project.Name = "test";
        ///         project.Identifier = "the project identifier";
        ///         project.Description = "the project description";
        ///         redmineManager.CreateObject(project);
        ///     </example>
        /// </code>
        public T CreateObject<T>(T entity, string ownerId) where T : class, new()
        {
            var url = RedmineApiUrls.CreateEntityFragment<T>(ownerId);

            var payload = Serializer.Serialize(entity);
            
            var response = ApiClient.Create(url, payload);

            return response.DeserializeTo<T>(Serializer);
        }

        /// <summary>
        ///     Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="entity">The object to be update.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to update an object with invalid or missing attribute parameters, you will get a
        ///     422(RedmineException) Unprocessable Entity response. That means that the object could not be updated.
        /// </remarks>
        /// <code>
        /// </code>
        public void UpdateObject<T>(string id, T entity, string projectId = null) where T : class, new()
        {
            var url = RedmineApiUrls.UpdateFragment<T>(id);

            var payload = Serializer.Serialize(entity);
            
            ApiClient.Update(url, payload);
        }

        /// <summary>
        /// Deletes the Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="parameters">The parameters</param>
        /// <exception cref="RedmineException"></exception>
        /// <code></code>
        public void DeleteObject<T>(string id, NameValueCollection parameters = null) where T : class, new()
        {
            var url = RedmineApiUrls.DeleteFragment<T>(id);

            ApiClient.Delete(url, parameters != null ? new RequestOptions { QueryString = parameters } : null);
        }

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server.
        /// </summary>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>
        ///     Returns the token for uploaded file.
        /// </returns>
        /// <exception cref="RedmineException"></exception>
        public Upload UploadFile(byte[] data)
        {
            var url = RedmineApiUrls.UploadFragment();

            var response = ApiClient.Upload(url, data);
            
            return response.DeserializeTo<Upload>(Serializer);
        }

        /// <summary>
        ///     Downloads a file from the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>The content of the downloaded file as a byte array.</returns>
        /// <exception cref="RedmineException"></exception>
        public byte[] DownloadFile(string address)
        {
            var response = ApiClient.Download(address);
            return response.Content;
        }
    }
}