using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    public delegate void Task();

    public delegate TRes Task<out TRes>();

    public static class RedmineManagerAsync
    {
        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager,
            NameValueCollection parameters = null)
        {
            return delegate { return redmineManager.GetCurrentUser(parameters); };
        }

        public static Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager, string projectId,
            string pageName, WikiPage wikiPage)
        {
            return delegate { return redmineManager.CreateOrUpdateWikiPage(projectId, pageName, wikiPage); };
        }

        public static Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            return delegate { redmineManager.DeleteWikiPage(projectId, pageName); };
        }

        public static Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId,
            NameValueCollection parameters, string pageName, uint version = 0)
        {
            return delegate { return redmineManager.GetWikiPage(projectId, parameters, pageName, version); };
        }

        public static Task<IList<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager,
            NameValueCollection parameters, string projectId)
        {
            return delegate { return redmineManager.GetAllWikiPages(projectId); };
        }

        public static Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate { redmineManager.AddUserToGroup(groupId, userId); };
        }

        public static Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate { redmineManager.RemoveUserFromGroup(groupId, userId); };
        }

        public static Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate { redmineManager.AddWatcherToIssue(issueId, userId); };
        }

        public static Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate { redmineManager.RemoveWatcherFromIssue(issueId, userId); };
        }

        public static Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetObject<T>(id, parameters); };
        }

        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj) where T : class, new()
        {
            return CreateObjectAsync(redmineManager, obj, null);
        }

        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj, string ownerId)
            where T : class, new()
        {
            return delegate { return redmineManager.CreateObject(obj, ownerId); };
        }


        public static Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetPaginatedObjects<T>(parameters); };
        }


        public static Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetObjects<T>(parameters); };
        }
        
        public static Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj,
            string projectId = null) where T : class, new()
        {
            return delegate { redmineManager.UpdateObject(id, obj, projectId); };
        }

        public static Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id,
            NameValueCollection parameters) where T : class, new()
        {
            return delegate { redmineManager.DeleteObject<T>(id, parameters); };
        }

        public static Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            return delegate { return redmineManager.UploadFile(data); };
        }

        public static Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            return delegate { return redmineManager.DownloadFile(address); };
        }
    }
}