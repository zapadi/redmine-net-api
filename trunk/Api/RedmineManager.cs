/*
   Copyright 2011 - 2012 Adrian Popescu, Dorin Huzum.

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
using System.Xml;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;
using System.Threading;

namespace Redmine.Net.Api
{
    /// <summary>
    /// The main class to access Redmine API.
    /// </summary>
    public partial class RedmineManager
    {
        private const string RequestFormat = "{0}/{1}/{2}.{3}";
        private const string Format = "{0}/{1}.{2}";

        private const string WikiIndexFormat = "{0}/projects/{1}/wiki/index.{2}";
        private const string WikiPageFormat = "{0}/projects/{1}/wiki/{2}.{3}";
        private const string WikiVersionFormat = "{0}/projects/{1}/wiki/{2}/{3}.{4}";

        private const string EntityWithParentFormat = "{0}/{1}/{2}/{3}.{4}";

        private const string CurrentUserUri = "current";
        private const string PUT = "PUT";
        private const string POST = "POST";
        private const string DELETE = "DELETE";

        private readonly string host, apiKey, basicAuthorization;
        private readonly MimeFormat mimeFormat;
        private readonly CredentialCache cache;

        /// <summary>
        /// Maximum page-size when retrieving complete object lists
        /// <remarks>By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// </summary>
        public int PageSize { get; set; }

        private readonly Dictionary<Type, String> urls = new Dictionary<Type, string>
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
            {typeof (IssuePriority), "enumerations/issue_priorities"}
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="mimeFormat"></param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, MimeFormat mimeFormat = MimeFormat.xml, bool verifyServerCert = true)
        {
            PageSize = 25;

            Uri uriResult;
            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult) || !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                host = "http://" + host;

            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult))
                throw new RedmineException("The host is not valid!");

            this.host = host;
            this.mimeFormat = mimeFormat;

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
        /// <param name="mimeFormat">The Mime format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string apiKey, MimeFormat mimeFormat = MimeFormat.xml, bool verifyServerCert = true)
            : this(host, mimeFormat, verifyServerCert)
        {
            PageSize = 25;
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
        /// <param name="mimeFormat">The Mime format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string login, string password, MimeFormat mimeFormat = MimeFormat.xml, bool verifyServerCert = true)
            : this(host, mimeFormat, verifyServerCert)
        {
            PageSize = 25;
            Uri uriResult;
            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult) || !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                host = "http://" + host;

            if (!Uri.TryCreate(host, UriKind.Absolute, out uriResult))
                throw new RedmineException("The host is not valid!");

            cache = new CredentialCache { { uriResult, "Basic", new NetworkCredential(login, password) } };
            basicAuthorization = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + password));
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
            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(string.Format(RequestFormat, host, urls[typeof(User)], CurrentUserUri, mimeFormat));

                    //#if RUNNING_ON_35_OR_ABOVE
                    if (mimeFormat == MimeFormat.json)
                        return RedmineSerialization.JsonDeserialize<User>(response, null);
                    //#endif
                    return RedmineSerialization.FromXML<User>(response);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "WikiPage");
                }
                return null;
            }
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
            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(version == 0
                                                                ? string.Format(WikiPageFormat, host, projectId, pageName, mimeFormat)
                                                                : string.Format(WikiVersionFormat, host, projectId, pageName, version, mimeFormat));
                    //   #if RUNNING_ON_35_OR_ABOVE
                    if (mimeFormat == MimeFormat.json)
                        return RedmineSerialization.JsonDeserialize<WikiPage>(response, null);
                    // #endif
                    return RedmineSerialization.FromXML<WikiPage>(response);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "WikiPage");
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <returns></returns>
        public IList<WikiPage> GetAllWikiPages(string projectId)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    var response = wc.DownloadString(string.Format(WikiIndexFormat, host, projectId, mimeFormat));
                    int totalCount;
                    return DeserializeList<WikiPage>(response, "wiki", out totalCount);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "WikiPage");
                }
                return null;
            }
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
            string result = Serialize(wikiPage);

            if (string.IsNullOrEmpty(result)) return null;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    var response = wc.UploadString(string.Format(WikiPageFormat, host, projectId, pageName, mimeFormat), PUT, result);
                    return Deserialize<WikiPage>(response);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "WikiPage");
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not deleted but changed as root pages.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        public void DeleteWikiPage(string projectId, string pageName)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(WikiPageFormat, host, projectId, pageName, mimeFormat), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "WikiPage");
                }
            }
        }

        /// <summary>
        /// Upload data on server.
        /// </summary>
        /// <param name="data">Data which will be uploaded on server</param>
        /// <returns>Returns 'Upload' object with inialized 'Token' by server response.</returns>
        public Upload UploadData(byte[] data)
        {
            using (var wc = CreateUploadWebClient(null))
            {
                try
                {
                    var response = wc.UploadData(string.Format(Format, host, "uploads", mimeFormat), data);
                    var responseString = Encoding.ASCII.GetString(response);
#if RUNNING_ON_35_OR_ABOVE
                    if (mimeFormat == MimeFormat.json)
                        return RedmineSerialization.JsonDeserialize<Upload>(responseString);
#endif
                    return RedmineSerialization.FromXML<Upload>(responseString);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "Upload");
                }
            }

            return null;
        }

        /// <summary>
        /// Adds an existing user to a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void AddUserToGroup(int groupId, int userId)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users", mimeFormat), POST, mimeFormat == MimeFormat.xml ? "<user_id>" + userId + "</user_id>" : "user_id:" + userId);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "User");
                }
            }
        }

        /// <summary>
        /// Removes an user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void DeleteUserFromGroup(int groupId, int userId)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users/" + userId, mimeFormat), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "User");
                }
            }
        }

        /// <summary>
        /// Returns a paginated list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a paginated list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved by request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        public IList<T> GetObjectList<T>(NameValueCollection parameters) where T : class, new()
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
        public IList<T> GetObjectList<T>(NameValueCollection parameters, out int totalCount) where T : class, new()
        {
            totalCount = -1;
            if (!urls.ContainsKey(typeof(T))) return null;

            using (var wc = CreateWebClient(parameters))
            {
                var type = typeof(T);
                try
                {
                    string result;

                    if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
                    {
                        var projectId = GetOwnerId(parameters, "project_id");
                        if (string.IsNullOrEmpty(projectId))
                            throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");

                        result = wc.DownloadString(string.Format(EntityWithParentFormat, host, "projects", projectId, urls[type], mimeFormat));
                    }
                    else
                        if (type == typeof(IssueRelation))
                        {
                            string issueId = GetOwnerId(parameters, "issue_id");
                            if (string.IsNullOrEmpty(issueId)) throw new RedmineException("The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");

                            result = wc.DownloadString(string.Format(EntityWithParentFormat, host, "issues", issueId, urls[type], mimeFormat));
                        }
                        else
                            result = wc.DownloadString(string.Format(Format, host, urls[type], mimeFormat));

                    return DeserializeList<T>(result, urls[type], out totalCount);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a complete list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        public IList<T> GetTotalObjectList<T>(NameValueCollection parameters) where T : class, new()
        {
            int totalCount, pageSize;
            List<T> resultList = null;
            if (parameters == null) parameters = new NameValueCollection();
            int offset = 0;
            int.TryParse(parameters["limit"], out pageSize);
            if (pageSize == default(int))
            {
                pageSize = PageSize > 0 ? PageSize : 25;
                parameters.Set("limit", pageSize.ToString(CultureInfo.InvariantCulture));
            }
            do
            {
                parameters.Set("offset", offset.ToString(CultureInfo.InvariantCulture));
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

            if (!urls.ContainsKey(type)) return null;

            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(string.Format(RequestFormat, host, urls[type], id, mimeFormat));
                    return Deserialize<T>(response);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <param name="parameters"></param>
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

            if (!urls.ContainsKey(type)) return null;

            var result = Serialize(obj);

            if (string.IsNullOrEmpty(result)) return null;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    string response;
                    if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
                    {
                        if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(project id) is mandatory!");
                        response = wc.UploadString(string.Format(EntityWithParentFormat, host, "projects", ownerId, urls[type], mimeFormat), result);
                    }
                    else
                        if (type == typeof(IssueRelation))
                        {
                            if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(issue id) is mandatory!");
                            response = wc.UploadString(string.Format(EntityWithParentFormat, host, "issues", ownerId, urls[type], mimeFormat), result);
                        }
                        else
                            response = wc.UploadString(string.Format(Format, host, urls[type], mimeFormat), result);
                    //  #if RUNNING_ON_35_OR_ABOVE
                    if (mimeFormat == MimeFormat.json)
                        return type == typeof(IssueCategory)
                            ? RedmineSerialization.JsonDeserialize<T>(response, "issue_category")
                            : RedmineSerialization.JsonDeserialize<T>(response, type == typeof(IssueRelation) ? "relation" : null);
                    // #endif
                    return RedmineSerialization.FromXML<T>(response);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
            }
            return null;
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

            if (!urls.ContainsKey(type)) return;

            var request = Serialize(obj);
            if (string.IsNullOrEmpty(request)) return;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
                    {
                        if (string.IsNullOrEmpty(projectId)) throw new RedmineException("The project owner id is mandatory!");
                        wc.UploadString(string.Format(EntityWithParentFormat, host, "projects", projectId, urls[type], mimeFormat), request);
                    }
                    else
                        wc.UploadString(string.Format(RequestFormat, host, urls[type], id, mimeFormat), PUT, request);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
            }
        }

        /// <summary>
        /// Deletes the Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code></code>
        public void DeleteObject<T>(string id, NameValueCollection parameters) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(typeof(T))) return;

            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    wc.UploadString(string.Format(RequestFormat, host, urls[type], id, mimeFormat), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
            }
        }

        /// <summary>
        /// Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <code></code>
        protected WebClient CreateWebClient(NameValueCollection parameters)
        {
            var webClient = new RedmineWebClient();

            if (parameters != null) webClient.QueryString = parameters;

            if (!string.IsNullOrEmpty(apiKey))
            {
                webClient.QueryString["key"] = apiKey;
            }
            else
            {
                if (cache != null) webClient.Credentials = cache;
            }

            // #if RUNNING_ON_35_OR_ABOVE
            webClient.Headers.Add("Content-Type", mimeFormat == MimeFormat.json ? "application/json; charset=utf-8" : "application/xml; charset=utf-8");
            //  #elseif
            //      webClient.Headers.Add("Content-Type", "application/xml; charset=utf-8");
            //  #endif

            webClient.Encoding = Encoding.UTF8;
            return webClient;
        }

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
                webClient.QueryString["key"] = apiKey;
            }
            else
            {
                if (cache != null) webClient.Credentials = cache;
            }

            webClient.UseDefaultCredentials = false;

            webClient.Headers.Add("Content-Type", "application/octet-stream");
            // Workaround - it seems that WebClient doesn't send credentials in each POST request
            webClient.Headers.Add("Authorization", basicAuthorization);

            return webClient;
        }

        /// <summary>
        /// This is to take care of SSL certification validation which are not issued by Trusted Root CA. Recommended for testing  only.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="cert">The cert.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        /// <code></code>
        protected bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //Cert Validation Logic
            return true;
        }

        private void HandleWebException(WebException exception, string method)
        {
            if (exception == null) return;

            switch (exception.Status)
            {
                case WebExceptionStatus.Timeout: throw new RedmineException("Timeout!");
                case WebExceptionStatus.NameResolutionFailure: throw new RedmineException("Bad domain name!");
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.InternalServerError:
                            case (int)HttpStatusCode.Unauthorized:
                            case (int)HttpStatusCode.NotFound:
                            case (int)HttpStatusCode.Forbidden:
                                throw new RedmineException(response.StatusDescription);

                            case (int)HttpStatusCode.Conflict:
                                throw new RedmineException("The page that you are trying to update is staled!");

                            case 422:
                                var errors = ReadWebExceptionResponse(exception.Response);
                                string message = string.Empty;
                                if (errors != null)
                                {
                                    foreach (var error in errors) message += error.Info + "\n";
                                }
                                throw new RedmineException(method + " has invalid or missing attribute parameters: " + message);

                            case (int)HttpStatusCode.NotAcceptable: throw new RedmineException(response.StatusDescription);
                        }
                    }
                    break;

                default: throw new RedmineException(exception.Message);
            }
        }

        private static string GetOwnerId(NameValueCollection parameters, string parameterName)
        {
            string ownerId = null;

            if (parameters != null)
            {
                ownerId = parameters.Get(parameterName);
                if (string.IsNullOrEmpty(ownerId)) return null;
            }
            return ownerId;
        }

        private IList<Error> ReadWebExceptionResponse(WebResponse webResponse)
        {
            using (var dataStream = webResponse.GetResponseStream())
            {
                if (dataStream == null) return null;
                var reader = new StreamReader(dataStream);

                var responseFromServer = reader.ReadToEnd();

                if (responseFromServer.Trim().Length > 0)
                {
                    try
                    {
                        int totalCount;
                        return DeserializeList<Error>(responseFromServer, "errors", out totalCount);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
                return null;
            }
        }

        private string Serialize<T>(T obj) where T : class, new()
        {
            //  #if RUNNING_ON_35_OR_ABOVE
            if (mimeFormat == MimeFormat.json)
                return RedmineSerialization.JsonSerializer(obj);
            //#else
            return RedmineSerialization.ToXML(obj);
            //  #endif
        }

        private T Deserialize<T>(string response) where T : class, new()
        {
            //  #if RUNNING_ON_35_OR_ABOVE
            if (mimeFormat == MimeFormat.json)
                return RedmineSerialization.JsonDeserialize<T>(response, null);
            // #endif
            return RedmineSerialization.FromXML<T>(response);
        }

        private IList<T> DeserializeList<T>(string response, string jsonRoot, out int totalCount) where T : class, new()
        {
            //#if RUNNING_ON_35_OR_ABOVE
            if (mimeFormat == MimeFormat.json)
                return RedmineSerialization.JsonDeserializeToList<T>(response, jsonRoot, out totalCount);
            // #endif

            using (var text = new StringReader(response))
            {
                using (var xmlReader = new XmlTextReader(text))
                {
                    xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                    xmlReader.Read();
                    xmlReader.Read();

                    totalCount = xmlReader.ReadAttributeAsInt("total_count");

                    return xmlReader.ReadElementContentAsCollection<T>();
                }
            }
        }
    }
}