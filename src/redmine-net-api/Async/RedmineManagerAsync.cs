
#if NET20

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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void Task();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRes">The type of the resource.</typeparam>
    /// <returns></returns>
    public delegate TRes Task<out TRes>();

    /// <summary>
    /// 
    /// </summary>
    public static class RedmineManagerAsync
    {
        /// <summary>
        /// Gets the current user asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager,
            NameValueCollection parameters = null)
        {
            return delegate { return redmineManager.GetCurrentUser(parameters); };
        }

        /// <summary>
        /// Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <returns></returns>
        public static Task<WikiPage> CreateWikiPageAsync(this RedmineManager redmineManager, string projectId,
            string pageName, WikiPage wikiPage)
        {
            return delegate { return redmineManager.CreateWikiPage(projectId, pageName, wikiPage); };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        public static Task UpdateWikiPageAsync(this RedmineManager redmineManager, string projectId,
            string pageName, WikiPage wikiPage)
        {
            return delegate {  redmineManager.UpdateWikiPage(projectId, pageName, wikiPage); };
        }

        /// <summary>
        /// Deletes the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            return delegate { redmineManager.DeleteWikiPage(projectId, pageName); };
        }

        /// <summary>
        /// Gets the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId,
            NameValueCollection parameters, string pageName, uint version = 0)
        {
            return delegate { return redmineManager.GetWikiPage(projectId, parameters, pageName, version); };
        }

        /// <summary>
        /// Gets all wiki pages asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static Task<IList<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, string projectId)
        {
            return delegate { return redmineManager.GetAllWikiPages(projectId); };
        }

        /// <summary>
        /// Adds the user to group asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate { redmineManager.AddUserToGroup(groupId, userId); };
        }

        /// <summary>
        /// Removes the user from group asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate { redmineManager.RemoveUserFromGroup(groupId, userId); };
        }

        /// <summary>
        /// Adds the watcher to issue asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate { redmineManager.AddWatcherToIssue(issueId, userId); };
        }

        /// <summary>
        /// Removes the watcher from issue asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate { redmineManager.RemoveWatcherFromIssue(issueId, userId); };
        }

        /// <summary>
        /// Gets the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetObject<T>(id, parameters); };
        }

        /// <summary>
        /// Creates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object.</param>
        /// <returns></returns>
        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity) where T : class, new()
        {
            return CreateObjectAsync(redmineManager, entity, null);
        }

        /// <summary>
        /// Creates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity, string ownerId)
            where T : class, new()
        {
            return delegate { return redmineManager.CreateObject(entity, ownerId); };
        }

        /// <summary>
        /// Gets the paginated objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<PagedResults<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetPaginatedObjects<T>(parameters); };
        }

        /// <summary>
        /// Gets the objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetObjects<T>(parameters); };
        }

        /// <summary>
        /// Updates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The object.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T entity,
            string projectId = null) where T : class, new()
        {
            return delegate { redmineManager.UpdateObject(id, entity, projectId); };
        }

        /// <summary>
        /// Deletes the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id) where T : class, new()
        {
            return delegate { redmineManager.DeleteObject<T>(id); };
        }

        /// <summary>
        /// Uploads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            return delegate { return redmineManager.UploadFile(data); };
        }

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            return delegate { return redmineManager.DownloadFile(address); };
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
        public static Task<PagedResults<Search>> SearchAsync(this RedmineManager redmineManager, string q, int limit = RedmineManager.DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null)
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
            
            var result =  redmineManager.GetPaginatedObjectsAsync<Search>(parameters);

            return result;
        }
    }
}
#endif