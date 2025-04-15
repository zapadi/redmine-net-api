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

#if !(NET20)
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using TaskExtensions = Redmine.Net.Api.Extensions.TaskExtensions;

namespace Redmine.Net.Api;

public partial class RedmineManager: IRedmineManagerAsync
{
    
    
    /// <inheritdoc />
    public async Task<int> CountAsync<T>(RequestOptions requestOptions, CancellationToken cancellationToken = default) 
        where T : class, new()
    {
        var totalCount = 0;

        requestOptions ??= new RequestOptions();

        requestOptions.QueryString.AddPagingParameters(pageSize: 1, offset: 0);

        var tempResult = await GetPagedAsync<T>(requestOptions, cancellationToken).ConfigureAwait(false);
        if (tempResult != null)
        {
            totalCount = tempResult.TotalItems;
        }

        return totalCount;
    }

    /// <inheritdoc />
    public async Task<PagedResults<T>> GetPagedAsync<T>(RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var url = RedmineApiUrls.GetListFragment<T>();

        var response = await ApiClient.GetAsync(url, requestOptions, cancellationToken).ConfigureAwait(false);

        return response.DeserializeToPagedResults<T>(Serializer);
    }
    
    
    /// <inheritdoc />
    public async Task<List<T>> GetAsync<T>(RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
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
            pageSize = PageSize > 0
                ? PageSize
                : RedmineConstants.DEFAULT_PAGE_SIZE_VALUE;
            requestOptions.QueryString.Set(RedmineKeys.LIMIT, pageSize.ToInvariantString());
        }
        
        var hasOffset = RedmineManager.TypesWithOffset.ContainsKey(typeof(T));
        if (hasOffset)
        {
            requestOptions.QueryString.Set(RedmineKeys.OFFSET, offset.ToInvariantString());

            var tempResult = await GetPagedAsync<T>(requestOptions, cancellationToken).ConfigureAwait(false);

            var totalCount = isLimitSet ? pageSize : tempResult.TotalItems;
            
            if (tempResult?.Items != null)
            {
                resultList = new List<T>(tempResult.Items);
            }
           
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var remainingPages = totalPages - offset / pageSize;

            if (remainingPages <= 0)
            {
                return resultList;
            }
            
            using (var semaphore = new SemaphoreSlim(MAX_CONCURRENT_TASKS))
            {
                var pageFetchTasks = new List<Task<PagedResults<T>>>();

                for (int page = 0; page < remainingPages; page++)
                {
                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    var innerOffset = (page * pageSize) + offset;
                    
                    pageFetchTasks.Add(GetPagedInternalAsync<T>(semaphore, new RequestOptions()
                    {
                        QueryString = new NameValueCollection()
                        {
                            {RedmineKeys.OFFSET, innerOffset.ToInvariantString()},
                            {RedmineKeys.LIMIT, pageSize.ToInvariantString()}
                        }
                    }, cancellationToken));
                }

                var pageResults = await
                    #if(NET45_OR_GREATER || NETCOREAPP)
                    Task.WhenAll(pageFetchTasks)
                    #else
                    TaskExtensions.WhenAll(pageFetchTasks)
                    #endif
                    .ConfigureAwait(false);
                
                foreach (var pageResult in pageResults)
                {
                    if (pageResult?.Items == null)
                    {
                        continue;
                    }
                    
                    resultList ??= new List<T>();

                    resultList.AddRange(pageResult.Items);
                }
            }
        }
        else
        {
            var result = await GetPagedAsync<T>(requestOptions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (result?.Items != null)
            {
                return new List<T>(result.Items);
            }
        }
        
        return resultList;
    }

    /// <inheritdoc />
    public async Task<T> GetAsync<T>(string id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var url = RedmineApiUrls.GetFragment<T>(id);

        var response = await ApiClient.GetAsync(url, requestOptions, cancellationToken).ConfigureAwait(false);

        return response.DeserializeTo<T>(Serializer);
    }
    
    /// <inheritdoc />
    public async Task<T> CreateAsync<T>(T entity, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        return await CreateAsync(entity, null, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<T> CreateAsync<T>(T entity, string ownerId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var url = RedmineApiUrls.CreateEntityFragment<T>(ownerId);

        var payload = Serializer.Serialize(entity);

        var response = await ApiClient.CreateAsync(url, payload, requestOptions, cancellationToken).ConfigureAwait(false);

        return response.DeserializeTo<T>(Serializer);
    }

    /// <inheritdoc />
    public async Task UpdateAsync<T>(string id, T entity, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var url = RedmineApiUrls.UpdateFragment<T>(id);

        var payload = Serializer.Serialize(entity);
        
        payload = payload.ReplaceEndings();
        
        await ApiClient.UpdateAsync(url, payload, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync<T>(string id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var url = RedmineApiUrls.DeleteFragment<T>(id);

        await ApiClient.DeleteAsync(url, requestOptions, cancellationToken).ConfigureAwait((false));
    }
    
    /// <inheritdoc />
    public async Task<Upload> UploadFileAsync(byte[] data, string fileName = null, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        var url = RedmineApiUrls.UploadFragment(fileName);

        var response = await ApiClient.UploadFileAsync(url, data,requestOptions  , cancellationToken: cancellationToken).ConfigureAwait(false);
            
        return response.DeserializeTo<Upload>(Serializer);
    }

    /// <inheritdoc />
    public async Task<byte[]> DownloadFileAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        var response = await ApiClient.DownloadAsync(address, requestOptions,cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.Content;
    }
    
    private const int MAX_CONCURRENT_TASKS = 3;
    
    private async Task<PagedResults<T>> GetPagedInternalAsync<T>(SemaphoreSlim semaphore, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        try
        {
            var url = RedmineApiUrls.GetListFragment<T>();

            var response = await ApiClient.GetAsync(url, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeToPagedResults<T>(Serializer);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
#endif