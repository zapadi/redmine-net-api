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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    /// <summary>
    /// </summary>
    [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Use RedmineManger async methods instead")]
    public static class RedmineManagerAsyncExtensions
    {
        /// <summary>
        ///     Gets the current user asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="impersonateUserName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, NameValueCollection parameters = null, string impersonateUserName = null, CancellationToken cancellationToken = default)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            return await redmineManager.GetCurrentUserAsync(requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<WikiPage> CreateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();

            return await redmineManager.CreateWikiPageAsync(projectId, pageName, wikiPage, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task UpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.UpdateWikiPageAsync(projectId, pageName, wikiPage, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Deletes the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.DeleteWikiPageAsync(projectId, pageName, requestOptions).ConfigureAwait(false);
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
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            return await redmineManager.UploadFileAsync(data, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Downloads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        [Obsolete("Use DownloadFileAsync instead")]
        public static async Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();

            return await redmineManager.DownloadFileAsync(address, requestOptions).ConfigureAwait(false);
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
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions(parameters);
            return await redmineManager.GetWikiPageAsync(projectId, pageName, requestOptions, version).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets all wiki pages asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<List<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, NameValueCollection parameters, string projectId)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions(parameters);
            return await redmineManager.GetAllWikiPagesAsync(projectId, requestOptions).ConfigureAwait(false);
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
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.AddUserToGroupAsync(groupId, userId, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes an user from a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.RemoveUserFromGroupAsync(groupId, userId, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds the watcher asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.AddWatcherToIssueAsync(issueId, userId, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes the watcher asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.RemoveWatcherFromIssueAsync(issueId, userId, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<int> CountAsync<T>(this RedmineManager redmineManager, params string[] include) where T : class, new()
        {
            return await redmineManager.CountAsync<T>(null, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<int> CountAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions(parameters);
            return await redmineManager.CountAsync<T>(requestOptions).ConfigureAwait(false);
        }


        /// <summary>
        ///     Gets the paginated objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<PagedResults<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions(parameters);
            return await redmineManager.GetPagedAsync<T>(requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions(parameters);
            return await redmineManager.GetAsync<T>(requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets a Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions(parameters);
            return await redmineManager.GetAsync<T>(id, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object to create.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            return await redmineManager.CreateAsync(entity, null, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object to create.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity, string ownerId)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            return await redmineManager.CreateAsync(entity, ownerId, requestOptions, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///     Updates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The object.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T entity)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.UpdateAsync(id, entity, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Deletes the Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The id of the object to delete</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id)
            where T : class, new()
        {
            var requestOptions = RedmineManagerExtensions.CreateRequestOptions();
            await redmineManager.DeleteAsync<T>(id, requestOptions).ConfigureAwait(false);
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
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        public static async Task<PagedResults<Search>> SearchAsync(this RedmineManager redmineManager, string q, int limit = RedmineManager.DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null)
        {
            return await RedmineManagerExtensions.SearchAsync(redmineManager, q, limit, offset, searchFilter).ConfigureAwait(false);
        }
    }
}
#endif