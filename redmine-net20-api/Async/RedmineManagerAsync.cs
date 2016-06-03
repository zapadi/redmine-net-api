using System.Collections.Generic;
using System.Collections.Specialized;
<<<<<<< HEAD
=======
using System.Net;
using System.Text;
using System.Threading;
using Redmine.Net.Api.Internals;
>>>>>>> master
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    public delegate void Task();
    public delegate TRes Task<out TRes>();

    public static class RedmineManagerAsync
    {
<<<<<<< HEAD
        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager,NameValueCollection parameters = null)
=======
        public static Task<TRes> Task<TRes>(Task<TRes> task)
        {
            TRes result = default(TRes);
            bool completed = false;

            object sync = new object();
            task.BeginInvoke(iac =>
            {
                lock (sync)
                {
                    completed = true;
                    result = task.EndInvoke(iac);
                    Monitor.Pulse(sync);
                }
            }, null);

            return delegate
            {
                lock (sync)
                {
                    if (!completed)
                    {
                        Monitor.Wait(sync);
                    }
                    return result;
                }
            };
        }

        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, NameValueCollection parameters = null)
>>>>>>> master
        {
            return delegate
            {
<<<<<<< HEAD
                return redmineManager.GetCurrentUser(parameters);
=======
                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    var uri = UrlHelper.GetCurrentUserUrl(redmineManager);
                    var response = wc.DownloadString(new Uri(uri));
                    return RedmineSerializer.Deserialize<User>(response,redmineManager.MimeFormat);
                }
>>>>>>> master
            };
        }

<<<<<<< HEAD
        public static Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager, string projectId,string pageName, WikiPage wikiPage)
=======
        public static Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
>>>>>>> master
        {
            return delegate
            {
<<<<<<< HEAD
                return redmineManager.CreateOrUpdateWikiPage(projectId, pageName, wikiPage);
=======
                var uri = UrlHelper.GetWikiCreateOrUpdaterUrl(redmineManager, projectId, pageName);
                var data = RedmineSerializer.Serialize(wikiPage, redmineManager.MimeFormat);

                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var response = wc.UploadString(uri, RedmineManager.PUT, data);
                    return RedmineSerializer.Deserialize<WikiPage>(response, redmineManager.MimeFormat);
                }
>>>>>>> master
            };
        }

        public static Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            return delegate
            {
<<<<<<< HEAD
                redmineManager.DeleteWikiPage(projectId, pageName);
            };
        }

        public static Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId,NameValueCollection parameters, string pageName, uint version = 0)
=======
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
            };
        }

        public static Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId, NameValueCollection parameters, string pageName, uint version = 0)
>>>>>>> master
        {
            return delegate
            {
                return redmineManager.GetWikiPage(projectId, parameters, pageName, version);
            };
        }

<<<<<<< HEAD
        public static Task<IList<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager,NameValueCollection parameters, string projectId)
=======
        public static Task<List<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, NameValueCollection parameters, string projectId)
>>>>>>> master
        {
            return delegate
            {
                return redmineManager.GetAllWikiPages(projectId);
            };
        }

        public static Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
<<<<<<< HEAD
            return delegate
            {
                redmineManager.AddUserToGroup(groupId, userId);
=======
            var data = redmineManager.MimeFormat == MimeFormat.xml
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}";
            Task task = delegate
            {
                var uri = UrlHelper.GetAddUserToGroupUrl(redmineManager, groupId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.POST, data);
                }
>>>>>>> master
            };
        }

        public static Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate
            {
<<<<<<< HEAD
                redmineManager.RemoveUserFromGroup(groupId, userId);
=======
                var uri = UrlHelper.GetRemoveUserFromGroupUrl(redmineManager, groupId, userId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
>>>>>>> master
            };
        }

        public static Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
<<<<<<< HEAD
            return delegate
            {
                redmineManager.AddWatcherToIssue(issueId, userId);
=======
            var data = redmineManager.MimeFormat == MimeFormat.xml
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}";
            Task task = delegate
            {
                var uri = UrlHelper.GetAddWatcherUrl(redmineManager, issueId, userId);

                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.POST, data);
                }
