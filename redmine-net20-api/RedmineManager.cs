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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Group = Redmine.Net.Api.Types.Group;
using Version = Redmine.Net.Api.Types.Version;
using Redmine.Net.Api.Exceptions;

namespace Redmine.Net.Api
{
    /// <summary>
    /// The main class to access Redmine API.
    /// </summary>
    public class RedmineManager : IRedmineManager
    {
        public const string REQUEST_FORMAT = "{0}/{1}/{2}.{3}";
        public const string FORMAT = "{0}/{1}.{2}";

        public const string WIKI_INDEX_FORMAT = "{0}/projects/{1}/wiki/index.{2}";
        public const string WIKI_PAGE_FORMAT = "{0}/projects/{1}/wiki/{2}.{3}";
        public const string WIKI_VERSION_FORMAT = "{0}/projects/{1}/wiki/{2}/{3}.{4}";

        public const string ENTITY_WITH_PARENT_FORMAT = "{0}/{1}/{2}/{3}.{4}";

        public const string ATTACHMENT_UPDATE_FORMAT = "{0}/attachments/issues/{1}.{2}";

        public const string CURRENT_USER_URI = "current";
        /// <summary>
        /// Represents an HTTP PUT protocol method that is used to replace an entity identified by a URI.
        /// </summary>
        public const string PUT = "PUT";
        /// <summary>
        /// Represents an HTTP POST protocol method that is used to post a new entity as an addition to a URI.
        /// </summary>
        public const string POST = "POST";
        /// <summary>
        /// Represents an HTTP PATCH protocol method that is used to patch an existing entity identified  by a URI.
        /// </summary>
        public const string PATCH = "PATCH";
        /// <summary>
        /// Represents an HTTP DELETE protocol method.
        /// </summary>
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
        private readonly MimeFormat mimeFormat;

        public static Dictionary<Type, string> Sufixes { get { return routes; } }

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

