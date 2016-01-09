using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Redmine.Net.Api.Types
{
    public interface IRedmineManagerAsync : IRedmineManager
    {
        Task<User> GetCurrentUserAsync(NameValueCollection parameters = null);
        Task<IList<User>> GetUsersAsync(UserStatus userStatus = UserStatus.STATUS_ACTIVE, string name = null, int groupId = 0);
        Task<WikiPage> CreateOrUpdateWikiPageAsync(string projectId, string pageName, WikiPage wikiPage);
        Task DeleteWikiPageAsync(string projectId, string pageName);
        Task<Upload> UploadFileAsync(byte[] data);
        Task<byte[]> DownloadFileAsync(string address);
        Task<WikiPage> GetWikiPageAsync(string projectId, NameValueCollection parameters, string pageName, uint version = 0);
        Task<IList<WikiPage>> GetAllWikiPagesAsync(string projectId);
        Task AddUserToGroupAsync(int groupId, int userId);
        Task DeleteUserFromGroupAsync(int groupId, int userId);
        Task AddWatcherAsync(int issueId, int userId);
        Task RemoveWatcherAsync(int issueId, int userId);
        Task<IList<T>> GetObjectListAsync<T>(NameValueCollection parameters);
     
        Task<IList<T>> GetTotalObjectListAsync<T>(NameValueCollection parameters) where T : class, new();
        Task<T> GetObjectAsync<T>(string id, NameValueCollection parameters) where T : class, new();
        Task<T> CreateObjectAsync<T>(T obj) where T : class, new();
        Task<T> CreateObjectAsync<T>(T obj, string ownerId) where T : class, new();
        Task UpdateObjectAsync<T>(string id, T obj, string projectId = null) where T : class, new();
        Task DeleteObjectAsync<T>(string id, NameValueCollection parameters) where T : class;
    }
}