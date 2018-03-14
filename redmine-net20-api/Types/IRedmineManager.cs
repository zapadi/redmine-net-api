/*
   Copyright 2011 - 2017 Adrian Popescu.

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

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Redmine.Net.Api.Types
{
     interface IRedmineManager
    {
        int PageSize { get; set; }
        string ImpersonateUser { get; set; }

        User GetCurrentUser(NameValueCollection parameters = null);
      
        void AddUserToGroup(int groupId, int userId);
        void RemoveUserFromGroup(int groupId, int userId);
        void AddWatcherToIssue(int issueId, int userId);
        void RemoveWatcherFromIssue(int issueId, int userId);
        WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0);
        IList<WikiPage> GetAllWikiPages(string projectId);
        WikiPage CreateOrUpdateWikiPage(string projectId, string pageName, WikiPage wikiPage);
        void DeleteWikiPage(string projectId, string pageName);
        Upload UploadFile(byte[] data);
        byte[] DownloadFile(string address);
        List<T> GetObjectList<T>(NameValueCollection parameters) where T : class, new();
        List<T> GetObjectList<T>(NameValueCollection parameters, out int totalCount) where T : class, new();
        List<T> GetTotalObjectList<T>(NameValueCollection parameters) where T : class, new();
		List<T> GetObjects<T> (NameValueCollection parameters) where T: class, new();
		PaginatedObjects<T> GetPaginatedObjects<T> (NameValueCollection parameters)where T : class, new();
        T GetObject<T>(string id, NameValueCollection parameters) where T : class, new();
        
        T CreateObject<T>(T obj, string ownerId = null) where T : class, new();
        void UpdateObject<T>(string id, T obj) where T : class, new();
        void UpdateObject<T>(string id, T obj, string projectId) where T : class, new();
		void DeleteObject<T>(string id, NameValueCollection parameters) where T : class, new();
    }
}