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
#if !(NET20)
using System.Threading;
using System.Threading.Tasks;
#endif
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RedmineManagerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static void ArchiveProject(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectArchive(projectIdentifier);

            var escapedUri = Uri.EscapeDataString(uri);
            
            redmineManager.ApiClient.Update(escapedUri, string.Empty ,requestOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static void UnarchiveProject(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectUnarchive(projectIdentifier);

            var escapedUri = Uri.EscapeDataString(uri);
            
            redmineManager.ApiClient.Update(escapedUri, string.Empty ,requestOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static void ReopenProject(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectReopen(projectIdentifier);

            var escapedUri = Uri.EscapeDataString(uri);
            
            redmineManager.ApiClient.Update(escapedUri, string.Empty ,requestOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static void CloseProject(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectClose(projectIdentifier);

            var escapedUri = Uri.EscapeDataString(uri);
            
            redmineManager.ApiClient.Update(escapedUri,string.Empty, requestOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="repositoryIdentifier"></param>
        /// <param name="revision"></param>
        /// <param name="requestOptions"></param>
        public static void ProjectRepositoryAddRelatedIssue(this RedmineManager redmineManager, string projectIdentifier, string repositoryIdentifier, string revision, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectRepositoryAddRelatedIssue(projectIdentifier, repositoryIdentifier, revision);
            
            var escapedUri = Uri.EscapeDataString(uri);
            
            _ = redmineManager.ApiClient.Create(escapedUri,string.Empty, requestOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="repositoryIdentifier"></param>
        /// <param name="revision"></param>
        /// <param name="issueIdentifier"></param>
        /// <param name="requestOptions"></param>
        public static void ProjectRepositoryRemoveRelatedIssue(this RedmineManager redmineManager, string projectIdentifier, string repositoryIdentifier, string revision, string issueIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectRepositoryRemoveRelatedIssue(projectIdentifier, repositoryIdentifier, revision, issueIdentifier);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            _ = redmineManager.ApiClient.Delete(escapedUri, requestOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static PagedResults<News> GetProjectNews(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier);

            var escapedUri = Uri.EscapeDataString(uri);
            
            var response = redmineManager.GetPaginatedInternal<News>(escapedUri, requestOptions);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="news"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static News AddProjectNews(this RedmineManager redmineManager, string projectIdentifier, News news, RequestOptions requestOptions = null)
        {
            if (news == null)
            {
                throw new RedmineException("Argument news is null");
            }

            if (news.Title.IsNullOrWhiteSpace())
            {
                throw new RedmineException("News title cannot be blank");
            }
            
            var payload = redmineManager.Serializer.Serialize(news);
            
            var uri = Uri.EscapeDataString(redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier));
            
            var escapedUri = Uri.EscapeDataString(uri);

            var response = redmineManager.ApiClient.Create(escapedUri, payload, requestOptions);

            return response.DeserializeTo<News>(redmineManager.Serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static PagedResults<ProjectMembership> GetProjectMemberships(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectMemberships(projectIdentifier);

            var response = redmineManager.GetPaginatedInternal<ProjectMembership>(uri, requestOptions);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static PagedResults<File> GetProjectFiles(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectFilesFragment(projectIdentifier);

            var response = redmineManager.GetPaginatedInternal<File>(uri, requestOptions);
            
            return response;
        }

        /// <summary>
        ///     Returns the user whose credentials are used to access the API.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static User GetCurrentUser(this RedmineManager redmineManager, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.CurrentUser();

            var response = redmineManager.ApiClient.Get(uri, requestOptions);

            return response.DeserializeTo<User>(redmineManager.Serializer);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the my account details.</returns>
        public static MyAccount GetMyAccount(this RedmineManager redmineManager, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.MyAccount();

            var response = redmineManager.ApiClient.Get(uri, requestOptions);

            return response.DeserializeTo<MyAccount>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Adds the watcher to issue.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requestOptions"></param>
        public static void AddWatcherToIssue(this RedmineManager redmineManager, int issueId, int userId, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.IssueWatcherAdd(issueId.ToString(CultureInfo.InvariantCulture));

            var payload = SerializationHelper.SerializeUserId(userId, redmineManager.Serializer);
            
            redmineManager.ApiClient.Create(uri, payload, requestOptions);
        }

        /// <summary>
        ///     Removes the watcher from issue.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requestOptions"></param>
        public static void RemoveWatcherFromIssue(this RedmineManager redmineManager, int issueId, int userId, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.IssueWatcherRemove(issueId.ToString(CultureInfo.InvariantCulture), userId.ToString(CultureInfo.InvariantCulture));
           
            redmineManager.ApiClient.Delete(uri,  requestOptions);
        }

        /// <summary>
        ///     Adds an existing user to a group.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="requestOptions"></param>
        public static void AddUserToGroup(this RedmineManager redmineManager, int groupId, int userId, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.GroupUserAdd(groupId.ToString(CultureInfo.InvariantCulture));

            var payload = SerializationHelper.SerializeUserId(userId, redmineManager.Serializer);
            
            redmineManager.ApiClient.Create(uri, payload, requestOptions);
        }

        /// <summary>
        ///     Removes an user from a group.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="requestOptions"></param>
        public static void RemoveUserFromGroup(this RedmineManager redmineManager, int groupId, int userId, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.GroupUserRemove(groupId.ToString(CultureInfo.InvariantCulture), userId.ToString(CultureInfo.InvariantCulture));
           
            redmineManager.ApiClient.Delete(uri, requestOptions);
        }

        /// <summary>
        ///     Creates or updates a wiki page.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="wikiPage">The wiki page to create or update.</param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static void UpdateWikiPage(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage, RequestOptions requestOptions = null)
        {
            var payload = redmineManager.Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(payload))
            {
                return;
            }

            var uri = redmineManager.RedmineApiUrls.ProjectWikiPageUpdate(projectId, pageName);

            var escapedUri = Uri.EscapeDataString(uri);
            
            redmineManager.ApiClient.Patch(escapedUri, payload, requestOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static WikiPage CreateWikiPage(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage, RequestOptions requestOptions = null)
        {
            var payload = redmineManager.Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(payload))
            {
                throw new RedmineException("The payload is empty");
            }

            var uri = redmineManager.RedmineApiUrls.ProjectWikiPageUpdate(projectId, pageName);

            var escapedUri = Uri.EscapeDataString(uri);
            
            var response = redmineManager.ApiClient.Create(escapedUri, payload, requestOptions);

            return response.DeserializeTo<WikiPage>(redmineManager.Serializer);
        }

        /// <summary>
        /// Gets the wiki page.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="requestOptions"></param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static WikiPage GetWikiPage(this RedmineManager redmineManager, string projectId,  string pageName, RequestOptions requestOptions = null, uint version = 0)
        {
            var uri = version == 0
                ? redmineManager.RedmineApiUrls.ProjectWikiPage(projectId, pageName)
                : redmineManager.RedmineApiUrls.ProjectWikiPageVersion(projectId, pageName, version.ToString(CultureInfo.InvariantCulture));
            
            var escapedUri = Uri.EscapeDataString(uri);

            var response = redmineManager.ApiClient.Get(escapedUri, requestOptions);

            return response.DeserializeTo<WikiPage>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static List<WikiPage> GetAllWikiPages(this RedmineManager redmineManager, string projectId, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectWikiIndex(projectId);

            var response = redmineManager.GetInternal<WikiPage>(uri, requestOptions);

            return response;
        }

        /// <summary>
        ///     Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not
        ///     deleted but changed as root pages.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="requestOptions"></param>
        public static void DeleteWikiPage(this RedmineManager redmineManager, string projectId, string pageName, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectWikiPageDelete(projectId, pageName);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            redmineManager.ApiClient.Delete(escapedUri,  requestOptions);
        }

        /// <summary>
        ///     Updates the attachment.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="requestOptions"></param>
        public static void UpdateIssueAttachment(this RedmineManager redmineManager, int issueId, Attachment attachment, RequestOptions requestOptions = null)
        {
            var attachments = new Attachments
            {
                {attachment.Id, attachment}
            };

            var data = redmineManager.Serializer.Serialize(attachments);
            
            var uri = redmineManager.RedmineApiUrls.AttachmentUpdate(issueId.ToString(CultureInfo.InvariantCulture));

            redmineManager.ApiClient.Patch(uri,  data, requestOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="q">query strings. enable to specify multiple values separated by a space " ".</param>
        /// <param name="limit">number of results in response.</param>
        /// <param name="offset">skip this number of results in response</param>
        /// <param name="searchFilter">Optional filters.</param>
        /// <param name="impersonateUserName"></param>
        /// <returns>
        /// Returns the search results by the specified condition parameters.
        /// </returns>
        public static PagedResults<Search> Search(this RedmineManager redmineManager, string q, int limit = RedmineConstants.DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null, string impersonateUserName = null)
        {
            var parameters = CreateSearchParameters(q, limit, offset, searchFilter);

            var response = redmineManager.GetPaginatedObjects<Search>(parameters);

            return response;
        }
        
        private static NameValueCollection CreateSearchParameters(string q, int limit, int offset, SearchFilterBuilder searchFilter)
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

            return searchFilter != null ? searchFilter.Build(parameters) : parameters;
        }

        #if !(NET20)

        /// <summary>
        /// Archives the project asynchronously
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task ArchiveProjectAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectArchive(projectIdentifier);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.DeleteAsync(escapedUri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Unarchives the project asynchronously
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task UnarchiveProjectAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectUnarchive(projectIdentifier);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.DeleteAsync(escapedUri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Closes the project asynchronously
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task CloseProjectAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectClose(projectIdentifier);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.UpdateAsync(escapedUri, string.Empty,  requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Reopens the project asynchronously
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task ReopenProjectAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectReopen(projectIdentifier);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.UpdateAsync(escapedUri, string.Empty, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="repositoryIdentifier"></param>
        /// <param name="revision"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task ProjectRepositoryAddRelatedIssueAsync(this RedmineManager redmineManager, string projectIdentifier, string repositoryIdentifier, string revision, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectRepositoryAddRelatedIssue(projectIdentifier, repositoryIdentifier, revision);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.CreateAsync(escapedUri, string.Empty ,requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="repositoryIdentifier"></param>
        /// <param name="revision"></param>
        /// <param name="issueIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task ProjectRepositoryRemoveRelatedIssueAsync(this RedmineManager redmineManager, string projectIdentifier, string repositoryIdentifier, string revision, string issueIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectRepositoryRemoveRelatedIssue(projectIdentifier, repositoryIdentifier, revision, issueIdentifier);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.DeleteAsync(escapedUri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<PagedResults<News>> GetProjectNewsAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier);

            var escapedUri = Uri.EscapeDataString(uri);
            
            var response = await redmineManager.ApiClient.GetPagedAsync(escapedUri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeToPagedResults<News>(redmineManager.Serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="news"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static async Task<News> AddProjectNewsAsync(this RedmineManager redmineManager, string projectIdentifier, News news, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (news == null)
            {
                throw new RedmineException("Argument news is null");
            }

            if (news.Title.IsNullOrWhiteSpace())
            {
                throw new RedmineException("News title cannot be blank");
            }
            
            var payload = redmineManager.Serializer.Serialize(news);
            
            var uri = Uri.EscapeDataString(redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier));
            
            var escapedUri = Uri.EscapeDataString(uri);

            var response = await redmineManager.ApiClient.CreateAsync(escapedUri, payload, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<News>(redmineManager.Serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static async Task<PagedResults<ProjectMembership>> GetProjectMembershipsAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectMemberships(projectIdentifier);

            var response = await redmineManager.ApiClient.GetPagedAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeToPagedResults<ProjectMembership>(redmineManager.Serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static async Task<PagedResults<File>> GetProjectFilesAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectFilesFragment(projectIdentifier);

            var response = await redmineManager.ApiClient.GetPagedAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);
            
            return response.DeserializeToPagedResults<File>(redmineManager.Serializer);
        }

        
        /// <summary>
        ///  
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="q"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="searchFilter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<PagedResults<Search>> SearchAsync(this RedmineManager redmineManager, string q, int limit = RedmineManager.DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null, CancellationToken cancellationToken = default)
        {
            var parameters = CreateSearchParameters(q, limit, offset, searchFilter);

            var response = await redmineManager.ApiClient.GetPagedAsync("", new RequestOptions()
            {
                QueryString = parameters
            }, cancellationToken).ConfigureAwait(false);

            return response.DeserializeToPagedResults<Search>(redmineManager.Serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.CurrentUser();

            var response = await redmineManager.ApiClient.GetAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<User>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<WikiPage> CreateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = redmineManager.Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(payload))
            {
                throw new RedmineException("The payload is empty");
            }

            var url = redmineManager.RedmineApiUrls.ProjectWikiPageUpdate(projectId, pageName);

            var escapedUri = Uri.EscapeDataString(url);

            var response = await redmineManager.ApiClient.CreateAsync(escapedUri, payload,requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<WikiPage>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Creates or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task UpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = redmineManager.Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(payload))
            {
                return;
            }

            var url = redmineManager.RedmineApiUrls.ProjectWikiPageUpdate(projectId, pageName);

            var escapedUri = Uri.EscapeDataString(url);
            
            await redmineManager.ApiClient.PatchAsync(escapedUri, payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Deletes the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectWikiPageDelete(projectId, pageName);
            
            var escapedUri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.DeleteAsync(escapedUri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        ///     Gets the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="requestOptions"></param>
        /// <param name="version">The version.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, RequestOptions requestOptions = null, uint version = 0, CancellationToken cancellationToken = default)
        {
            var uri = version == 0
                ? redmineManager.RedmineApiUrls.ProjectWikiPage(projectId, pageName)
                : redmineManager.RedmineApiUrls.ProjectWikiPageVersion(projectId, pageName, version.ToString(CultureInfo.InvariantCulture));
            
            var escapedUri = Uri.EscapeDataString(uri);

            var response = await redmineManager.ApiClient.GetAsync(escapedUri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<WikiPage>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Gets all wiki pages asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, string projectId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectWikiIndex(projectId);

            var response = await redmineManager.ApiClient.GetPagedAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeToList<WikiPage>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Adds an existing user to a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     Returns the Guid associated with the async request.
        /// </returns>
        public static async Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.GroupUserAdd(groupId.ToString(CultureInfo.InvariantCulture));

            var payload = SerializationHelper.SerializeUserId(userId, redmineManager.Serializer);
            
            await redmineManager.ApiClient.CreateAsync(uri, payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes an user from a group. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.GroupUserRemove(groupId.ToString(CultureInfo.InvariantCulture), userId.ToString(CultureInfo.InvariantCulture));
           
            await redmineManager.ApiClient.DeleteAsync(uri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds the watcher asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId, RequestOptions requestOptions = null , CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.IssueWatcherAdd(issueId.ToString(CultureInfo.InvariantCulture));

            var payload = SerializationHelper.SerializeUserId(userId, redmineManager.Serializer);
            
            await redmineManager.ApiClient.CreateAsync(uri, payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes the watcher asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.IssueWatcherRemove(issueId.ToString(CultureInfo.InvariantCulture), userId.ToString(CultureInfo.InvariantCulture));
           
            await redmineManager.ApiClient.DeleteAsync(uri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }
        #endif
        
        internal static RequestOptions CreateRequestOptions(NameValueCollection parameters = null, string impersonateUserName = null)
        {
            RequestOptions requestOptions = null;
            if (parameters != null)
            {
                requestOptions = new RequestOptions()
                {
                    QueryString = parameters
                };
            }

            if (impersonateUserName.IsNullOrWhiteSpace())
            {
                return requestOptions;
            }
            
            requestOptions ??= new RequestOptions();
            requestOptions.ImpersonateUser = impersonateUserName;

            return requestOptions;
        }
    }
}