        public IWebProxy Proxy { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="mimeFormat"></param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, MimeFormat mimeFormat = MimeFormat.xml, bool verifyServerCert = true, IWebProxy proxy = null)
        {
            if (string.IsNullOrEmpty(host)) throw new RedmineException("Host is not defined!");
            PageSize = 25;

            Uri uriResult;
            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult) || !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                host = "http://" + host;

            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult))
                throw new RedmineException("The host is not valid!");

            this.host = host;
            this.mimeFormat = mimeFormat;
            Proxy = proxy;

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
        /// <param name="mimeFormat"></param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string apiKey, MimeFormat mimeFormat = MimeFormat.xml, bool verifyServerCert = true, IWebProxy proxy = null)
            : this(host, mimeFormat, verifyServerCert, proxy)
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
        /// <param name="mimeFormat"></param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string login, string password, MimeFormat mimeFormat = MimeFormat.xml, bool verifyServerCert = true, IWebProxy proxy = null)
            : this(host, mimeFormat, verifyServerCert, proxy)
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
            var url = UrlHelper.GetCurrentUserUrl(this);
            return ExecuteDownload<User>(url, "GetCurrentUser", parameters);
        }

        public void AddWatcherToIssue(int issueId, int userId)
        {
            var url = UrlHelper.GetAddWatcherUrl(this, issueId, userId);
            ExecuteUpload(url, POST, MimeFormat == MimeFormat.xml
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}"
                , "AddWatcher");
        }

        public void RemoveWatcherFromIssue(int issueId, int userId)
        {
            var url = UrlHelper.GetRemoveWatcherUrl(this, issueId, userId);
            ExecuteUpload(url, DELETE, string.Empty, "RemoveWatcher");
        }

        /// <summary>
        /// Adds an existing user to a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void AddUserToGroup(int groupId, int userId)
        {
            var url = UrlHelper.GetAddUserToGroupUrl(this, groupId);
            ExecuteUpload(url, POST, MimeFormat == MimeFormat.xml
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}", "AddUser");
        }

        /// <summary>
        /// Removes an user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void RemoveUserFromGroup(int groupId, int userId)
        {
            var url = UrlHelper.GetRemoveUserFromGroupUrl(this, groupId, userId);
            ExecuteUpload(url, DELETE, string.Empty, "DeleteUser");
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
            var url = UrlHelper.GetWikiPageUrl(this, projectId, parameters, pageName, version);
            return ExecuteDownload<WikiPage>(url, "GetWikiPage", parameters);
        }

        /// <summary>
        /// Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <returns></returns>
        public IList<WikiPage> GetAllWikiPages(string projectId)
        {
            var url = UrlHelper.GetWikisUrl(this, projectId);
            var result = ExecuteDownloadList<WikiPage>(url, "GetAllWikiPages");
            return result != null ? result.Objects : null;
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

            var url = UrlHelper.GetWikiCreateOrUpdaterUrl(this, projectId, pageName);
            return ExecuteUpload<WikiPage>(url, PUT, result, "CreateOrUpdateWikiPage");
        }

        /// <summary>
        /// Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not deleted but changed as root pages.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        public void DeleteWikiPage(string projectId, string pageName)
        {
            var url = UrlHelper.GetDeleteWikirUrl(this, projectId, pageName);
            ExecuteUpload(url, DELETE, string.Empty, "DeleteWikiPage");
        }

        /// <summary>
        /// Gets the redmine object based on id.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns the object of type T.</returns>
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
            var url = UrlHelper.GetGetUrl<T>(this, id);
            return ExecuteDownload<T>(url, "GetObject", parameters);
        }

        /// <summary>
        /// Returns a paginated list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a paginated list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved by request. 
        /// Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        [Obsolete("Use GetObjects method instead.")]
        public List<T> GetObjectList<T>(NameValueCollection parameters) where T : class, new()
        {
            return GetObjects<T>(parameters);
        }

        public PaginatedObjects<T> GetPaginatedObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            var url = UrlHelper.GetListUrl<T>(this, parameters);
            return ExecuteDownloadList<T>(url, "GetObjectList", parameters);
        }

        /// <summary>
        /// Returns a paginated list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <param name="totalCount">Provide information about the total object count available in Redmine.</param>
        /// <returns>Returns a paginated list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved by request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <code></code>
        [Obsolete("Use GetPaginatedObjects method instead.")]
        public List<T> GetObjectList<T>(NameValueCollection parameters, out int totalCount) where T : class, new()
        {
            totalCount = -1;
            var result = GetPaginatedObjects<T>(parameters);
            if (result != null)
            {
                totalCount = result.TotalCount;
                return result.Objects;
            }
            return null;
        }

        /// <summary>
        /// Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a complete list of objects.</returns>

        public List<T> GetObjects<T>(NameValueCollection parameters) where T : class, new()
        {
            int totalCount = 0, pageSize;
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
                var tempResult = GetPaginatedObjects<T>(parameters);
                if (tempResult != null)
                {
                    if (resultList == null)
                    {
                        resultList = tempResult.Objects;
                        totalCount = tempResult.TotalCount;
                    }
                    else
                        resultList.AddRange(tempResult.Objects);
                }
                offset += pageSize;
            } while (offset < totalCount);
            return resultList;
        }

        /// <summary>
        /// Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a complete list of objects.</returns>

        [Obsolete("Use GetObjects method instead.")]
        public List<T> GetTotalObjectList<T>(NameValueCollection parameters) where T : class, new()
        {
            return GetObjects<T>(parameters);
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
            var url = UrlHelper.GetCreateUrl<T>(this, ownerId);
            var data = RedmineSerializer.Serialize(obj, MimeFormat);
            return ExecuteUpload<T>(url, POST, data, "CreateObject");
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
            var url = UrlHelper.GetUploadUrl(this, id, obj, projectId);
            var data = RedmineSerializer.Serialize(obj, MimeFormat);
            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");
            ExecuteUpload(url, PUT, data, "UpdateObject");
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
            var url = UrlHelper.GetDeleteUrl<T>(this, id);
            ExecuteUpload(url, DELETE, string.Empty, "DeleteObject");
        }

        /// <summary>
        /// Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        /// <code></code>
        public virtual RedmineWebClient CreateWebClient(NameValueCollection parameters, bool uploadFile = false)
        {
            var webClient = new RedmineWebClient { Proxy = Proxy };
            if (!uploadFile)
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType, MimeFormat == MimeFormat.xml ? "application/xml" : "application/json");
                webClient.Encoding = Encoding.UTF8;
            }
            else
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
            }

            if (parameters != null)
                webClient.QueryString = parameters;

            if (!string.IsNullOrEmpty(apiKey))
            {
                webClient.QueryString[RedmineKeys.KEY] = apiKey;
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

            if (!string.IsNullOrEmpty(ImpersonateUser))
                webClient.Headers.Add("X-Redmine-Switch-User", ImpersonateUser);

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

            Trace.TraceError("X509Certificate [{0}] Policy Error: '{1}'", cert.Subject, error);

            return false;
        }

        /// <summary>
        /// Support for adding attachments through the REST API is added in Redmine 1.4.0.
        /// Upload a file to server.
        /// </summary>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>Returns the token for uploaded file.</returns>
        public Upload UploadFile(byte[] data)
        {
            using (var wc = CreateWebClient(null, true))
            {
                try
                {
                    var url = UrlHelper.GetUploadFileUrl(this);
                    var response = wc.UploadData(url, data);
                    var responseString = Encoding.ASCII.GetString(response);
                    return RedmineSerializer.Deserialize<Upload>(responseString, MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException("UploadFile", MimeFormat);
                }
            }

            return null;
        }

        public byte[] DownloadFile(string address)
        {
            using (var wc = CreateWebClient(null, true))
            {
                try
                {
                    return wc.DownloadData(address);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException("DownloadFile", MimeFormat);
                }
            }

            return null;
        }

        private void ExecuteUpload(string address, string actionType, string data, string methodName)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    if (actionType == POST || actionType == DELETE || actionType == PUT || actionType == PATCH)
                    {
                        wc.UploadString(address, actionType, data);
                    }
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, MimeFormat);
                }
            }
        }

        private T ExecuteUpload<T>(string address, string actionType, string data, string methodName) where T : class, new()
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    if (actionType == POST || actionType == DELETE || actionType == PUT || actionType == PATCH)
                    {
                        var response = wc.UploadString(address, actionType, data);
                        return RedmineSerializer.Deserialize<T>(response, MimeFormat);
                    }
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, MimeFormat);
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
                    webException.HandleWebException(methodName, MimeFormat);
                }
                return default(T);
            }
        }

        private PaginatedObjects<T> ExecuteDownloadList<T>(string address, string methodName, NameValueCollection parameters = null) where T : class, new()
        {
            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    return RedmineSerializer.DeserializeList<T>(response, MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, MimeFormat);
                }
                return null;
            }
        }

        public void UpdateAttachment(int issueId, Attachment attachment)
        {
            var address = UrlHelper.GetAttachmentUpdateUrl(this, issueId);
            var attachments = new Attachments {{attachment.Id, attachment}};
            var data = RedmineSerializer.Serialize(attachments, MimeFormat);

            ExecuteUpload(address, PATCH, data, "UpdateAttachment");    
        }
    }

    public class Attachments : Dictionary<int, Attachment>
    {

    }
}
