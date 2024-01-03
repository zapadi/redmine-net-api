/*
   Copyright 2011 - 2023 Adrian Popescu

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
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net.WebClient;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
{
    /// <summary>
    ///     The main class to access Redmine API.
    /// </summary>
    public partial class RedmineManager
    {
        /// <summary>
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Use RedmineConstants.DEFAULT_PAGE_SIZE")]public const int DEFAULT_PAGE_SIZE_VALUE = 25;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtocolType.
        /// Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <param name="scheme">http or https. Default is https.</param>
        /// <param name="timeout">The webclient timeout. Default is 100 seconds.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Use RedmineManager(RedmineManagerOptionsBuilder")]
        public RedmineManager(string host, MimeFormat mimeFormat = MimeFormat.Xml, bool verifyServerCert = true,
            IWebProxy proxy = null, SecurityProtocolType securityProtocolType = default, string scheme = "https", TimeSpan? timeout = null)
        :this(new RedmineManagerOptionsBuilder()
            .WithHost(host)
            .WithAuthentication(new RedmineNoAuthentication())
            .WithSerializationType(mimeFormat)
            .WithVerifyServerCert(verifyServerCert)
            .WithClientOptions(new RedmineWebClientOptions()
            {
                Proxy = proxy,
                Scheme = scheme,
                Timeout = timeout,
                SecurityProtocolType = securityProtocolType
            })
        ) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class using your API key for authentication.
        /// </summary>
        /// <remarks>
        /// To enable the API-style authentication, you have to check Enable REST API in Administration -&amp;gt; Settings -&amp;gt; Authentication.
        /// You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the default layout.
        /// </remarks>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtocolType.
        /// Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <param name="scheme"></param>
        /// <param name="timeout">The webclient timeout. Default is 100 seconds.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Use RedmineManager(RedmineManagerOptionsBuilder")]
        public RedmineManager(string host, string apiKey, MimeFormat mimeFormat = MimeFormat.Xml,
                              bool verifyServerCert = true, IWebProxy proxy = null,
                              SecurityProtocolType securityProtocolType = default, string scheme = "https", TimeSpan? timeout = null)
            : this(new RedmineManagerOptionsBuilder()
                   .WithHost(host)
                   .WithAuthentication(new RedmineApiKeyAuthentication(apiKey))
                   .WithSerializationType(mimeFormat)
                   .WithVerifyServerCert(verifyServerCert)
                   .WithClientOptions(new RedmineWebClientOptions()
                   {
                       Proxy = proxy,
                       Scheme = scheme,
                       Timeout = timeout,
                       SecurityProtocolType = securityProtocolType
                   })){}

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager" /> class using your login and password for authentication.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtocolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <param name="scheme"></param>
        /// <param name="timeout">The webclient timeout. Default is 100 seconds.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Use RedmineManager(RedmineManagerOptionsBuilder")]
        public RedmineManager(string host, string login, string password, MimeFormat mimeFormat = MimeFormat.Xml,
                              bool verifyServerCert = true, IWebProxy proxy = null,
                              SecurityProtocolType securityProtocolType = default, string scheme = "https", TimeSpan? timeout = null)
            : this(new RedmineManagerOptionsBuilder()
                   .WithHost(host)
                   .WithAuthentication(new RedmineBasicAuthentication(login, password))
                   .WithSerializationType(mimeFormat)
                   .WithVerifyServerCert(verifyServerCert)
                   .WithClientOptions(new RedmineWebClientOptions()
                   {
                       Proxy = proxy,
                       Scheme = scheme,
                       Timeout = timeout,
                       SecurityProtocolType = securityProtocolType
                   })) {}
        
        #region Obsolete
        /// <summary>
        ///     Gets the suffixes.
        /// </summary>
        /// <value>
        ///     The suffixes.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public static Dictionary<Type, string> Suffixes => null;

        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public string Format { get; }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public string Scheme { get; }
        
        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public TimeSpan? Timeout { get; }

        /// <summary>
        ///     Gets the host.
        /// </summary>
        /// <value>
        ///     The host.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public string Host { get; }
        
        /// <summary>
        ///     The ApiKey used to authenticate.
        /// </summary>
        /// <value>
        ///     The API key.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public string ApiKey { get; }

        /// <summary>
        ///     Gets the MIME format.
        /// </summary>
        /// <value>
        ///     The MIME format.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public MimeFormat MimeFormat { get; }

        /// <summary>
        ///     Gets the proxy.
        /// </summary>
        /// <value>
        ///     The proxy.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public IWebProxy Proxy { get; }

        /// <summary>
        ///     Gets the type of the security protocol.
        /// </summary>
        /// <value>
        ///     The type of the security protocol.
        /// </value>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT)]public SecurityProtocolType SecurityProtocolType { get; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Returns null")]
        public static readonly Dictionary<Type, bool> TypesWithOffset = null;
        
        /// <summary>
        ///     Returns the user whose credentials are used to access the API.
        /// </summary>
        /// <param name="parameters">The accepted parameters are: memberships and groups (added in 2.1).</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        ///     An error occurred during deserialization. The original exception is available
        ///     using the System.Exception.InnerException property.
        /// </exception>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use GetCurrentUser extension instead")]
        public User GetCurrentUser(NameValueCollection parameters = null)
        {
            return this.GetCurrentUser(RedmineManagerExtensions.CreateRequestOptions(parameters));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the my account details.</returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use GetMyAccount extension instead")]
        public MyAccount GetMyAccount()
        {
            return RedmineManagerExtensions.GetMyAccount(this);
        }
        
        /// <summary>
        ///     Adds the watcher to issue.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use AddWatcherToIssue extension instead")]
        public void AddWatcherToIssue(int issueId, int userId)
        {
            RedmineManagerExtensions.AddWatcherToIssue(this, issueId, userId);
        }

        /// <summary>
        ///     Removes the watcher from issue.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use RemoveWatcherFromIssue extension instead")]
        public void RemoveWatcherFromIssue(int issueId, int userId)
        {
            RedmineManagerExtensions.RemoveWatcherFromIssue(this, issueId, userId);
        }

        /// <summary>
        ///     Adds an existing user to a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use AddUserToGroup extension instead")]
        public void AddUserToGroup(int groupId, int userId)
        {
            RedmineManagerExtensions.AddUserToGroup(this, groupId, userId);
        }

        /// <summary>
        ///     Removes an user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use RemoveUserFromGroup extension instead")]
        public void RemoveUserFromGroup(int groupId, int userId)
        {
            RedmineManagerExtensions.RemoveUserFromGroup(this, groupId, userId);
        }

        /// <summary>
        ///     Creates or updates a wiki page.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="wikiPage">The wiki page to create or update.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use UpdateWikiPage extension instead")]
        public void UpdateWikiPage(string projectId, string pageName, WikiPage wikiPage)
        {
            RedmineManagerExtensions.UpdateWikiPage(this, projectId, pageName, wikiPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use CreateWikiPage extension instead")]
        public WikiPage CreateWikiPage(string projectId, string pageName, WikiPage wikiPage)
        {
            return RedmineManagerExtensions.CreateWikiPage(this, projectId, pageName, wikiPage);
        }

        /// <summary>
        ///     Gets the wiki page.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use GetWikiPage extension instead")]
        public WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            return this.GetWikiPage(projectId, pageName, RedmineManagerExtensions.CreateRequestOptions(parameters), version);
        }

        /// <summary>
        ///     Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <returns></returns>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use GetAllWikiPages extension instead")]
        public List<WikiPage> GetAllWikiPages(string projectId)
        {
            return RedmineManagerExtensions.GetAllWikiPages(this, projectId);
        }

        /// <summary>
        ///     Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not
        ///     deleted but changed as root pages.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use DeleteWikiPage extension instead")] 
        public void DeleteWikiPage(string projectId, string pageName)
        {
            RedmineManagerExtensions.DeleteWikiPage(this, projectId, pageName);
        }
        
        /// <summary>
        ///     Updates the attachment.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="attachment">The attachment.</param>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use UpdateAttachment extension instead")] 
        public void UpdateAttachment(int issueId, Attachment attachment)
        {
            this.UpdateIssueAttachment(issueId, attachment);
        }
        
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
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use Search extension instead")] 
        public PagedResults<Search> Search(string q, int limit = DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null)
        {
            return RedmineManagerExtensions.Search(this, q, limit, offset, searchFilter);
        }
        
        /// <summary>
        ///     Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="uploadFile">if set to <c>true</c> [upload file].</param>
        /// <returns></returns>
        /// <code></code>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "If a custom webClient is needed, use Func<WebClient> from RedmineManagerSettings instead")]
        public virtual RedmineWebClient CreateWebClient(NameValueCollection parameters, bool uploadFile = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     This is to take care of SSL certification validation which are not issued by Trusted Root CA.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="cert">The cert.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The error.</param>
        /// <returns></returns>
        /// <code></code>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + "Use WebClientSettings.ServerCertificateValidationCallback instead")]
        public virtual bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            const SslPolicyErrors IGNORED_ERRORS = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch;
 
            return (sslPolicyErrors & ~IGNORED_ERRORS) == SslPolicyErrors.None;
        }
    }
}