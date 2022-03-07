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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Group = Redmine.Net.Api.Types.Group;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api
{
    /// <summary>
    ///     The main class to access Redmine API.
    /// </summary>
    public class RedmineManager : IRedmineManager
    {
        /// <summary>
        /// </summary>
        public const int DEFAULT_PAGE_SIZE_VALUE = 25;

        private static readonly Dictionary<Type, string> Routes = new Dictionary<Type, string>
        {
            {typeof(Issue), "issues"},
            {typeof(Project), "projects"},
            {typeof(User), "users"},
            {typeof(News), "news"},
            {typeof(Query), "queries"},
            {typeof(Version), "versions"},
            {typeof(Attachment), "attachments"},
            {typeof(IssueRelation), "relations"},
            {typeof(TimeEntry), "time_entries"},
            {typeof(IssueStatus), "issue_statuses"},
            {typeof(Tracker), "trackers"},
            {typeof(IssueCategory), "issue_categories"},
            {typeof(Role), "roles"},
            {typeof(ProjectMembership), "memberships"},
            {typeof(Group), "groups"},
            {typeof(TimeEntryActivity), "enumerations/time_entry_activities"},
            {typeof(IssuePriority), "enumerations/issue_priorities"},
            {typeof(Watcher), "watchers"},
            {typeof(IssueCustomField), "custom_fields"},
            {typeof(CustomField), "custom_fields"},
            {typeof(Search), "search"}
        };

        /// <summary>
        /// 
        /// </summary>
        public static readonly Dictionary<Type, bool> TypesWithOffset = new Dictionary<Type, bool>{
            {typeof(Issue), true},
            {typeof(Project), true},
            {typeof(User), true},
            {typeof(News), true},
            {typeof(Query), true},
            {typeof(TimeEntry), true},
            {typeof(ProjectMembership), true},
            {typeof(Search), true}
        };

        private readonly string basicAuthorization;
        private readonly CredentialCache cache;
        private string host;

        internal IRedmineSerializer Serializer { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtcolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <param name="scheme">http or https. Default is https.</param>
        /// <param name="timeout">The webclient timeout. Default is 100 seconds.</param>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        ///     Host is not defined!
        ///     or
        ///     The host is not valid!
        /// </exception>
        public RedmineManager(string host, MimeFormat mimeFormat = MimeFormat.Xml, bool verifyServerCert = true,
            IWebProxy proxy = null, SecurityProtocolType securityProtocolType = default, string scheme = "https", TimeSpan? timeout = null)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new RedmineException("Host is not defined!");
            }

            PageSize = 25;
            Scheme = scheme;
            Host = host;
            MimeFormat = mimeFormat;
            Timeout = timeout;
            Proxy = proxy;

            if (mimeFormat == MimeFormat.Xml)
            {
                Format = "xml";
                Serializer = new XmlRedmineSerializer();
            }
            else
            {
                Format = "json";
                Serializer = new JsonRedmineSerializer();
            }

            if (securityProtocolType == default)
            {
                securityProtocolType = ServicePointManager.SecurityProtocol;
            }

            SecurityProtocolType = securityProtocolType;

            ServicePointManager.SecurityProtocol = securityProtocolType;

            if (!verifyServerCert)
            {
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertValidate;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        ///     Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable
        ///     REST API in Administration -&gt; Settings -&gt; Authentication. Then, authentication can be done in 2 different
        ///     ways:
        ///     using your regular login/password via HTTP Basic authentication.
        ///     using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to
        ///     each request in one of the following way:
        ///     passed in as a "key" parameter
        ///     passed in as a username with a random password via HTTP Basic authentication
        ///     passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
        ///     You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the
        ///     default layout.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtcolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <param name="timeout">The webclient timeout. Default is 100 seconds.</param>
        public RedmineManager(string host, string apiKey, MimeFormat mimeFormat = MimeFormat.Xml,
            bool verifyServerCert = true, IWebProxy proxy = null,
            SecurityProtocolType securityProtocolType = default, TimeSpan? timeout = null)
            : this(host, mimeFormat, verifyServerCert, proxy, securityProtocolType, timeout: timeout)
        {
            ApiKey = apiKey;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        ///     Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable
        ///     REST API in Administration -&gt; Settings -&gt; Authentication. Then, authentication can be done in 2 different
        ///     ways:
        ///     using your regular login/password via HTTP Basic authentication.
        ///     using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to
        ///     each request in one of the following way:
        ///     passed in as a "key" parameter
        ///     passed in as a username with a random password via HTTP Basic authentication
        ///     passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
        ///     You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the
        ///     default layout.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtcolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <param name="timeout">The webclient timeout. Default is 100 seconds.</param>
        public RedmineManager(string host, string login, string password, MimeFormat mimeFormat = MimeFormat.Xml,
            bool verifyServerCert = true, IWebProxy proxy = null,
            SecurityProtocolType securityProtocolType = default, TimeSpan? timeout = null)
            : this(host, mimeFormat, verifyServerCert, proxy, securityProtocolType, timeout: timeout)
        {
            cache = new CredentialCache { { new Uri(host), "Basic", new NetworkCredential(login, password) } };

            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", login, password)));
            basicAuthorization = string.Format(CultureInfo.InvariantCulture, "Basic {0}", token);
        }

        /// <summary>
        ///     Gets the suffixes.
        /// </summary>
        /// <value>
        ///     The suffixes.
        /// </value>
        public static Dictionary<Type, string> Suffixes => Routes;

        /// <summary>
        /// 
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Scheme { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? Timeout { get; private set; }

        /// <summary>
        ///     Gets the host.
        /// </summary>
        /// <value>
        ///     The host.
        /// </value>
        public string Host
        {
            get => host;
            private set
            {
                host = value;

                if (Uri.TryCreate(host, UriKind.Absolute, out Uri uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    return;
                }

                host = $"{Scheme ?? "https"}://{host}";

                if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult)) throw new RedmineException("The host is not valid!");

                Scheme = uriResult.Scheme;
            }
        }

        /// <summary>
        ///     The ApiKey used to authenticate.
        /// </summary>
        /// <value>
        ///     The API key.
        /// </value>
        public string ApiKey { get; }

        /// <summary>
        ///     Maximum page-size when retrieving complete object lists
        ///     <remarks>
        ///         By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set
        ///         in your Settings -&gt; General, "Objects per page options".By adding (for instance) 9999 there would make you
        ///         able to get that many results per request.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        ///     As of Redmine 2.2.0 you can impersonate user setting user login (eg. jsmith). This only works when using the API
        ///     with an administrator account, this header will be ignored when using the API with a regular user account.
        /// </summary>
        /// <value>
        ///     The impersonate user.
        /// </value>
        public string ImpersonateUser { get; set; }

        /// <summary>
        ///     Gets the MIME format.
        /// </summary>
        /// <value>
        ///     The MIME format.
        /// </value>
        public MimeFormat MimeFormat { get; }

        /// <summary>
        ///     Gets the proxy.
        /// </summary>
        /// <value>
        ///     The proxy.
        /// </value>
        public IWebProxy Proxy { get; }

        /// <summary>
        ///     Gets the type of the security protocol.
        /// </summary>
        /// <value>
        ///     The type of the security protocol.
        /// </value>
        public SecurityProtocolType SecurityProtocolType { get; }

        /// <summary>
        ///     Returns the user whose credentials are used to access the API.
        /// </summary>
        /// <param name="parameters">The accepted parameters are: memberships and groups (added in 2.1).</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        ///     An error occurred during deserialization. The original exception is available
        ///     using the System.Exception.InnerException property.
        /// </exception>
        public User GetCurrentUser(NameValueCollection parameters = null)
        {
            var url = UrlHelper.GetCurrentUserUrl(this);
            return WebApiHelper.ExecuteDownload<User>(this, url, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the my account details.</returns>
        public MyAccount GetMyAccount()
        {
            var url = UrlHelper.GetMyAccountUrl(this);
            return WebApiHelper.ExecuteDownload<MyAccount>(this, url);
        }
        
        /// <summary>
        ///     Adds the watcher to issue.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void AddWatcherToIssue(int issueId, int userId)
        {
            var url = UrlHelper.GetAddWatcherUrl(this, issueId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.POST, DataHelper.UserData(userId, MimeFormat));
        }

        /// <summary>
        ///     Removes the watcher from issue.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void RemoveWatcherFromIssue(int issueId, int userId)
        {
            var url = UrlHelper.GetRemoveWatcherUrl(this, issueId, userId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty);
        }

        /// <summary>
        ///     Adds an existing user to a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void AddUserToGroup(int groupId, int userId)
        {
            var url = UrlHelper.GetAddUserToGroupUrl(this, groupId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.POST, DataHelper.UserData(userId, MimeFormat));
        }

        /// <summary>
        ///     Removes an user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void RemoveUserFromGroup(int groupId, int userId)
        {
            var url = UrlHelper.GetRemoveUserFromGroupUrl(this, groupId, userId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty);
        }

        /// <summary>
        ///     Creates or updates a wiki page.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="wikiPage">The wiki page to create or update.</param>
        /// <returns></returns>
        public void UpdateWikiPage(string projectId, string pageName, WikiPage wikiPage)
        {
            var result = Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(result))
            {
                return;
            }

            var url = UrlHelper.GetWikiCreateOrUpdaterUrl(this, projectId, pageName);

            url = Uri.EscapeUriString(url);

            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.PUT, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        public WikiPage CreateWikiPage(string projectId, string pageName, WikiPage wikiPage)
        {
            var result = Serializer.Serialize(wikiPage);

            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            var url = UrlHelper.GetWikiCreateOrUpdaterUrl(this, projectId, pageName);

            url = Uri.EscapeUriString(url);

            return WebApiHelper.ExecuteUpload<WikiPage>(this, url, HttpVerbs.PUT, result);
        }

        /// <summary>
        /// Gets the wiki page.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            var url = UrlHelper.GetWikiPageUrl(this, projectId, pageName, version);

            url = Uri.EscapeUriString(url);

            return WebApiHelper.ExecuteDownload<WikiPage>(this, url, parameters);
        }

        /// <summary>
        ///     Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <returns></returns>
        public List<WikiPage> GetAllWikiPages(string projectId)
        {
            var url = UrlHelper.GetWikisUrl(this, projectId);

            var result = WebApiHelper.ExecuteDownloadList<WikiPage>(this, url);

            return result == null ? null : new List<WikiPage>(result.Items);
        }

        /// <summary>
        ///     Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not
        ///     deleted but changed as root pages.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        public void DeleteWikiPage(string projectId, string pageName)
        {
            var url = UrlHelper.GetDeleteWikiUrl(this, projectId, pageName);

            url = Uri.EscapeUriString(url);

            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Count<T>(NameValueCollection parameters) where T : class, new()
        {
            int totalCount = 0, pageSize = 1, offset = 0;

            if (parameters == null)
            {
                parameters = new NameValueCollection();
            }

            parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

            try
            {
                var tempResult = GetPaginatedObjects<T>(parameters);

                if (tempResult != null)
                {
                    totalCount = tempResult.TotalItems;
                }
            }
            catch (WebException wex)
            {
                wex.HandleWebException(Serializer);
            }

            return totalCount;
        }

        /// <summary>
        ///     Gets the redmine object based on id.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>
        ///     Returns the object of type T.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     An error occurred during deserialization. The original exception is available
        ///     using the System.Exception.InnerException property.
        /// </exception>
        /// <code>
        ///   <example>
        ///         string issueId = "927";
        ///         NameValueCollection parameters = null;
        ///         Issue issue = redmineManager.GetObject&lt;Issue&gt;(issueId, parameters);
        ///     </example>
        /// </code>
        public T GetObject<T>(string id, NameValueCollection parameters) where T : class, new()
        {
            var url = UrlHelper.GetGetUrl<T>(this, id);
            return WebApiHelper.ExecuteDownload<T>(this, url, parameters);
        }

        /// <summary>
        ///     Gets the paginated objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public PagedResults<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            var url = UrlHelper.GetListUrl<T>(this, parameters);
            return WebApiHelper.ExecuteDownloadList<T>(this, url, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="include"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Count<T>(params string[] include) where T : class, new()
        {
            var parameters = new NameValueCollection();

            if (include != null && include.Length > 0)
            {
                parameters.Add(RedmineKeys.INCLUDE, string.Join(",", include));
            }

            return Count<T>(parameters);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">The page size.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="include">Optional fetched data.</param>
        /// <remarks>
        /// Optional fetched data:
        ///     Project: trackers, issue_categories, enabled_modules (since 2.6.0)
        ///     Issue: children, attachments, relations, changesets, journals, watchers - Since 2.3.0
        ///     Users: memberships, groups (added in 2.1)
        ///     Groups: users, memberships
        /// </remarks>
        /// <returns>Returns the complete list of objects.</returns>
        public List<T> GetObjects<T>(int limit, int offset, params string[] include) where T : class, new()
        {
            var parameters = new NameValueCollection();

            parameters.Add(RedmineKeys.LIMIT, limit.ToString(CultureInfo.InvariantCulture));
            parameters.Add(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

            if (include != null && include.Length > 0)
            {
                parameters.Add(RedmineKeys.INCLUDE, string.Join(",", include));
            }

            return GetObjects<T>(parameters);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="include">Optional fetched data.</param>
        /// <remarks>
        /// Optional fetched data:
        ///     Project: trackers, issue_categories, enabled_modules (since 2.6.0)
        ///     Issue: children, attachments, relations, changesets, journals, watchers - Since 2.3.0
        ///     Users: memberships, groups (added in 2.1)
        ///     Groups: users, memberships
        /// </remarks>
        /// <returns>Returns the complete list of objects.</returns>
        public List<T> GetObjects<T>(params string[] include) where T : class, new()
        {
            var parameters = new NameValueCollection();

            if (include != null && include.Length > 0)
            {
                parameters.Add(RedmineKeys.INCLUDE, string.Join(",", include));
            }

            return GetObjects<T>(parameters);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>
        ///     Returns a complete list of objects.
        /// </returns>
        public List<T> GetObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            int pageSize = 0, offset = 0;
            var isLimitSet = false;
            List<T> resultList = null;

            if (parameters == null)
            {
                parameters = new NameValueCollection();
            }
            else
            {
                isLimitSet = int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
                int.TryParse(parameters[RedmineKeys.OFFSET], out offset);
            }
            if (pageSize == default)
            {
                pageSize = PageSize > 0 ? PageSize : DEFAULT_PAGE_SIZE_VALUE;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }

            try
            {
                var hasOffset = TypesWithOffset.ContainsKey(typeof(T));
                if (hasOffset)
                {
                    var totalCount = 0;
                    do
                    {
                        parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

                        var tempResult = GetPaginatedObjects<T>(parameters);

                        totalCount = isLimitSet ? pageSize : tempResult.TotalItems;

                        if (tempResult?.Items != null)
                        {
                            if (resultList == null)
                            {
                                resultList = new List<T>(tempResult.Items);
                            }
                            else
                            {
                                resultList.AddRange(tempResult.Items);
                            }
                        }

                        offset += pageSize;
                    }
                    while (offset < totalCount);
                }
                else
                {
                    var result = GetPaginatedObjects<T>(parameters);
                    if (result?.Items != null)
                    {
                        return new List<T>(result.Items);
                    }
                }
            }
            catch (WebException wex)
            {
                wex.HandleWebException(Serializer);
            }
            return resultList;
        }

        /// <summary>
        ///     Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="NotAcceptableException"></exception>
        /// <remarks>
        ///     When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable
        ///     Entity response. That means that the object could not be created.
        /// </remarks>
        public T CreateObject<T>(T obj) where T : class, new()
        {
            return CreateObject(obj, null);
        }

        /// <summary>
        ///     Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable
        ///     Entity response. That means that the object could not be created.
        /// </remarks>
        /// <code>
        ///   <example>
        ///         var project = new Project();
        ///         project.Name = "test";
        ///         project.Identifier = "the project identifier";
        ///         project.Description = "the project description";
        ///         redmineManager.CreateObject(project);
        ///     </example>
        /// </code>
        public T CreateObject<T>(T obj, string ownerId) where T : class, new()
        {
            var url = UrlHelper.GetCreateUrl<T>(this, ownerId);

            var data = Serializer.Serialize(obj);

            return WebApiHelper.ExecuteUpload<T>(this, url, HttpVerbs.POST, data);
        }

        /// <summary>
        ///     Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="obj">The object to be update.</param>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to update an object with invalid or missing attribute parameters, you will get a 422(RedmineException)
        ///     Unprocessable Entity response. That means that the object could not be updated.
        /// </remarks>
        /// <code></code>
        public void UpdateObject<T>(string id, T obj) where T : class, new()
        {
            UpdateObject(id, obj, null);
        }

        /// <summary>
        ///     Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="obj">The object to be update.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to update an object with invalid or missing attribute parameters, you will get a
        ///     422(RedmineException) Unprocessable Entity response. That means that the object could not be updated.
        /// </remarks>
        /// <code></code>
        public void UpdateObject<T>(string id, T obj, string projectId) where T : class, new()
        {
            var url = UrlHelper.GetUploadUrl<T>(this, id);

            var data = Serializer.Serialize(obj);

            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");

            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.PUT, data);
        }

        /// <summary>
        /// Deletes the Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="parameters">The parameters</param>
        /// <exception cref="RedmineException"></exception>
        /// <code></code>
        public void DeleteObject<T>(string id, NameValueCollection parameters = null) where T : class, new()
        {
            var url = UrlHelper.GetDeleteUrl<T>(this, id);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty, parameters);
        }

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server.
        /// </summary>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>
        ///     Returns the token for uploaded file.
        /// </returns>
        /// <exception cref="RedmineException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="NotAcceptableException"></exception>
        public Upload UploadFile(byte[] data)
        {
            var url = UrlHelper.GetUploadFileUrl(this);
            return WebApiHelper.ExecuteUploadFile(this, url, data);
        }

        /// <summary>
        ///     Updates the attachment.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="attachment">The attachment.</param>
        public void UpdateAttachment(int issueId, Attachment attachment)
        {
            var address = UrlHelper.GetAttachmentUpdateUrl(this, issueId);

            var attachments = new Attachments { { attachment.Id, attachment } };

            var data = Serializer.Serialize(attachments);

            WebApiHelper.ExecuteUpload(this, address, HttpVerbs.PATCH, data);
        }

        /// <summary>
        ///     Downloads the file.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="NotAcceptableException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public byte[] DownloadFile(string address)
        {
            return WebApiHelper.ExecuteDownloadFile(this, address);
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
        public PagedResults<Search> Search(string q, int limit = DEFAULT_PAGE_SIZE_VALUE, int offset = 0, SearchFilterBuilder searchFilter = null)
        {
            if (q.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(q));
            }

            var parameters = new NameValueCollection
            {
                {RedmineKeys.Q, q},
                {RedmineKeys.LIMIT, limit.ToString(CultureInfo.InvariantCulture)},
                {RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture)},
            };

            if (searchFilter != null)
            {
               parameters = searchFilter.Build(parameters);
            }
            
            var result = GetPaginatedObjects<Search>(parameters);

            return result;
        }
        
        private const string UA = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.163 Safari/535.1";

        /// <summary>
        ///     Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="uploadFile">if set to <c>true</c> [upload file].</param>
        /// <returns></returns>
        /// <code></code>
        public virtual RedmineWebClient CreateWebClient(NameValueCollection parameters, bool uploadFile = false)
        {
            var webClient = new RedmineWebClient { Scheme = Scheme, RedmineSerializer = Serializer};
            webClient.UserAgent = UA;
            webClient.Timeout = Timeout;
            if (!uploadFile)
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType,
                    MimeFormat is MimeFormat.Xml ? "application/xml" : "application/json");
                webClient.Encoding = Encoding.UTF8;
            }
            else
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
            }

            if (parameters != null)
            {
                webClient.QueryString = parameters;
            }

            if (!string.IsNullOrEmpty(ApiKey))
            {
                webClient.QueryString[RedmineKeys.KEY] = ApiKey;
            }
            else
            {
                if (cache != null)
                {
                    webClient.PreAuthenticate = true;
                    webClient.Credentials = cache;
                    webClient.Headers[HttpRequestHeader.Authorization] = basicAuthorization;
                }
                else
                {
                    webClient.UseDefaultCredentials = true;
                    webClient.Credentials = CredentialCache.DefaultCredentials;
                }
            }

            if (Proxy != null)
            {
                Proxy.Credentials = cache;
                webClient.Proxy = Proxy;
                webClient.UseProxy = true;
            }

            if (!string.IsNullOrEmpty(ImpersonateUser))
            {
                webClient.Headers.Add("X-Redmine-Switch-User", ImpersonateUser);
            }

            return webClient;
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
        public virtual bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            const SslPolicyErrors ignoredErrors =
                SslPolicyErrors.RemoteCertificateChainErrors |
                SslPolicyErrors.RemoteCertificateNameMismatch;
 
            if ((sslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }
    }
}