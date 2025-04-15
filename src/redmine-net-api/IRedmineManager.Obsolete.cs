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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    public partial interface IRedmineManager
    {
        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        string Host { get; }
        
        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        string ApiKey { get; }
        
        /// <summary>
        ///     Maximum page-size when retrieving complete object lists
        ///     <remarks>
        ///         By default, only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set
        ///         in your Settings -&gt; General, "Objects per page options".By adding (for instance) 9999 there would make you
        ///         able to get that many results per request.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     The size of the page.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        int PageSize { get; set; }
        
        /// <summary>
        ///     As of Redmine 2.2.0 you can impersonate user setting user login (e.g. jsmith). This only works when using the API
        ///     with an administrator account, this header will be ignored when using the API with a regular user account.
        /// </summary>
        /// <value>
        ///     The impersonate user.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        string ImpersonateUser { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        MimeFormat MimeFormat { get; }
        
        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        IWebProxy Proxy { get; }
        
        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        SecurityProtocolType SecurityProtocolType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'GetCurrentUser' extension instead")]
        User GetCurrentUser(NameValueCollection parameters = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'AddUserToGroup' extension instead")]
        void AddUserToGroup(int groupId, int userId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'RemoveUserFromGroup' extension instead")]
        void RemoveUserFromGroup(int groupId, int userId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'AddWatcherToIssue' extension instead")]
        void AddWatcherToIssue(int issueId, int userId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'RemoveWatcherFromIssue' extension instead")]
        void RemoveWatcherFromIssue(int issueId, int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'CreateWikiPage' extension instead")]
        WikiPage CreateWikiPage(string projectId, string pageName, WikiPage wikiPage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'UpdateWikiPage' extension instead")]
        void UpdateWikiPage(string projectId, string pageName, WikiPage wikiPage);
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="parameters"></param>
        /// <param name="pageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'GetWikiPage' extension instead")]
        WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'GetAllWikiPages' extension instead")]
        List<WikiPage> GetAllWikiPages(string projectId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'DeleteWikiPage' extension instead")]
        void DeleteWikiPage(string projectId, string pageName);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="attachment"></param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'UpdateAttachment' extension instead")]
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
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Search' extension instead")]
        PagedResults<Search> Search(string q, int limit , int offset = 0, SearchFilterBuilder searchFilter = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'GetPaginated' method instead")]
        PagedResults<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="include"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Count' method instead")]
        int Count<T>(params string[] include) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Get' method instead")]
        T GetObject<T>(string id, NameValueCollection parameters) where T : class, new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Get' method instead")]
        List<T> GetObjects<T>(int limit, int offset, params string[] include) where T : class, new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Get' method instead")]
        List<T> GetObjects<T>(params string[] include) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Get' method instead")]
        List<T> GetObjects<T>(NameValueCollection parameters) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Create' method instead")]
        T CreateObject<T>(T entity) where T : class, new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ownerId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Create' method instead")]
        T CreateObject<T>(T entity, string ownerId) where T : class, new();
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="projectId"></param>
        /// <typeparam name="T"></typeparam>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Update' method instead")]
        void UpdateObject<T>(string id, T entity, string projectId = null) where T : class, new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use 'Delete' method instead")]
        void DeleteObject<T>(string id, NameValueCollection parameters = null) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        RedmineWebClient CreateWebClient(NameValueCollection parameters, bool uploadFile = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]
        bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors);
    }
}