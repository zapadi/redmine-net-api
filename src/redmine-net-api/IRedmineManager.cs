/*
   Copyright 2011 - 2022 Adrian Popescu

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
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedmineManager
    {
        /// <summary>
        /// 
        /// </summary>
        string Host { get; }
        /// <summary>
        /// 
        /// </summary>
        string ApiKey { get; }
        /// <summary>
        /// 
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string ImpersonateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        MimeFormat MimeFormat { get; }
        /// <summary>
        /// 
        /// </summary>
        IWebProxy Proxy { get; }
        /// <summary>
        /// 
        /// </summary>
        SecurityProtocolType SecurityProtocolType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        User GetCurrentUser(NameValueCollection parameters = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        void AddUserToGroup(int groupId, int userId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        void RemoveUserFromGroup(int groupId, int userId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        void AddWatcherToIssue(int issueId, int userId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        void RemoveWatcherFromIssue(int issueId, int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        WikiPage CreateWikiPage(string projectId, string pageName, WikiPage wikiPage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        void UpdateWikiPage(string projectId, string pageName, WikiPage wikiPage);
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="parameters"></param>
        /// <param name="pageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        List<WikiPage> GetAllWikiPages(string projectId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        void DeleteWikiPage(string projectId, string pageName);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Upload UploadFile(byte[] data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="attachment"></param>
        void UpdateAttachment(int issueId, Attachment attachment);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q">query strings. enable to specify multiple values separated by a space " ".</param>
        /// <param name="limit">number of results in response.</param>
        /// <param name="offset">skip this number of results in response</param>
        /// <param name="searchFilter">Optional filters.</param>
        /// <returns>
        /// Returns the search results by the specified condition parameters.
        /// </returns>
        PagedResults<Search> Search(string q, int limit , int offset = 0,
            SearchFilterBuilder searchFilter = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        byte[] DownloadFile(string address);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        PagedResults<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="include"></param>
        /// <returns></returns>
        int Count<T>(params string[] include) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int Count<T>(NameValueCollection parameters) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetObject<T>(string id, NameValueCollection parameters) where T : class, new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetObjects<T>(int limit, int offset, params string[] include) where T : class, new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetObjects<T>(params string[] include) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetObjects<T>(NameValueCollection parameters) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CreateObject<T>(T entity) where T : class, new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ownerId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CreateObject<T>(T entity, string ownerId) where T : class, new();
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        void UpdateObject<T>(string id, T entity) where T : class, new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="projectId"></param>
        /// <typeparam name="T"></typeparam>
        void UpdateObject<T>(string id, T entity, string projectId) where T : class, new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        void DeleteObject<T>(string id, NameValueCollection parameters) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        RedmineWebClient CreateWebClient(NameValueCollection parameters, bool uploadFile = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors);
    }
}