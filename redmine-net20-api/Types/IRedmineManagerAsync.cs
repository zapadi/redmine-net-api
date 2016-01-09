using System;
using System.Collections.Specialized;

namespace Redmine.Net.Api.Types
{
    public interface IRedmineManagerAsync : IRedmineManager
    {
        Guid GetCurrentUserAsync(NameValueCollection parameters = null);

        Guid CreateOrUpdateWikiPageAsync(string projectId, string pageName, WikiPage wikiPage);

        Guid DeleteWikiPageAsync(string projectId, string pageName);

        Guid UploadDataAsync(byte[] data);

        Guid GetWikiPageAsync(string projectId, NameValueCollection parameters, string pageName, uint version = 0);

        Guid AddUserToGroupAsync(int groupId, int userId);

        Guid GetObjectListAsync<T>(NameValueCollection parameters);

        Guid DeleteUserFromGroupAsync(int groupId, int userId);

        Guid CreateObjectAsync<T>(T obj) where T : class, new();

        Guid UpdateObjectAsync<T>(string id, T obj, string projectId = null) where T : class, new();

        Guid DeleteObjectAsync<T>(string id, NameValueCollection parameters) where T : class;
    }
}