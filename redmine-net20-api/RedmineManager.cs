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
using System.Diagnostics;
using System.Globalization;
using System.IO;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Group = Redmine.Net.Api.Types.Group;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api
{
    /// <summary>
    /// The main class to access Redmine API.
    /// </summary>
	public partial class RedmineManager : IRedmineManager
    {

        public const string REQUEST_FORMAT = "{0}/{1}/{2}.xml";
		public const string FORMAT = "{0}/{1}.xml";

		public const string WIKI_INDEX_FORMAT = "{0}/projects/{1}/wiki/index.xml";
		public const string WIKI_PAGE_FORMAT = "{0}/projects/{1}/wiki/{2}.xml";
		public const string WIKI_VERSION_FORMAT = "{0}/projects/{1}/wiki/{2}/{3}.xml";

		public const string ENTITY_WITH_PARENT_FORMAT = "{0}/{1}/{2}/{3}.xml";

		public const string CURRENT_USER_URI = "current";
		public const string PUT = "PUT";
		public const string POST = "POST";
		public const string DELETE = "DELETE";

        private static readonly Dictionary<Type, string> routes = new Dictionary<Type, string>
        {
            {typeof (Issue), "issues"},
            {typeof (Project), "projects"},
            {typeof (User), "users"},
            {typeof (News), "news"},
            {typeof (Query), "queries"},
            {typeof (Version), "versions"},
            {typeof (Attachment), "attachments"},
            {typeof (IssueRelation), "relations"},
            {typeof (TimeEntry), "time_entries"},
            {typeof (IssueStatus), "issue_statuses"},
            {typeof (Tracker), "trackers"},
            {typeof (IssueCategory), "issue_categories"},
            {typeof (Role), "roles"},
            {typeof (ProjectMembership), "memberships"},
            {typeof (Group), "groups"},
            {typeof (TimeEntryActivity), "enumerations/time_entry_activities"},
            {typeof (IssuePriority), "enumerations/issue_priorities"},
            {typeof (Watcher), "watchers"},
            {typeof (IssueCustomField), "custom_fields"},
            {typeof (CustomField), "custom_fields"}
        };

        private readonly string host, apiKey, basicAuthorization;
        private readonly CredentialCache cache;
        private MimeFormat mimeFormat = MimeFormat.xml;

		public static Dictionary<Type, string> Sufixes { get { return routes;} } 

		public string Host { get { return host; } }

        public MimeFormat MimeFormat { get { return mimeFormat; } }
        /// <summary>
        /// Maximum page-size when retrieving complete object lists
        /// <remarks>By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// As of Redmine 2.2.0 you can impersonate user setting user login (eg. jsmith). This only works when using the API with an administrator account, this header will be ignored when using the API with a regular user account.
        /// </summary>
        public string ImpersonateUser { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, bool verifyServerCert = true)
        {
            PageSize = 25;

            Uri uriResult;
            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult) || !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                host = "http://" + host;

            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult))
                throw new RedmineException("The host is not valid!");

            this.host = host;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            if (!verifyServerCert)
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertValidate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable REST API in Administration -> Settings -> Authentication. Then, authentication can be done in 2 different ways:
        /// using your regular login/password via HTTP Basic authentication.
        /// using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to each request in one of the following way:
        /// passed in as a "key" parameter
        /// passed in as a username with a random password via HTTP Basic authentication
        /// passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
        /// You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the default layout.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string apiKey, bool verifyServerCert = true)
            : this(host, verifyServerCert)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable REST API in Administration -> Settings -> Authentication. Then, authentication can be done in 2 different ways:
        /// using your regular login/password via HTTP Basic authentication.
        /// using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to each request in one of the following way:
        /// passed in as a "key" parameter
        /// passed in as a username with a random password via HTTP Basic authentication
        /// passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
        /// You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the default layout.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string login, string password, bool verifyServerCert = true)
            : this(host, verifyServerCert)
        {
            cache = new CredentialCache { { new Uri(host), "Basic", new NetworkCredential(login, password) } };

            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", login, password)));
            basicAuthorization = string.Format("Basic {0}", token);
        }

        /// <summary>
        /// Returns the user whose credentials are used to access the API.
        /// </summary>
        /// <param name="parameters">The accepted parameters are: memberships and groups (added in 2.1).</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        public User GetCurrentUser(NameValueCollection parameters = null)
        {
            return ExecuteDownload<User>(string.Format(REQUEST_FORMAT, host, routes[typeof(User)], CURRENT_USER_URI), "GetCurrentUser", parameters);
        }

        public void AddWatcherToIssue(int issueId, int userId)
        {
            ExecuteUpload(string.Format(REQUEST_FORMAT, host, routes[typeof(Issue)], issueId + "/watchers"), POST, "<user_id>" + userId + "</user_id>", "AddWatcher");
        }

        public void RemoveWatcherFromIssue(int issueId, int userId)
        {
            ExecuteUpload(string.Format(REQUEST_FORMAT, host, routes[typeof(Issue)], issueId + "/watchers/" + userId), DELETE, string.Empty, "RemoveWatcher");
        }

        /// <summary>
        /// Adds an existing user to a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void AddUserToGroup(int groupId, int userId)
        {
            ExecuteUpload(string.Format(REQUEST_FORMAT, host, routes[typeof(Group)], groupId + "/users"), POST, "<user_id>" + userId + "</user_id>", "AddUser");
        }

        /// <summary>
        /// Removes an user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void RemoveUserFromGroup(int groupId, int userId)
        {
            ExecuteUpload(string.Format(REQUEST_FORMAT, host, routes[typeof(Group)], groupId + "/users/" + userId), DELETE, string.Empty, "DeleteUser");
        }

        /// <summary>
        /// Downloads the user whose credentials are used to access the API. This method does not block the calling thread.
        /// </summary>
        /// <returns>Returns the Guid associated with the async request.</returns>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        /// <summary>
        /// Returns the details of a wiki page or the details of an old version of a wiki page if the <b>version</b> parameter is set.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="parameters">
        ///     attachments
        ///     The accepted parameters are: memberships and groups (added in 2.1).
        /// </param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="version">The version of the wiki page.</param>
        /// <returns></returns>
        public WikiPage GetWikiPage(string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            string address = version == 0
                                 ? string.Format(WIKI_PAGE_FORMAT, host, projectId, pageName)
                                 : string.Format(WIKI_VERSION_FORMAT, host, projectId, pageName, version);

            return ExecuteDownload<WikiPage>(address, "GetWikiPage", parameters);
        }

        /// <summary>
        /// Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <returns></returns>
        public IList<WikiPage> GetAllWikiPages(string projectId)
        {
            int totalCount;
            return ExecuteDownloadList<WikiPage>(string.Format(WIKI_INDEX_FORMAT, host, projectId), "GetAllWikiPages", out totalCount);
        }

        /// <summary>
        /// Creates or updates a wiki page.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="wikiPage">The wiki page to create or update.</param>
        /// <returns></returns>
        public WikiPage CreateOrUpdateWikiPage(string projectId, string pageName, WikiPage wikiPage)
        {
            string result = RedmineSerializer.Serialize(wikiPage, MimeFormat);

            if (string.IsNullOrEmpty(result)) return null;

            return ExecuteUpload<WikiPage>(string.Format(WIKI_PAGE_FORMAT, host, projectId, pageName), PUT, result, "CreateOrUpdateWikiPage");
        }

        /// <summary>
        /// Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not deleted but changed as root pages.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        public void DeleteWikiPage(string projectId, string pageName)
        {
            ExecuteUpload(string.Format(WIKI_PAGE_FORMAT, host, projectId, pageName), DELETE, string.Empty, "DeleteWikiPage");
        }

        /// <summary>
        /// Support for adding attachments through the REST API is added in Redmine 1.4.0.
        /// Upload a file to server.
        /// </summary>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>Returns the token for uploaded file.</returns>
        public Upload UploadFile(byte[] data)
        {
            using (var wc = CreateUploadWebClient())
            {
                try
                {
                    var response = wc.UploadData(string.Format(FORMAT, host, "uploads"), data);
                    var responseString = Encoding.ASCII.GetString(response);
                    return RedmineSerializer.Deserialize<Upload>(responseString, MimeFormat);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "Upload");
                }
            }

            return null;
        }

        public byte[] DownloadFile(string address)
        {
            using (var wc = CreateUploadWebClient())
            {
                try
                {
                    return wc.DownloadData(address);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "Download");
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a paginated list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a paginated list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved by request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        public List<T> GetObjectList<T>(NameValueCollection parameters) where T : class, new()
        {
            int totalCount;
            return GetObjectList<T>(parameters, out totalCount);
        }

        /// <summary>
        /// Returns a paginated list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <param name="totalCount">Provide information about the total object count available in Redmine.</param>
        /// <returns>Returns a paginated list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved by request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code></code>
        public List<T> GetObjectList<T>(NameValueCollection parameters, out int totalCount) where T : class, new()
        {
            totalCount = -1;
            if (!routes.ContainsKey(typeof(T))) return null;

            var type = typeof(T);
            string address;
            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                var projectId = parameters.GetParameterValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                    throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");

                address = string.Format(ENTITY_WITH_PARENT_FORMAT, host, RedmineKeys.PROJECTS, projectId, routes[type]);
            }
            else
                if (type == typeof(IssueRelation))
                {
                    string issueId = parameters.GetParameterValue(RedmineKeys.ISSUE_ID);
                    if (string.IsNullOrEmpty(issueId))
                        throw new RedmineException("The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");

                    address = string.Format(ENTITY_WITH_PARENT_FORMAT, host, RedmineKeys.ISSUES, issueId, routes[type]);
                }
                else
                    address = string.Format(FORMAT, host, routes[type]);

            return ExecuteDownloadList<T>(address, "GetObjectList<" + type.Name + ">", out totalCount, parameters);
        }

        /// <summary>
        /// Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a complete list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        public List<T> GetTotalObjectList<T>(NameValueCollection parameters) where T : class, new()
        {
            int totalCount, pageSize;
            List<T> resultList = null;
            if (parameters == null) parameters = new NameValueCollection();
            int offset = 0;
            int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
            if (pageSize == default(int))
            {
                pageSize = PageSize > 0 ? PageSize : 25;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }
            do
            {
                parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));
                var tempResult = (List<T>)GetObjectList<T>(parameters, out totalCount);
                if (resultList == null)
                    resultList = tempResult;
                else
                    resultList.AddRange(tempResult);
                offset += pageSize;
            }
            while (offset < totalCount);
            return resultList;
        }

        /// <summary>
        /// Returns a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns the object of type T.</returns>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        /// <code> 
        /// <example>
        ///     string issueId = "927";
        ///     NameValueCollection parameters = null;
        ///     Issue issue = redmineManager.GetObject&lt;Issue&gt;(issueId, parameters);
        /// </example>
        /// </code>
        public T GetObject<T>(string id, NameValueCollection parameters) where T : class, new()
        {
            var type = typeof(T);

            return !routes.ContainsKey(type) ? null : ExecuteDownload<T>(string.Format(REQUEST_FORMAT, host, routes[type], id), "GetObject<" + type.Name + ">", parameters);
        }

        public List<T> GetObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public PaginatedObjects<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <remarks>When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable Entity response. That means that the object could not be created.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        public T CreateObject<T>(T obj) where T : class, new()
        {
            return CreateObject(obj, null);
        }

        /// <summary>
        /// Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <param name="ownerId"></param>
        /// <remarks>When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable Entity response. That means that the object could not be created.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code>
        /// <example>
        ///     var project = new Project();
        ///     project.Name = "test";
        ///     project.Identifier = "the project identifier";
        ///     project.Description = "the project description";
        ///     redmineManager.CreateObject(project);
        /// </example>
        /// </code>
        public T CreateObject<T>(T obj, string ownerId) where T : class, new()
        {
            var type = typeof(T);

            if (!routes.ContainsKey(type)) return null;

            var result = RedmineSerializer.Serialize(obj, MimeFormat);

            if (string.IsNullOrEmpty(result)) return null;

            string address;

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                if (string.IsNullOrEmpty(ownerId))
                    throw new RedmineException("The owner id(project id) is mandatory!");
                address = string.Format(ENTITY_WITH_PARENT_FORMAT, host, RedmineKeys.PROJECTS, ownerId, routes[type]);
            }
            else
                if (type == typeof(IssueRelation))
                {
                    if (string.IsNullOrEmpty(ownerId))
                        throw new RedmineException("The owner id(issue id) is mandatory!");
                    address = string.Format(ENTITY_WITH_PARENT_FORMAT, host, RedmineKeys.ISSUES, ownerId, routes[type]);
                }
                else
                    address = string.Format(FORMAT, host, routes[type]);

            return ExecuteUpload<T>(address, POST, result, "CreateObject<" + type.Name + ">");
        }

        /// <summary>
        /// Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="obj">The object to be update.</param>
        /// <remarks>When trying to update an object with invalid or missing attribute parameters, you will get a 422 Unprocessable Entity response. That means that the object could not be updated.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code></code>
        public void UpdateObject<T>(string id, T obj) where T : class, new()
        {
            UpdateObject(id, obj, null);
        }

        /// <summary>
        /// Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="obj">The object to be update.</param>
        /// <param name="projectId"></param>
        /// <remarks>When trying to update an object with invalid or missing attribute parameters, you will get a 422 Unprocessable Entity response. That means that the object could not be updated.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code></code>
        public void UpdateObject<T>(string id, T obj, string projectId) where T : class, new()
        {
            var type = typeof(T);

            if (!routes.ContainsKey(type)) return;

            var request = RedmineSerializer.Serialize(obj, MimeFormat);
            if (string.IsNullOrEmpty(request)) return;

            request = Regex.Replace(request, @"\r\n|\r|\n", "\r\n");

            string address;

            if (type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                if (string.IsNullOrEmpty(projectId))
                    throw new RedmineException("The project owner id is mandatory!");
                address = string.Format(ENTITY_WITH_PARENT_FORMAT, host, RedmineKeys.PROJECTS, projectId, routes[type]);
            }
            else
            {
                address = string.Format(REQUEST_FORMAT, host, routes[type], id);
            }

            ExecuteUpload(address, PUT, request, "UpdateObject<" + type.Name + ">");
        }

        /// <summary>
        /// Deletes the Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code></code>
        public void DeleteObject<T>(string id, NameValueCollection parameters) where T : class, new()
        {
            var type = typeof(T);

            if (!routes.ContainsKey(typeof(T))) return;

            ExecuteUpload(string.Format(REQUEST_FORMAT, host, routes[type], id), DELETE, string.Empty, "DeleteObject<" + type.Name + ">");
        }

        /// <summary>
        /// Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <code></code>
          public virtual RedmineWebClient CreateWebClient(NameValueCollection parameters)
        {
            var webClient = new RedmineWebClient();

            webClient.Headers.Add(HttpRequestHeader.ContentType,  "application/xml");
            webClient.Encoding = Encoding.UTF8;

            if (parameters != null) webClient.QueryString = parameters;

            if (!string.IsNullOrEmpty(apiKey))
            {
                webClient.QueryString[RedmineKeys.KEY] = apiKey;
            }
            else
            {
                // if (credentialCache != null) webClient.Credentials = credentialCache;
                // webClient.UseDefaultCredentials = true;

                webClient.Headers[HttpRequestHeader.Authorization] = basicAuthorization;
            }

            if (!string.IsNullOrEmpty(ImpersonateUser)) webClient.Headers.Add("X-Redmine-Switch-User", ImpersonateUser);

            return webClient;
        }
        //protected WebClient CreateWebClient(NameValueCollection parameters)
        //{
        //    var webClient = new RedmineWebClient();

        //    if (parameters != null) webClient.QueryString = parameters;

        //    if (!string.IsNullOrEmpty(apiKey))
        //    {
        //        webClient.QueryString[RedmineKeys.KEY] = apiKey;
        //    }
        //    else
        //    {
        //        if (cache != null) webClient.Credentials = cache;
        //    }

        //    if (!string.IsNullOrEmpty(ImpersonateUser)) webClient.Headers.Add("X-Redmine-Switch-User", ImpersonateUser);

        //    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
        //    webClient.Headers.Add(HttpRequestHeader.UserAgent, ua);
        //    webClient.Encoding = Encoding.UTF8;

        //    return webClient;
        //}
        const string ua = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:8.0) Gecko/20100101 Firefox/8.0";
        /// <summary>
        /// Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <code></code>
        protected WebClient CreateUploadWebClient(NameValueCollection parameters = null)
        {
            var webClient = new RedmineWebClient();

            if (parameters != null) webClient.QueryString = parameters;

            if (!string.IsNullOrEmpty(apiKey))
            {
                webClient.QueryString[RedmineKeys.KEY] = apiKey;
            }
            else
            {
                if (cache != null) webClient.Credentials = cache;
            }

            webClient.UseDefaultCredentials = false;

            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
            // Workaround - it seems that WebClient doesn't send credentials in each POST request
            webClient.Headers.Add(HttpRequestHeader.Authorization, basicAuthorization);

            return webClient;
        }

        /// <summary>
        /// This is to take care of SSL certification validation which are not issued by Trusted Root CA.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="cert">The cert.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        /// <code></code>
        public virtual bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            if (error == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                error);

            return false;
        }

        public virtual void HandleWebException(WebException exception, string method)
        {
            if (exception == null) return;

            switch (exception.Status)
            {
                case WebExceptionStatus.Timeout: throw new RedmineException("Timeout!", exception);
                case WebExceptionStatus.NameResolutionFailure: throw new RedmineException("Bad domain name!", exception);
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.InternalServerError:
                            case (int)HttpStatusCode.Unauthorized:
                            case (int)HttpStatusCode.NotFound:
                            case (int)HttpStatusCode.Forbidden:
                                throw new RedmineException(response.StatusDescription, exception);

                            case (int)HttpStatusCode.Conflict:
                                throw new RedmineException("The page that you are trying to update is staled!", exception);

                            case 422:
                                var errors = ReadWebExceptionResponse(exception.Response);
                                string message = string.Empty;
                                if (errors != null)
                                {
                                    foreach (var error in errors)
                                    {
                                        message += error.Info + Environment.NewLine;
                                    }
                                }
                                throw new RedmineException(method + " has invalid or missing attribute parameters: " + message, exception);

                            case (int)HttpStatusCode.NotAcceptable: throw new RedmineException(response.StatusDescription, exception);
                        }
                    }
                    break;

                default: throw new RedmineException(exception.Message, exception);
            }
        }

        private IEnumerable<Error> ReadWebExceptionResponse(WebResponse webResponse)
        {
            using (var dataStream = webResponse.GetResponseStream())
            {
                if (dataStream == null) return null;
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();

                    if (responseFromServer.Trim().Length > 0)
                    {
                        try
                        {
                            var result = RedmineSerializer.DeserializeList<Error>(responseFromServer, MimeFormat);
                            if (result == null) throw new RedmineException("could not deserialize the errors!");
                            return result.Objects;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.Message);
                        }
                    }
                    return null;
                }
            }
        }

        private void ExecuteUpload(string address, string actionType, string data, string methodName)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    if (actionType == POST || actionType == DELETE || actionType == PUT)
                    {
                        wc.UploadString(address, actionType, data);
                    }
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, methodName);
                }
            }
        }

        private T ExecuteUpload<T>(string address, string actionType, string data, string methodName) where T : class , new()
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    if (actionType == POST || actionType == DELETE || actionType == PUT)
                    {
                        var response = wc.UploadString(address, actionType, data);

                        return RedmineSerializer.Deserialize<T>(response, MimeFormat);
                    }
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, methodName);
                }
                return default(T);
            }
        }

        private T ExecuteDownload<T>(string address, string methodName, NameValueCollection parameters = null) where T : class, new()
        {
            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    if (!string.IsNullOrEmpty(response))
                        return RedmineSerializer.Deserialize<T>(response, MimeFormat);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, methodName);
                }
                return default(T);
            }
        }

        private List<T> ExecuteDownloadList<T>(string address, string methodName, out int totalCount, NameValueCollection parameters = null) where T : class, new()
        {
            totalCount = -1;
            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    var result = RedmineSerializer.DeserializeList<T>(response, MimeFormat);
                    return result.Objects;
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, methodName);
                }
                return null;
            }
        }
    }

	public class PaginatedObjects<T>{
		public List<T> Objects {
			get;
			set;
		}

		public int TotalCount {
			get;
			set;
		}
	}
}