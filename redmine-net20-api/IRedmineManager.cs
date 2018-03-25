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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api.Types
{
    public interface IRedmineManager
    {
        string Host { get; }
        string ApiKey { get; }
        int PageSize { get; set; }
        string ImpersonateUser { get; set; }
        MimeFormat MimeFormat { get; }
        IWebProxy Proxy { get; }
        SecurityProtocolType SecurityProtocolType { get; }

        User GetCurrentUser(NameValueCollection parameters = null);

        void AddUserToGroup(int groupId, int userId);
        void RemoveUserFromGroup(int groupId, int userId);
        
        void AddWatcherToIssue(int issueId, int userId);
        void RemoveWatcherFromIssue(int issueId, int userId);
        
        WikiPage CreateOrUpdateWikiPage(string projectId, string pageName, WikiPage wikiPage);
        WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0);
        List<WikiPage> GetAllWikiPages(string projectId);
        void DeleteWikiPage(string projectId, string pageName);
        
        Upload UploadFile(byte[] data);
        void UpdateAttachment(int issueId, Attachment attachment);
        byte[] DownloadFile(string address);
        
        PaginatedObjects<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new();
        
        T GetObject<T>(string id, NameValueCollection parameters) where T : class, new();
        List<T> GetObjects<T>(int limit, int offset, params string[] include) where T : class, new();
        List<T> GetObjects<T>(params string[] include) where T : class, new();

        List<T> GetObjects<T>(NameValueCollection parameters) where T : class, new();

        T CreateObject<T>(T obj) where T : class, new();
        T CreateObject<T>(T obj, string ownerId) where T : class, new();
       
        void UpdateObject<T>(string id, T obj) where T : class, new();
        void UpdateObject<T>(string id, T obj, string projectId) where T : class, new();
        
        void DeleteObject<T>(string id) where T : class, new();
        void DeleteObject<T>(string id, NameValueCollection parameters) where T : class, new();

        RedmineWebClient CreateWebClient(NameValueCollection parameters, bool uploadFile = false);
        bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error);
    }
}