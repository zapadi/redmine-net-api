/*
Copyright 2011 - 2022 Adrian Popescu

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

#if !(NET20 || NET40)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    /// <summary>
    /// </summary>
    public static class RedmineManagerAsync
    {
        /// <summary>
        ///     Gets the current user asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static async Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, NameValueCollection parameters = null)
        {
            var uri = UrlHelper.GetCurrentUserUrl(redmineManager);
            return await WebApiAsyncHelper.ExecuteDownload<User>(redmineManager, uri, parameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <returns></returns>
        public static async Task<WikiPage> CreateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
        {
            var data = redmineManager.Serializer.Serialize(wikiPage);
            if (string.IsNullOrEmpty(data)) return null;

            var url = UrlHelper.GetWikiCreateOrUpdaterUrl(redmineManager, projectId, pageName);

            url = Uri.EscapeUriString(url);

            var response = await WebApiAsyncHelper.ExecuteUpload(redmineManager, url, HttpVerbs.PUT, data).ConfigureAwait(false);
            return redmineManager.Serializer.Deserialize<WikiPage>(response);
        }

 /// <summary>
        ///     Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <returns></returns>
        public static async Task UpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
        {
            var data = redmineManager.Serializer.Serialize(wikiPage);
            if (string.IsNullOrEmpty(data))
            {
                return ;
            }

            var url = UrlHelper.GetWikiCreateOrUpdaterUrl(redmineManager, projectId, pageName);

            url = Uri.EscapeUriString(url);

            var response = await WebApiAsyncHelper.ExecuteUpload(redmineManager, url, HttpVerbs.PUT, data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Deletes the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static async Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId,
            string pageName)
        {
            var uri = UrlHelper.GetDeleteWikiUrl(redmineManager, projectId, pageName);
            uri = Uri.EscapeUriString(uri);
            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>
        ///     .
        /// </returns>
        public static async Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            var uri = UrlHelper.GetUploadFileUrl(redmineManager);
            return await WebApiAsyncHelper.ExecuteUploadFile(redmineManager, uri, data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Downloads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            return await WebApiAsyncHelper.ExecuteDownloadFile(redmineManager, address).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static async Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId,
            NameValueCollection parameters, string pageName, uint version = 0)
        {
            var uri = UrlHelper.GetWikiPageUrl(redmineManager, projectId,  pageName, version);
            uri = Uri.EscapeUriString(uri);
            return await WebApiAsyncHelper.ExecuteDownload<WikiPage>(redmineManager, uri, parameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets all wiki pages asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static async Task<List<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, NameValueCollection parameters, string projectId)
        {
            var uri = UrlHelper.GetWikisUrl(redmineManager, projectId);
            return await WebApiAsyncHelper.ExecuteDownloadList<WikiPage>(redmineManager, uri, parameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds an existing user to a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>
        ///     Returns the Guid associated with the async request.
        /// </returns>
        public static async Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var data = DataHelper.UserData(userId, redmineManager.MimeFormat);
            var uri = UrlHelper.GetAddUserToGroupUrl(redmineManager, groupId);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.POST, data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes an user from a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static async Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var uri = UrlHelper.GetRemoveUserFromGroupUrl(redmineManager, groupId, userId);
            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds the watcher asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static async Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var data = DataHelper.UserData(userId, redmineManager.MimeFormat);
            var uri = UrlHelper.GetAddWatcherUrl(redmineManager, issueId);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.POST, data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes the watcher asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static async Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var uri = UrlHelper.GetRemoveWatcherUrl(redmineManager, issueId, userId);
            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this RedmineManager redmineManager, params string[] include) where T : class, new()
        {
            var parameters = new NameValueCollection();

            if (include != null)
            {
                parameters.Add(RedmineKeys.INCLUDE, string.Join(",", include));
            }

            return await CountAsync<T>(redmineManager,parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            int totalCount = 0, pageSize = 1, offset = 0;

            if (parameters == null)
            {
                parameters = new NameValueCollection();
            }

            parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

            try
            {
                var tempResult = await GetPaginatedObjectsAsync<T>(redmineManager,parameters).ConfigureAwait(false);
                if (tempResult != null)
                {
                    totalCount = tempResult.TotalItems;
                }
            }
            catch (WebException wex)
            {
                wex.HandleWebException(redmineManager.Serializer);
            }

            return totalCount;
        }


        /// <summary>
        ///     Gets the paginated objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static async Task<PagedResults<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager,
            NameValueCollection parameters)
            where T : class, new()
        {
            var uri = UrlHelper.GetListUrl<T>(redmineManager, parameters);
            return await WebApiAsyncHelper.ExecuteDownloadPaginatedList<T>(redmineManager, uri, parameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static async Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            int  pageSize = 0, offset = 0;
            var isLimitSet = false;
            List<T> resultList = null;

            if (parameters == null)
            {
                parameters = new NameValueCollection();
            }
            else
            {
                isLimitSet = int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
                int.TryParse(parameters[RedmineKeys.OFFSET], out offset);
            }
            
            if (pageSize == default(int))
            {
                pageSize = redmineManager.PageSize > 0
                    ? redmineManager.PageSize
                    : RedmineManager.DEFAULT_PAGE_SIZE_VALUE;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }
            try
            {
                var hasOffset = RedmineManager.TypesWithOffset.ContainsKey(typeof(T));
                if (hasOffset)
                {
                    var totalCount = 0;
                do
                {
                    parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));
                    var tempResult = await redmineManager.GetPaginatedObjectsAsync<T>(parameters).ConfigureAwait(false);
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
                    var result = await redmineManager.GetPaginatedObjectsAsync<T>(parameters).ConfigureAwait(false);
                    if (result?.Items != null)
                    {
                        return new List<T>(result.Items);
                    }
                }
            }
            catch (WebException wex)
            {
                wex.HandleWebException(redmineManager.Serializer);
            }
            return resultList;
        }

        /// <summary>
        ///     Gets a Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns></returns>
        public static async Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters)
            where T : class, new()
        {
            var uri = UrlHelper.GetGetUrl<T>(redmineManager, id);
            return await WebApiAsyncHelper.ExecuteDownload<T>(redmineManager, uri, parameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object to create.</param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity)
            where T : class, new()
        {
            return await CreateObjectAsync(redmineManager, entity, null).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object to create.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity, string ownerId)
            where T : class, new()
        {
            var uri = UrlHelper.GetCreateUrl<T>(redmineManager, ownerId);
            var data = redmineManager.Serializer.Serialize(entity);

            var response = await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.POST, data).ConfigureAwait(false);
            return redmineManager.Serializer.Deserialize<T>(response);
        }

        /// <summary>
        ///     Updates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The object.</param>
        /// <returns></returns>
        public static async Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T entity)
            where T : class, new()
        {
            var uri = UrlHelper.GetUploadUrl<T>(redmineManager, id);
            var data = redmineManager.Serializer.Serialize(entity);
            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.PUT, data).ConfigureAwait(false);
        }

        /// <summary>
        ///     Deletes the Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The id of the object to delete</param>
        /// <returns></returns>
        public static async Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id)
            where T : class, new()
        {
            var uri = UrlHelper.GetDeleteUrl<T>(redmineManager, id);
            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="q"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PagedResults<Search>> SearchAsync(this RedmineManager redmineManager, string q, int limit = RedmineManager.DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null)
        {
            if (q.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(q));
            }

            var parameters = new NameValueCollection
            {
                {RedmineKeys.Q, q},
                {RedmineKeys.LIMIT, limit.ToString(CultureInfo.InvariantCulture)},
                {RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture)},
            };

            if (searchFilter != null)
            {
               parameters = searchFilter.Build(parameters);
            }
            
            var result = await redmineManager.GetPaginatedObjectsAsync<Search>(parameters).ConfigureAwait(false);

            return result;
        }
    }
}
#endif