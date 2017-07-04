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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    public static class RedmineManagerAsyncExtensions
    {
        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, NameValueCollection parameters = null)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetCurrentUserUrl(redmineManager);

                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    return wc.DownloadString(uri);
                }
            });

            return task.ContinueWith(t => RedmineSerializer.Deserialize<User>(t.Result, redmineManager.MimeFormat));
        }

        public static Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName, WikiPage wikiPage)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetWikiCreateOrUpdaterUrl(redmineManager, projectId, pageName);
                var data = RedmineSerializer.Serialize(wikiPage,redmineManager.MimeFormat);

                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var response = wc.UploadString(uri, RedmineManager.PUT, data);
                    return RedmineSerializer.Deserialize<WikiPage>(response, redmineManager.MimeFormat);
                }
            }, TaskCreationOptions.LongRunning);

            return task;
        }

        public static Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            var uri = UrlHelper.GetDeleteWikirUrl(redmineManager, projectId, pageName);
            return Task.Factory.StartNew(() =>
            {
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public static Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager, string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetWikiPageUrl(redmineManager, projectId, parameters, pageName, version);
                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    try
                    {
                        var response = wc.DownloadString(uri);
                        return RedmineSerializer.Deserialize<WikiPage>(response, redmineManager.MimeFormat);
                    }
                    catch (WebException wex)
                    {
                        wex.HandleWebException("GetWikiPageAsync", redmineManager.MimeFormat);
                    }
                    return null; 
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task<PaginatedObjects<WikiPage>> GetAllWikiPagesAsync(this RedmineManager redmineManager, NameValueCollection parameters, string projectId)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetWikisUrl(redmineManager, projectId);
                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    var response = wc.DownloadString(uri);
                    return RedmineSerializer.DeserializeList<WikiPage>(response, redmineManager.MimeFormat);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var data = redmineManager.MimeFormat == MimeFormat.xml
                   ? "<user_id>" + userId + "</user_id>"
                   : "{\"user_id\":\"" + userId + "\"}";
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetAddUserToGroupUrl(redmineManager, groupId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.POST, data);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetRemoveUserFromGroupUrl(redmineManager, groupId, userId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var data = redmineManager.MimeFormat == MimeFormat.xml
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}";
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetAddWatcherUrl(redmineManager, issueId, userId);

                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.POST, data);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetRemoveWatcherUrl(redmineManager, issueId, userId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task<T> GetObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters) where T : class, new()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var url = UrlHelper.GetGetUrl<T>(redmineManager, id);
                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    try
                    {
                        var response = wc.DownloadString(url);
                        return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
                    }
                    catch (WebException wex)
                    {
                        wex.HandleWebException("GetObject", redmineManager.MimeFormat);
                    }
                    return null; 
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj) where T : class, new()
        {
            return CreateObjectAsync(redmineManager, obj, null);
        }

        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj, string ownerId) where T : class, new()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var url = UrlHelper.GetCreateUrl<T>(redmineManager, ownerId);
                var data = RedmineSerializer.Serialize(obj,redmineManager.MimeFormat);

                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var response = wc.UploadString(url, RedmineManager.POST, data);
                    return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            var task = Task.Factory.StartNew(() =>
           {
               var url = UrlHelper.GetListUrl<T>(redmineManager, parameters);
               using (var wc = redmineManager.CreateWebClient(parameters))
               {
                   var response = wc.DownloadString(url);
                   return RedmineSerializer.DeserializeList<T>(response, redmineManager.MimeFormat);
               }
           }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task<List<T>> GetObjectsAsync<T>(this RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            var task = Task.Factory.StartNew(() =>
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
                   var requestTask = redmineManager.GetPaginatedObjectsAsync<T>(parameters).ContinueWith(t =>
                   {
                       if (t.Result != null)
                       {
                           if (resultList == null)
                           {
                               resultList = t.Result.Objects;
                               totalCount = t.Result.TotalCount;
                           }
                           else
                               resultList.AddRange(t.Result.Objects);
                       }
                       offset += pageSize;
                   });
                   requestTask.Wait(TimeSpan.FromMilliseconds(5000));
               } while (offset < totalCount);
               return resultList;
           });
            return task;
        }

        public static Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj, string projectId = null) where T : class, new()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var url = UrlHelper.GetUploadUrl(redmineManager, id, obj, projectId);
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    var data = RedmineSerializer.Serialize(obj,redmineManager.MimeFormat);
                    wc.UploadString(url, RedmineManager.PUT, data);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id, NameValueCollection parameters) where T : class, new()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetDeleteUrl<T>(redmineManager, id);

                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    wc.UploadString(uri, RedmineManager.DELETE, string.Empty);
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }

        public static Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var uri = UrlHelper.GetUploadFileUrl(redmineManager);
                using (var wc = redmineManager.CreateWebClient(null, true))
                {
                    var response = wc.UploadData(uri, RedmineManager.POST, data);

                    var responseString = Encoding.ASCII.GetString(response);
                    return RedmineSerializer.Deserialize<Upload>(responseString, redmineManager.MimeFormat);
                }
            }, TaskCreationOptions.LongRunning);

            return task;
        }

        public static Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            var task = Task.Factory.StartNew(() =>
            {
                using (var wc = redmineManager.CreateWebClient(null))
                {
                    wc.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
                    var response = wc.DownloadData(address);
                    return response;
                }
            }, TaskCreationOptions.LongRunning);
            return task;
        }
    }
}