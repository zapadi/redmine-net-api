using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
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
        public static PagedResults<News> GetProjectNews(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = Uri.EscapeDataString(redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier));

            var response = redmineManager.GetPaginatedObjects<News>(uri, requestOptions);

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

            var uri = Uri.EscapeDataString(redmineManager.RedmineApiUrls.ProjectNews(projectIdentifier));

            var payload = redmineManager.Serializer.Serialize(news);

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
        public static PagedResults<ProjectMembership> GetProjectMemberships(this RedmineManager redmineManager, string projectIdentifier, RequestOptions requestOptions = null)
        {
            var uri = redmineManager.RedmineApiUrls.ProjectMemberships(projectIdentifier);

            var response = redmineManager.GetPaginatedObjects<ProjectMembership>(uri, requestOptions);

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

            var response = redmineManager.GetPaginatedObjects<File>(uri, requestOptions);
            
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

            uri = Uri.EscapeDataString(uri);
            
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

            uri = Uri.EscapeDataString(uri);
            
            var response = redmineManager.ApiClient.Create(uri, payload, requestOptions);

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
            
            uri = Uri.EscapeDataString(uri);

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

            var response = redmineManager.GetObjects<WikiPage>(uri, requestOptions);

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
            
            uri = Uri.EscapeDataString(uri);
           
            redmineManager.ApiClient.Delete(uri,  requestOptions);
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

            var uri = Uri.EscapeDataString(url);

            var response = await redmineManager.ApiClient.CreateAsync(uri, payload,requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<WikiPage>(redmineManager.Serializer);
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
        public static async Task UpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var payload = redmineManager.Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(payload))
            {
                return;
            }

            var url = redmineManager.RedmineApiUrls.ProjectWikiPageUpdate(projectId, pageName);

            var uri = Uri.EscapeDataString(url);
            
            await redmineManager.ApiClient.PatchAsync(uri, payload, requestOptions, cancellationToken).ConfigureAwait(false);
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
            
            uri = Uri.EscapeDataString(uri);
           
            await redmineManager.ApiClient.DeleteAsync(uri,  requestOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server. This method does not block the calling thread.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     .
        /// </returns>
        public static async Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var url = redmineManager.RedmineApiUrls.UploadFragment();

            var response = await redmineManager.ApiClient.UploadFileAsync(url, data,requestOptions  , cancellationToken: cancellationToken).ConfigureAwait(false);
            
            return response.DeserializeTo<Upload>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Downloads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var response = await redmineManager.ApiClient.DownloadAsync(address, requestOptions,cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Content;
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
            
            uri = Uri.EscapeDataString(uri);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this RedmineManager redmineManager, params string[] include) where T : class, new()
        {
            RequestOptions requestOptions = null;

            if (include is {Length: > 0})
            {
                requestOptions = new RequestOptions()
                {
                    QueryString = new NameValueCollection
                    {
                        {RedmineKeys.INCLUDE, string.Join(",", include)}
                    }
                };
            }

            return await CountAsync<T>(redmineManager, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="requestOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this RedmineManager redmineManager, RequestOptions requestOptions) where T : class, new()
        {
            var totalCount = 0;
            const int PAGE_SIZE = 1;
            const int OFFSET = 0;

            if (requestOptions == null)
            {
                requestOptions = new RequestOptions();
            }
            
            requestOptions.QueryString.AddPagingParameters(PAGE_SIZE, OFFSET);

            var tempResult = await GetPagedAsync<T>(redmineManager, requestOptions).ConfigureAwait(false);
            if (tempResult != null)
            {
                totalCount = tempResult.TotalItems;
            }

            return totalCount;
        }


        /// <summary>
        ///     Gets the paginated objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static  async Task<PagedResults<T>> GetPagedAsync<T>(this RedmineManager redmineManager, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var url = redmineManager.RedmineApiUrls.GetListFragment<T>();

            var response= await redmineManager.ApiClient.GetAsync(url, requestOptions, cancellationToken).ConfigureAwait(false);
            
            return response.DeserializeToPagedResults<T>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Gets the objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            int pageSize = 0, offset = 0;
            var isLimitSet = false;
            List<T> resultList = null;

            if (requestOptions == null)
            {
                requestOptions = new RequestOptions();
            }
            
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
                pageSize = redmineManager.PageSize > 0
                    ? redmineManager.PageSize
                    : RedmineManager.DEFAULT_PAGE_SIZE_VALUE;
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
                        
                        var tempResult = await redmineManager.GetPagedAsync<T>(requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
                        
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
                    var result = await redmineManager.GetPagedAsync<T>(requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var url = redmineManager.RedmineApiUrls.GetFragment<T>(id);

            var response = await redmineManager.ApiClient.GetAsync(url,requestOptions, cancellationToken).ConfigureAwait(false);
            
            return response.DeserializeTo<T>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object to create.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            return await redmineManager.CreateObjectAsync( entity, null, requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="entity">The object to create.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T entity, string ownerId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var url = redmineManager.RedmineApiUrls.CreateEntityFragment<T>(ownerId);

            var payload = redmineManager.Serializer.Serialize(entity);
            
            var response = await redmineManager.ApiClient.CreateAsync(url, payload, requestOptions, cancellationToken).ConfigureAwait(false);

            return response.DeserializeTo<T>(redmineManager.Serializer);
        }

        /// <summary>
        ///     Updates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The object.</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T entity, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var url = redmineManager.RedmineApiUrls.UpdateFragment<T>(id);

            var payload = redmineManager.Serializer.Serialize(entity);
            
            await redmineManager.ApiClient.UpdateAsync(url, payload, requestOptions,cancellationToken: cancellationToken).ConfigureAwait(false);
           // data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");
        }

        /// <summary>
        ///     Deletes the Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var url = redmineManager.RedmineApiUrls.DeleteFragment<T>(id);

            await redmineManager.ApiClient.DeleteAsync(url, requestOptions, cancellationToken).ConfigureAwait((false));
        }
        #endif
        
        internal static RequestOptions CreateRequestOptions(NameValueCollection parameters = null, string impersonateUserName = null)
        {
            RequestOptions requestOptions = null;
            if (parameters != null || !impersonateUserName.IsNullOrWhiteSpace())
            {
                requestOptions = new RequestOptions()
                {
                    QueryString = parameters,
                    ImpersonateUser = impersonateUserName
                };
            }

            return requestOptions;
        }
    }
}