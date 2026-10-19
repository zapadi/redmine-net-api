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

            redmineManager.ApiClient.Update(uri, string.Empty ,requestOptions);
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

            redmineManager.ApiClient.Update(uri, string.Empty ,requestOptions);
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

            redmineManager.ApiClient.Update(uri, string.Empty ,requestOptions);
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

            redmineManager.ApiClient.Update(uri,string.Empty, requestOptions);
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
            
            _ = redmineManager.ApiClient.Create(uri,string.Empty, requestOptions);
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
            
            _ = redmineManager.ApiClient.Delete(uri, requestOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
        public static PagedResults<News> GetProjectNews(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier);

            var response = redmineManager.GetPaginatedInternal<News>(uri, requestOptions);

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
            
            var uri = redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier);

            var response = redmineManager.ApiClient.Create(uri, payload, requestOptions);

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
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
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
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
        public static PagedResults<File> GetProjectFiles(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectFilesFragment(projectIdentifier);

            var response = redmineManager.GetPaginatedInternal<File>(uri, requestOptions);
            
            return response;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"> The id or the identifier of the project</param>
        /// <param name="requestOptions"></param>
        /// <returns>Returns the issue categories available for the specified project</returns>
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
        public static PagedResults<ProjectIssueCategory> GetProjectIssueCategories(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectIssueCategories(projectIdentifier);

            var response =  redmineManager.ApiClient.GetPaged(uri, requestOptions);

            return response.DeserializeToPagedResults<ProjectIssueCategory>(redmineManager.Serializer);
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
        ///     Updates the account of the user whose credentials are used to access the API (PUT /my/account).
        ///     Redmine accepts the first name, last name, email and custom field values.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="account">The account to update.</param>
        /// <param name="requestOptions"></param>
        public static void UpdateMyAccount(this RedmineManager redmineManager, MyAccount account, RequestOptions requestOptions = null)
        {
            var payload = SerializeMyAccount(redmineManager, account);

            redmineManager.ApiClient.Update(redmineManager.RedmineApiUrls.MyAccount(), payload, requestOptions);
        }

        private static string SerializeMyAccount(RedmineManager redmineManager, MyAccount account)
        {
            if (account == null)
            {
                throw new RedmineException("Argument account is null");
            }

            return redmineManager.Serializer.Serialize(account);
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
            var uri = redmineManager.RedmineApiUrls.IssueWatcherAdd(issueId.ToInvariantString());

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
            var uri = redmineManager.RedmineApiUrls.IssueWatcherRemove(issueId.ToInvariantString(), userId.ToInvariantString());
           
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
            var uri = redmineManager.RedmineApiUrls.GroupUserAdd(groupId.ToInvariantString());

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
            var uri = redmineManager.RedmineApiUrls.GroupUserRemove(groupId.ToInvariantString(), userId.ToInvariantString());
           
            redmineManager.ApiClient.Delete(uri, requestOptions);
        }

        /// <summary>
        ///     Adds existing users to a group (POST /groups/:id/users).
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userIds">The ids of the users to add.</param>
        /// <param name="requestOptions"></param>
        /// <exception cref="RedmineException"><paramref name="userIds"/> is null or empty.</exception>
        public static void AddUsersToGroup(this RedmineManager redmineManager, int groupId, IEnumerable<int> userIds, RequestOptions requestOptions = null)
        {
            var payload = SerializationHelper.SerializeUserIds(userIds, null, redmineManager.Serializer);

            redmineManager.ApiClient.Create(redmineManager.RedmineApiUrls.GroupUserAdd(groupId.ToInvariantString()), payload, requestOptions);
        }

        /// <summary>
        ///     Removes users from a group (DELETE /groups/:id/users). Requires Redmine 7.0 or later.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userIds">The ids of the users to remove.</param>
        /// <param name="requestOptions"></param>
        /// <exception cref="RedmineException"><paramref name="userIds"/> is null or empty.</exception>
        public static void RemoveUsersFromGroup(this RedmineManager redmineManager, int groupId, IEnumerable<int> userIds, RequestOptions requestOptions = null)
        {
            redmineManager.ApiClient.Delete(GroupUsersRemoveUri(redmineManager, groupId, userIds), requestOptions);
        }

        /// <summary>
        ///     Adds users as watchers of an issue (POST /issues/:id/watchers).
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId">The issue id.</param>
        /// <param name="userIds">The ids of the users to add as watchers.</param>
        /// <param name="requestOptions"></param>
        /// <exception cref="RedmineException"><paramref name="userIds"/> is null or empty.</exception>
        public static void AddWatchersToIssue(this RedmineManager redmineManager, int issueId, IEnumerable<int> userIds, RequestOptions requestOptions = null)
        {
            var payload = SerializationHelper.SerializeUserIds(userIds, RedmineKeys.WATCHER, redmineManager.Serializer);

            redmineManager.ApiClient.Create(redmineManager.RedmineApiUrls.IssueWatcherAdd(issueId.ToInvariantString()), payload, requestOptions);
        }

        private static string GroupUsersRemoveUri(RedmineManager redmineManager, int groupId, IEnumerable<int> userIds)
        {
            return redmineManager.RedmineApiUrls.GroupUsersRemove(groupId.ToInvariantString(), SerializationHelper.EnsureUserIds(userIds));
        }

        /// <summary>
        ///     Updates the notes and/or the private flag of a journal (PUT /journals/:id).
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="journalId">The journal id.</param>
        /// <param name="notes">The new notes; <c>null</c> leaves the notes unchanged.</param>
        /// <param name="privateNotes">Whether the notes are private; <c>null</c> leaves the flag unchanged.</param>
        /// <param name="requestOptions"></param>
        /// <exception cref="RedmineException">Both <paramref name="notes"/> and <paramref name="privateNotes"/> are <c>null</c>.</exception>
        public static void UpdateJournal(this RedmineManager redmineManager, int journalId, string notes, bool? privateNotes = null, RequestOptions requestOptions = null)
        {
            var payload = SerializeJournal(redmineManager, notes, privateNotes);

            redmineManager.ApiClient.Update(JournalUri(redmineManager, journalId), payload, requestOptions);
        }

        /// <summary>
        ///     Clears the notes of a journal (PUT /journals/:id with empty notes).
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="journalId">The journal id.</param>
        /// <param name="requestOptions"></param>
        public static void DeleteJournalNotes(this RedmineManager redmineManager, int journalId, RequestOptions requestOptions = null)
        {
            UpdateJournal(redmineManager, journalId, string.Empty, null, requestOptions);
        }

        private static string SerializeJournal(RedmineManager redmineManager, string notes, bool? privateNotes)
        {
            if (notes == null && privateNotes == null)
            {
                throw new RedmineException("Either notes or privateNotes must be set.");
            }

            return SerializationHelper.SerializeJournal(notes, privateNotes, redmineManager.Serializer);
        }

        private static string JournalUri(RedmineManager redmineManager, int journalId)
        {
            return redmineManager.RedmineApiUrls.UpdateFragment<Journal>(journalId.ToInvariantString());
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

            redmineManager.ApiClient.Patch(uri, payload, requestOptions);
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
            
            var response = redmineManager.ApiClient.Update(uri, payload, requestOptions);

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
                : redmineManager.RedmineApiUrls.ProjectWikiPageVersion(projectId, pageName, version.ToInvariantString());
            
            var response = redmineManager.ApiClient.Get(uri, requestOptions);

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
            
            redmineManager.ApiClient.Delete(uri,  requestOptions);
        }

        /// <summary>
        ///     Updates the attachment.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="requestOptions"></param>
        [Obsolete("Redmine does not accept API authentication for this endpoint; use UpdateAttachment.")]
        public static void UpdateIssueAttachment(this RedmineManager redmineManager, int issueId, Attachment attachment, RequestOptions requestOptions = null)
        {
            if (attachment == null)
            {
                throw new RedmineException("Argument attachment is null");
            }

            var attachments = new Attachments
            {
                {attachment.Id, attachment}
            };

            var data = redmineManager.Serializer.Serialize(attachments);
            
            var uri = redmineManager.RedmineApiUrls.AttachmentUpdate(issueId.ToInvariantString());

            redmineManager.ApiClient.Patch(uri,  data, requestOptions);
        }

        /// <summary>
        ///     Updates the file name and description of the attachment identified by <see cref="Identifiable{T}.Id"/>.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="requestOptions"></param>
        public static void UpdateAttachment(this RedmineManager redmineManager, Attachment attachment, RequestOptions requestOptions = null)
        {
            var data = SerializeAttachment(redmineManager, attachment);

            redmineManager.ApiClient.Patch(AttachmentUri(redmineManager, attachment), data, requestOptions);
        }

        private static string SerializeAttachment(RedmineManager redmineManager, Attachment attachment)
        {
            if (attachment == null)
            {
                throw new RedmineException("Argument attachment is null");
            }

            return redmineManager.Serializer.Serialize(attachment);
        }

        private static string AttachmentUri(RedmineManager redmineManager, Attachment attachment)
        {
            return redmineManager.RedmineApiUrls.UpdateFragment<Attachment>(attachment.Id.ToInvariantString());
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

            var response = redmineManager.GetPaginated<Search>(new RequestOptions
            {
                QueryString = parameters, 
                ImpersonateUser = impersonateUserName
            });

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
                {RedmineKeys.LIMIT, limit.ToInvariantString()},
                {RedmineKeys.OFFSET, offset.ToInvariantString()},
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
            
            await redmineManager.ApiClient.UpdateAsync(uri, string.Empty, requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            await redmineManager.ApiClient.UpdateAsync(uri, string.Empty, requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            await redmineManager.ApiClient.UpdateAsync(uri, string.Empty,  requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            await redmineManager.ApiClient.UpdateAsync(uri, string.Empty, requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            await redmineManager.ApiClient.CreateAsync(uri, string.Empty ,requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            await redmineManager.ApiClient.DeleteAsync(uri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
        public static async Task<PagedResults<News>> GetProjectNewsAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier);

            var response = await redmineManager.ApiClient.GetPagedAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

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
            
            var uri = redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier);
            
            var response = await redmineManager.ApiClient.CreateAsync(uri, payload, requestOptions, cancellationToken).ConfigureAwait(false);

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
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
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
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
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
        /// <param name="projectIdentifier"> The id or the identifier of the project</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns the issue categories available for the specified project</returns>
        /// <remarks>
        /// Returns a single page. To get all the items use Get&lt;T&gt; or GetAsync&lt;T&gt; with the <c>project_id</c> query string parameter.
        /// </remarks>
        public static async Task<PagedResults<ProjectIssueCategory>> GetProjectIssueCategoriesAsync(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectIssueCategories(projectIdentifier);

            var response = await redmineManager.ApiClient.GetPagedAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeToPagedResults<ProjectIssueCategory>(redmineManager.Serializer);
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
        public static async Task<PagedResults<Search>> SearchAsync(this RedmineManager redmineManager, 
            string q, 
            int limit = RedmineConstants.DEFAULT_PAGE_SIZE_VALUE, 
            int offset = 0, 
            SearchFilterBuilder searchFilter = null,
            CancellationToken cancellationToken = default)
        {
            var parameters = CreateSearchParameters(q, limit, offset, searchFilter);

            var response = await redmineManager.GetPagedAsync<Search>(new RequestOptions()
            {
                QueryString = parameters
            }, cancellationToken).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Searches asynchronously using the specified request options (query string, impersonation, headers).
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="q">query strings. enable to specify multiple values separated by a space " ".</param>
        /// <param name="limit">number of results in response.</param>
        /// <param name="offset">skip this number of results in response</param>
        /// <param name="searchFilter">Optional filters; can be null.</param>
        /// <param name="requestOptions">The request options; can be null. q, limit, offset and the search filters replace the same keys of its query string.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Returns the search results by the specified condition parameters.
        /// </returns>
        public static async Task<PagedResults<Search>> SearchAsync(this RedmineManager redmineManager,
            string q,
            int limit,
            int offset,
            SearchFilterBuilder searchFilter,
            RequestOptions requestOptions,
            CancellationToken cancellationToken = default)
        {
            var parameters = CreateSearchParameters(q, limit, offset, searchFilter);

            var options = requestOptions != null ? requestOptions.Clone() : new RequestOptions();
            options.QueryString ??= new NameValueCollection();
            foreach (string key in parameters)
            {
                options.QueryString.Set(key, parameters[key]);
            }

            var response = await redmineManager.GetPagedAsync<Search>(options, cancellationToken).ConfigureAwait(false);

            return response;
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
        /// Retrieves the account details of the currently authenticated user.
        /// </summary>
        /// <param name="redmineManager">The instance of the RedmineManager used to perform the API call.</param>
        /// <param name="requestOptions">Optional configuration for the API request.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns the account details of the authenticated user as a MyAccount object.</returns>
        public static async Task<MyAccount> GetMyAccountAsync(this RedmineManager redmineManager, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var uri = redmineManager.RedmineApiUrls.MyAccount();

            var response = await redmineManager.ApiClient.GetAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<MyAccount>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Updates the account of the user whose credentials are used to access the API (PUT /my/account) asynchronous.
        ///     Redmine accepts the first name, last name, email and custom field values.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="account">The account to update.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task UpdateMyAccountAsync(this RedmineManager redmineManager, MyAccount account, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = SerializeMyAccount(redmineManager, account);

            await redmineManager.ApiClient.UpdateAsync(redmineManager.RedmineApiUrls.MyAccount(), payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        ///     Creates or updates wiki page asynchronous.
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
            if (pageName.IsNullOrWhiteSpace())
            {
                throw new RedmineException("Page name cannot be blank");
            }
            
            var payload = redmineManager.Serializer.Serialize(wikiPage);
            
            if (string.IsNullOrEmpty(payload))
            {
                throw new RedmineException("The payload is empty");
            }

            var uri = redmineManager.RedmineApiUrls.ProjectWikiPageUpdate(projectId, pageName);
            
            var response = await redmineManager.ApiClient.UpdateAsync(uri, payload, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<WikiPage>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Creates or updates wiki page asynchronous.
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

            await redmineManager.ApiClient.PatchAsync(url, payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Updates the file name and description of the attachment identified by <see cref="Identifiable{T}.Id"/> asynchronous.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static async Task UpdateAttachmentAsync(this RedmineManager redmineManager, Attachment attachment, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var data = SerializeAttachment(redmineManager, attachment);

            await redmineManager.ApiClient.PatchAsync(AttachmentUri(redmineManager, attachment), data, requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            await redmineManager.ApiClient.DeleteAsync(uri,  requestOptions, cancellationToken).ConfigureAwait(false);
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
                : redmineManager.RedmineApiUrls.ProjectWikiPageVersion(projectId, pageName, version.ToInvariantString());
            
            var response = await redmineManager.ApiClient.GetAsync(uri, requestOptions, cancellationToken).ConfigureAwait(false);

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
            var uri = redmineManager.RedmineApiUrls.GroupUserAdd(groupId.ToInvariantString());

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
            var uri = redmineManager.RedmineApiUrls.GroupUserRemove(groupId.ToInvariantString(), userId.ToInvariantString());
           
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
            var uri = redmineManager.RedmineApiUrls.IssueWatcherAdd(issueId.ToInvariantString());

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
            var uri = redmineManager.RedmineApiUrls.IssueWatcherRemove(issueId.ToInvariantString(), userId.ToInvariantString());
           
            await redmineManager.ApiClient.DeleteAsync(uri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds existing users to a group (POST /groups/:id/users) asynchronous.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userIds">The ids of the users to add.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="RedmineException"><paramref name="userIds"/> is null or empty.</exception>
        public static async Task AddUsersToGroupAsync(this RedmineManager redmineManager, int groupId, IEnumerable<int> userIds, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = SerializationHelper.SerializeUserIds(userIds, null, redmineManager.Serializer);

            await redmineManager.ApiClient.CreateAsync(redmineManager.RedmineApiUrls.GroupUserAdd(groupId.ToInvariantString()), payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Removes users from a group (DELETE /groups/:id/users) asynchronous. Requires Redmine 7.0 or later.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId">The group id.</param>
        /// <param name="userIds">The ids of the users to remove.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="RedmineException"><paramref name="userIds"/> is null or empty.</exception>
        public static async Task RemoveUsersFromGroupAsync(this RedmineManager redmineManager, int groupId, IEnumerable<int> userIds, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            await redmineManager.ApiClient.DeleteAsync(GroupUsersRemoveUri(redmineManager, groupId, userIds), requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds users as watchers of an issue (POST /issues/:id/watchers) asynchronous.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId">The issue id.</param>
        /// <param name="userIds">The ids of the users to add as watchers.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="RedmineException"><paramref name="userIds"/> is null or empty.</exception>
        public static async Task AddWatchersToIssueAsync(this RedmineManager redmineManager, int issueId, IEnumerable<int> userIds, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = SerializationHelper.SerializeUserIds(userIds, RedmineKeys.WATCHER, redmineManager.Serializer);

            await redmineManager.ApiClient.CreateAsync(redmineManager.RedmineApiUrls.IssueWatcherAdd(issueId.ToInvariantString()), payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Updates the notes and/or the private flag of a journal (PUT /journals/:id) asynchronous.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="journalId">The journal id.</param>
        /// <param name="notes">The new notes; <c>null</c> leaves the notes unchanged.</param>
        /// <param name="privateNotes">Whether the notes are private; <c>null</c> leaves the flag unchanged.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="RedmineException">Both <paramref name="notes"/> and <paramref name="privateNotes"/> are <c>null</c>.</exception>
        public static async Task UpdateJournalAsync(this RedmineManager redmineManager, int journalId, string notes, bool? privateNotes = null, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = SerializeJournal(redmineManager, notes, privateNotes);

            await redmineManager.ApiClient.UpdateAsync(JournalUri(redmineManager, journalId), payload, requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Clears the notes of a journal (PUT /journals/:id with empty notes) asynchronous.
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="journalId">The journal id.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        public static Task DeleteJournalNotesAsync(this RedmineManager redmineManager, int journalId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return UpdateJournalAsync(redmineManager, journalId, string.Empty, null, requestOptions, cancellationToken);
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