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

#if !(NET20)
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api;

public partial class RedmineManager: IRedmineManagerAsync
{
    /// <inheritdoc />
    public async Task<int> CountAsync<T>(RequestOptions requestOptions, CancellationToken cancellationToken = default) where T : class, new()
    {
        const int PAGE_SIZE = 1;
        const int OFFSET = 0;
        var totalCount = 0;

        requestOptions ??= new RequestOptions();

        requestOptions.QueryString.AddPagingParameters(PAGE_SIZE, OFFSET);

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
            requestOptions.QueryString.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
        }

        try
        {
            var hasOffset = RedmineManager.TypesWithOffset.ContainsKey(typeof(T));
            if (hasOffset)
            {
                int totalCount;
                do
                {
                    requestOptions.QueryString.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

                    var tempResult = await GetPagedAsync<T>(requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);

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
                } while (offset < totalCount);
            }
            else
            {
                var result = await GetPagedAsync<T>(requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result?.Items != null)
                {
                    return new List<T>(result.Items);
                }
            }
        }
        catch (WebException wex)
        {
            wex.HandleWebException(Serializer);
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

        // payload = Regex.Replace(payload, @"\r\n|\r|\n", "\r\n");
        
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
    public async Task<Upload> UploadFileAsync(byte[] data, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        var url = RedmineApiUrls.UploadFragment();

        var response = await ApiClient.UploadFileAsync(url, data,requestOptions  , cancellationToken: cancellationToken).ConfigureAwait(false);
            
        return response.DeserializeTo<Upload>(Serializer);
    }

    /// <inheritdoc />
    public async Task<byte[]> DownloadFileAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        var response = await ApiClient.DownloadAsync(address, requestOptions,cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.Content;
    }
}
#endif