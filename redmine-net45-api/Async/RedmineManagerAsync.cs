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
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    public static class RedmineManagerAsync
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, NameValueCollection parameters = null)
        {
            var uri = string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[typeof(User)], RedmineManager.CURRENT_USER_URI, redmineManager.MimeFormat);
            using (var wc = redmineManager.CreateWebClient(null))
            {
                var result = await wc.DownloadStringTaskAsync(uri).ConfigureAwait(false);
                return RedmineSerializer.Deserialize<User>(result, redmineManager.MimeFormat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        public static async Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
        {
            var uri = UrlHelper.GetWikiCreateOrUpdaterUrl(redmineManager, projectId, pageName);
            var data = RedmineSerializer.Serialize(wikiPage, redmineManager.MimeFormat);

            using (var wc = redmineManager.CreateWebClient(null))
            {
                var response = await wc.UploadStringTaskAsync(uri, RedmineManager.PUT, data).ConfigureAwait(false);
                return RedmineSerializer.Deserialize<WikiPage>(response, redmineManager.MimeFormat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public static async Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            var uri = UrlHelper.GetDeleteWikirUrl(redmineManager, projectId, pageName);
            using (var wc = redmineManager.CreateWebClient(null))
            {
                await wc.UploadStringTaskAsync(uri, RedmineManager.DELETE, string.Empty).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Support for adding attachments through the REST API is added in Redmine 1.4.0.
        /// Upload a file to server. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>.</returns>
        public static async Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            var uri = UrlHelper.GetUploadFileUrl(redmineManager);
            using (var wc = redmineManager.CreateWebClient(null))
            {
                var response = await wc.UploadDataTaskAsync(uri, RedmineManager.POST, data).ConfigureAwait(false);
                var responseString = Encoding.ASCII.GetString(response);
                return RedmineSerializer.Deserialize<Upload>(responseString, redmineManager.MimeFormat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            using (var wc = redmineManager.CreateWebClient(null))
            {
                wc.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
                var response = await wc.DownloadDataTaskAsync(address).ConfigureAwait(false);
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="parameters"></param>
        /// <param name="pageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static async Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            var uri = UrlHelper.GetWikiPageUrl(redmineManager, projectId, parameters, pageName, version);
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                var response = await wc.DownloadStringTaskAsync(uri).ConfigureAwait(false);
                return RedmineSerializer.Deserialize<WikiPage>(response, redmineManager.MimeFormat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static async Task<List<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, NameValueCollection parameters, string projectId)
        {
            var uri = UrlHelper.GetWikisUrl(redmineManager, projectId);
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                var response = await wc.DownloadStringTaskAsync(uri).ConfigureAwait(false);
                var result = RedmineSerializer.DeserializeList<WikiPage>(response, redmineManager.MimeFormat);
                return result.Objects;
            }
        }

        /// <summary>
        /// Adds an existing user to a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the Guid associated with the async request.</returns>
        public static async Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var data = redmineManager.MimeFormat == MimeFormat.xml ? "<user_id>" + userId + "</user_id>" : "{\"user_id\":\"" + userId + "\"}";
            var uri = UrlHelper.GetAddUserToGroupUrl(redmineManager, groupId);
            try
            {
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    await wc.UploadStringTaskAsync(uri, RedmineManager.POST, data).ConfigureAwait(false);
                }
            }
            catch (WebException wex)
            {

            }
        }

        /// <summary>
        /// Removes an user from a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static async Task DeleteUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var uri = UrlHelper.GetRemoveUserFromGroupUrl(redmineManager, groupId, userId);
            using (var wc = redmineManager.CreateWebClient(null))
            {
                await wc.UploadStringTaskAsync(uri, RedmineManager.DELETE, string.Empty).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task AddWatcherAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var data = redmineManager.MimeFormat == MimeFormat.xml ? "<user_id>" + userId + "</user_id>" : "{\"user_id\":\"" + userId + "\"}";
            var uri = UrlHelper.GetAddWatcherUrl(redmineManager, issueId, userId);

            using (var wc = redmineManager.CreateWebClient(null))
            {
                await wc.UploadStringTaskAsync(uri, RedmineManager.POST, data).ConfigureAwait(false);
            }
        }

        public static async Task RemoveWatcherAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var uri = UrlHelper.GetRemoveWatcherUrl(redmineManager, issueId, userId);
            using (var wc = redmineManager.CreateWebClient(null))
            {
                await wc.UploadStringTaskAsync(uri, RedmineManager.DELETE, string.Empty).ConfigureAwait(false);
            }
        }

        public static async Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            var url = UrlHelper.GetListUrl<T>(redmineManager, parameters);
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                var response = await wc.DownloadStringTaskAsync(url).ConfigureAwait(false);
                return RedmineSerializer.DeserializeList<T>(response, redmineManager.MimeFormat);
            }
        }

        public static async Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            int totalCount = 0, pageSize;
            List<T> resultList = null;
            if (parameters == null) parameters = new NameValueCollection();
            int offset = 0;
            int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
            if (pageSize == default(int))
            {
                pageSize = redmineManager.PageSize > 0 ? redmineManager.PageSize : 25;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }
            do
            {
                parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));
                var tempResult = await redmineManager.GetPaginatedObjectsAsync<T>(parameters);
                if (tempResult != null)
                {
                    if (resultList == null)
                        resultList = tempResult.Objects;
                    else
                        resultList.AddRange(tempResult.Objects);
                }
                offset += pageSize;
            }
            while (offset < totalCount);
            return resultList;
        }

        /// <summary>
        /// Gets a Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <param name="redmineManager"></param>
        /// <returns></returns>
        public static async Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters) where T : class, new()
        {
            var url = UrlHelper.GetGetUrl<T>(redmineManager, id);
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                var response = await wc.DownloadStringTaskAsync(url).ConfigureAwait(false);
                return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
            }
        }

        /// <summary>
        /// Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="obj">The object to create.</param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj) where T : class, new()
        {
            return await CreateObjectAsync(redmineManager, obj, null);
        }

        ///  <summary>
        ///  Creates a new Redmine object. This method does not block the calling thread.
        ///  </summary>
        ///  <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="obj">The object to create.</param>
        /// <param name="ownerId"></param>
        ///  <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj, string ownerId) where T : class, new()
        {
            var url = UrlHelper.GetCreateUrl<T>(redmineManager, ownerId);
            var data = RedmineSerializer.Serialize(obj, redmineManager.MimeFormat);

            using (var wc = redmineManager.CreateWebClient(null))
            {
                var response = await wc.UploadStringTaskAsync(url, RedmineManager.POST, data).ConfigureAwait(false);
                return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
            }
        }

        public static async Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj, string projectId = null) where T : class, new()
        {
            var url = UrlHelper.GetUploadUrl(redmineManager, id, obj, projectId);
            using (var wc = redmineManager.CreateWebClient(null))
            {
                var data = RedmineSerializer.Serialize(obj, redmineManager.MimeFormat);
                await wc.UploadStringTaskAsync(url, RedmineManager.PUT, data).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns></returns>
        public static async Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters) where T : class, new()
        {
            var uri = UrlHelper.GetDeleteUrl<T>(redmineManager, id);

            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                await wc.UploadStringTaskAsync(uri, RedmineManager.DELETE, string.Empty).ConfigureAwait(false);
            }
        }
    }
}