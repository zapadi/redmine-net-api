/*
Copyright 2011 - 2016 Adrian Popescu.

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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    public static class RedmineManagerAsync
    {
        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<User> GetCurrentUserAsync(this RedmineManager redmineManager,
            NameValueCollection parameters = null)
        {
            var uri = string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host,
                RedmineManager.Sufixes[typeof(User)], RedmineManager.CURRENT_USER_URI, redmineManager.MimeFormat.ToString().ToLower());

            return await WebApiAsyncHelper.ExecuteDownload<User>(redmineManager, uri, "GetCurrentUserAsync", parameters);
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        public static async Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager,
            string projectId, string pageName, WikiPage wikiPage)
        {
            var uri = UrlHelper.GetWikiCreateOrUpdaterUrl(redmineManager, projectId, pageName);
            var data = RedmineSerializer.Serialize(wikiPage, redmineManager.MimeFormat);

            return await WebApiAsyncHelper.ExecuteUpload<WikiPage>(redmineManager, uri, HttpVerbs.PUT, data, "CreateOrUpdateWikiPageAsync");
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public static async Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId,
            string pageName)
        {
            var uri = UrlHelper.GetDeleteWikirUrl(redmineManager, projectId, pageName);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty, "DeleteWikiPageAsync");
        }

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>.</returns>
        public static async Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            var uri = UrlHelper.GetUploadFileUrl(redmineManager);

            return await WebApiAsyncHelper.ExecuteUploadFile(redmineManager, uri, data, "UploadFileAsync");
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            return await WebApiAsyncHelper.ExecuteDownloadFile(redmineManager, address, "DownloadFileAsync");
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="parameters"></param>
        /// <param name="pageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static async Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId,
            NameValueCollection parameters, string pageName, uint version = 0)
        {
            var uri = UrlHelper.GetWikiPageUrl(redmineManager, projectId, parameters, pageName, version);

            return await WebApiAsyncHelper.ExecuteDownload<WikiPage>(redmineManager, uri, "GetWikiPageAsync", parameters);
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static async Task<List<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager,
            NameValueCollection parameters, string projectId)
        {
            var uri = UrlHelper.GetWikisUrl(redmineManager, projectId);

            return await WebApiAsyncHelper.ExecuteDownloadList<WikiPage>(redmineManager, uri, "GetAllWikiPagesAsync", parameters);
        }

        /// <summary>
        ///     Adds an existing user to a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the Guid associated with the async request.</returns>
        public static async Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var data = DataHelper.UserData(userId, redmineManager.MimeFormat);

            var uri = UrlHelper.GetAddUserToGroupUrl(redmineManager, groupId);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.POST, data, "AddUserToGroupAsync");
        }

        /// <summary>
        ///     Removes an user from a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static async Task DeleteUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var uri = UrlHelper.GetRemoveUserFromGroupUrl(redmineManager, groupId, userId);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty, "DeleteUserFromGroupAsync");
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task AddWatcherAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var data = DataHelper.UserData(userId, redmineManager.MimeFormat);

            var uri = UrlHelper.GetAddWatcherUrl(redmineManager, issueId, userId);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.POST, data, "AddWatcherAsync");
        }

        /// <summary>
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task RemoveWatcherAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var uri = UrlHelper.GetRemoveWatcherUrl(redmineManager, issueId, userId);

            await WebApiAsyncHelper.ExecuteUpload(redmineManager, uri, HttpVerbs.DELETE, string.Empty, "RemoveWatcherAsync");
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            var uri = UrlHelper.GetListUrl<T>(redmineManager, parameters);

            return await WebApiAsyncHelper.ExecuteDownloadPaginatedList<T>(redmineManager, uri, "GetPaginatedObjectsAsync", parameters);
        }

        public static async Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            int totalCount = 0, pageSize, offset;
            List<T> resultList = null;

            if (parameters == null) parameters = new NameValueCollection();

            int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
            int.TryParse(parameters[RedmineKeys.OFFSET], out offset);
            if (pageSize == default(int))
            {
                pageSize = redmineManager.PageSize > 0
                    ? redmineManager.PageSize
                    : RedmineManager.DEFAULT_PAGE_SIZE_VALUE;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }
            try
            {
                do
                {
                    parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));
                    var tempResult = await redmineManager.GetPaginatedObjectsAsync<T>(parameters);
                    if (tempResult != null)
                    {
                        if (resultList == null)
                        {
                            resultList = tempResult.Objects;
                            totalCount = tempResult.TotalCount;
                        }
                        else
                        {
                            resultList.AddRange(tempResult.Objects);
                        }
                    }
                    offset += pageSize;
                } while (offset < totalCount);
            }
            catch (WebException wex)
            {
                wex.HandleWebException("GetObjectsAsync", redmineManager.MimeFormat);
            }
            return resultList;
        }

        /// <summary>
        ///     Gets a Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <param name="redmineManager"></param>
        /// <returns></returns>
        public static async Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters)
            where T : class, new()
        {
            var uri = UrlHelper.GetGetUrl<T>(redmineManager, id);

            return await WebApiAsyncHelper.ExecuteDownload<T>(redmineManager, uri, "GetobjectAsync", parameters);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="obj">The object to create.</param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj)
            where T : class, new()
        {
            return await CreateObjectAsync(redmineManager, obj, null);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="obj">The object to create.</param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj, string ownerId)
            where T : class, new()
        {
            var uri = UrlHelper.GetCreateUrl<T>(redmineManager, ownerId);
            var data = RedmineSerializer.Serialize(obj, redmineManager.MimeFormat);

            return await WebApiAsyncHelper.ExecuteUpload<T>(redmineManager, uri, HttpVerbs.POST, data, "CreateObjectAsync");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <param name="projectId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj, string projectId = null)
            where T : class, new()
        {
            var uri = UrlHelper.GetUploadUrl(redmineManager, id, obj, projectId);
            var data = RedmineSerializer.Serialize(obj, redmineManager.MimeFormat);
            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");

            await WebApiAsyncHelper.ExecuteUpload<T>(redmineManager, uri, HttpVerbs.PUT, data, "UpdateObjectAsync");
        }

        /// <summary>
        ///     Deletes the Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns></returns>
        public static async Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters)
            where T : class, new()
        {
            var uri = UrlHelper.GetDeleteUrl<T>(redmineManager, id);

            await WebApiAsyncHelper.ExecuteUpload<T>(redmineManager, uri, HttpVerbs.DELETE, string.Empty, "DeleteObjectAsync");
        }
    }
}