>>>>>>> master
            };
        }

        public static Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate
            {
<<<<<<< HEAD
                redmineManager.RemoveWatcherFromIssue(issueId, userId);
=======
                var uri = UrlHelper.GetRemoveWatcherUrl(redmineManager, issueId, userId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
>>>>>>> master
            };
        }

<<<<<<< HEAD
        public static Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id,NameValueCollection parameters) where T : class, new()
=======
        public static Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters) where T : class, new()
>>>>>>> master
        {
            return delegate
            {
                return redmineManager.GetObject<T>(id, parameters);
            };
        }

        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj) where T : class, new()
        {
            return CreateObjectAsync(redmineManager, obj, null);
        }

        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj, string ownerId) where T : class, new()
        {
            return delegate
            {
<<<<<<< HEAD
                return redmineManager.CreateObject(obj, ownerId);
=======
                var url = UrlHelper.GetCreateUrl<T>(redmineManager, ownerId);
                var data = RedmineSerializer.Serialize(obj, redmineManager.MimeFormat);

                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var response = wc.UploadString(url, RedmineManager.POST, data);
                    return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
                }
>>>>>>> master
            };
        }

<<<<<<< HEAD
        public static Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager,NameValueCollection parameters) where T : class, new()
=======
        public static Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
>>>>>>> master
        {
            return delegate
            {
                return redmineManager.GetPaginatedObjects<T>(parameters);
            };
        }

<<<<<<< HEAD
        public static Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
=======
        //public static Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        //{
        //    Task<List<T>> task = delegate
        //    {
        //        int totalCount = 0, pageSize;
        //        List<T> resultList = null;
        //        if (parameters == null) parameters = new NameValueCollection();
        //        int offset = 0;
        //        int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
        //        if (pageSize == default(int))
        //        {
        //            pageSize = redmineManager.PageSize > 0 ? redmineManager.PageSize : 25;
        //            parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
        //        }
        //        do
        //        {
        //            parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));
        //            var requestTask = redmineManager.GetPaginatedObjectsAsync<T>(parameters).ContinueWith(t =>
        //            {
        //                if (t.Result != null)
        //                {
        //                    if (resultList == null)
        //                    {
        //                        resultList = t.Result.Objects;
        //                        totalCount = t.Result.TotalCount;
        //                    }
        //                    else
        //                        resultList.AddRange(t.Result.Objects);
        //                }
        //                offset += pageSize;
        //            });
        //            requestTask.Wait(TimeSpan.FromMilliseconds(5000));
        //        } while (offset < totalCount);
        //        return resultList;
        //    });
        //    return task;
        //}

        public static Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj, string projectId = null) where T : class, new()
>>>>>>> master
        {
            return delegate
            {
<<<<<<< HEAD
                return redmineManager.GetObjects<T>(parameters);
=======
                var url = UrlHelper.GetUploadUrl(redmineManager, id, obj, projectId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var data = RedmineSerializer.Serialize(obj, redmineManager.MimeFormat);
                    wc.UploadString(url, RedmineManager.PUT, data);
                }
>>>>>>> master
            };
        }

<<<<<<< HEAD
        public static Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj, string projectId = null) where T : class, new()
        {
            return delegate { redmineManager.UpdateObject(id, obj, projectId); };
        }

=======
>>>>>>> master
        public static Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters) where T : class, new()
        {
            Task task = delegate
            {
<<<<<<< HEAD
                redmineManager.DeleteObject<T>(id, parameters);
=======
                var uri = UrlHelper.GetDeleteUrl<T>(redmineManager, id);

                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
>>>>>>> master
            };
            return task;
        }

        public static Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            return delegate
            {
<<<<<<< HEAD
                return redmineManager.UploadFile(data);
=======
                var uri = UrlHelper.GetUploadFileUrl(redmineManager);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var response = wc.UploadData(uri, RedmineManager.POST, data);

                    var responseString = Encoding.ASCII.GetString(response);
                    return RedmineSerializer.Deserialize<Upload>(responseString, redmineManager.MimeFormat);
                }
>>>>>>> master
            };
        }

        public static Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            return delegate
            {
                return redmineManager.DownloadFile(address);
            };
        }
    }
}