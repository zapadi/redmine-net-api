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
using Padi.RedmineApi.Builders;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Serialization;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi;

/// <summary>
///     The main class to access Redmine API.
/// </summary>
public 
#if !(NET20)
    partial
#endif
    class RedmineManager : IRedmineManager
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
        if (optionsBuilder == null)
        {
            throw new ArgumentNullException(nameof(optionsBuilder));
        }
        
        _redmineManagerOptions = optionsBuilder.Build();

        Serializer = _redmineManagerOptions.Serializer;
        RedmineApiUrls = new RedmineApiUrls(Serializer.Format);
        ApiClient = _redmineManagerOptions.ApiClient;
    }
    
    /// <inheritdoc />
    public int Count<T>(RequestOptions requestOptions = null) 
        where T : class, new()
    {
        var totalCount = 0;
        const int pageSize = 1;
        const int offset = 0;

        requestOptions ??= new RequestOptions();
            
        requestOptions.QueryString = requestOptions.QueryString.AddPagingParameters(pageSize, offset);
            
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
            
        return response.RawContent;
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
            
        if (pageSize == 0)
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

    private static readonly Dictionary<Type, bool> TypesWithOffset = new()
    {
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