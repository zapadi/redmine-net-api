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

using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;
using System.Threading;
using System.ComponentModel;

namespace Redmine.Net.Api
{
    /// <summary>
    /// The main class to access Redmine API.
    /// </summary>
    public class RedmineManager
    {
        private const string RequestFormat = "{0}/{1}/{2}.{3}";
        private const string Format = "{0}/{1}.{2}";
        private const string CurrentUserUri = "current";
        private const string PUT = "PUT";
        private const string POST = "POST";
        private const string DELETE = "DELETE";

        private readonly string host, apiKey, basicAuthorization;
        string responseType = "xml";
        private readonly CredentialCache cache;

        public event EventHandler<AsyncEventArgs> DownloadCompleted;

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
            {typeof (Group), "groups"}
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, bool verifyServerCert = true)
        {
            PageSize = 25;
            this.host = host;

            if (!verifyServerCert)
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertValidate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string apiKey, bool verifyServerCert = true)
            : this(host, verifyServerCert)
        {
            PageSize = 25;
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        public RedmineManager(string host, string login, string password, bool verifyServerCert = true)
            : this(host, verifyServerCert)
        {
            PageSize = 25;
            cache = new CredentialCache { { new Uri(host), "Basic", new NetworkCredential(login, password) } };
            basicAuthorization = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + password));
        }

        /// <summary>
        /// Returns the user whose credentials are used to access the API.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        public User GetCurrentUser(NameValueCollection parameters = null)
        {
            using (var wc = CreateWebClient(parameters))
            {
                var xml = wc.DownloadString(string.Format(RequestFormat, host, urls[typeof(User)], CurrentUserUri, responseType));

                return RedmineSerialization.FromXML<User>(xml);
            }
        }

        public void GetCurrentUserAsync(NameValueCollection parameters = null)
        {
            using (var wc = CreateWebClient(parameters))
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.DownloadStringAsync(new Uri(string.Format(RequestFormat, host, urls[typeof(User)], CurrentUserUri, responseType)), new AsyncToken { Method = "GetCurrentUser", ResponsetType = typeof(User) });
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var ut = e.UserState as AsyncToken;
            if (e.Error != null)
                HandleWebException((WebException)e.Error, ut.ResponsetType.Name);
            else
                if (!e.Cancelled)
                {
                    AsyncEventArgs aev = new AsyncEventArgs();
                    try
                    {
                        aev.Result = RedmineSerialization.FromXML(e.Result, ut.ResponsetType);
                    }
                    catch (ThreadAbortException ex)
                    {
                        aev.Error = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        aev.Error = ex.Message;
                    }
                    if (DownloadCompleted != null)
                        DownloadCompleted(this, aev);
                }
        }

        /// <summary>
        /// Upload data on server.
        /// </summary>
        /// <param name="data">Data which will be uploaded on server</param>
        /// <returns>Returns 'Upload' object with inialized 'Token' by server response.</returns>
        public Upload UploadData(byte[] data)
        {
            using (var wc = new WebClient())
            {
                wc.UseDefaultCredentials = false;

                wc.Headers.Add("Content-Type", "application/octet-stream");
                // Workaround - it seems that WebClient doesn't send credentials in each POST request
                wc.Headers.Add("Authorization", basicAuthorization);
                try
                {
                    //  wc.UploadDataAsync(new Uri(string.Format(Format, host, "uploads", responseType)), data);
                    wc.UploadDataCompleted += new UploadDataCompletedEventHandler(wc_UploadDataCompleted);

                    byte[] response = wc.UploadData(string.Format(Format, host, "uploads", responseType), data);
                    var responseString = Encoding.ASCII.GetString(response);

                    return RedmineSerialization.FromXML<Upload>(responseString);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "Upload");
                }
            }
            return null;
        }

        public void UploadDataAsync(byte[] data)
        {
            using (var wc = new WebClient())
            {
                wc.UseDefaultCredentials = false;

                wc.Headers.Add("Content-Type", "application/octet-stream");
                // Workaround - it seems that WebClient doesn't send credentials in each POST request
                wc.Headers.Add("Authorization", basicAuthorization);
                try
                {
                    wc.UploadDataCompleted += new UploadDataCompletedEventHandler(wc_UploadDataCompleted);
                    wc.UploadDataAsync(new Uri(string.Format(Format, host, "uploads", responseType)), data);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "Upload");
                }
            }
        }

        private void wc_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            var responseString = Encoding.ASCII.GetString(e.Result);
            var result = RedmineSerialization.FromXML<Upload>(responseString);
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
                    wc.UploadString(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users", responseType), POST, "<user_id>" + userId + "</user_id>");
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "User");
                }
            }
        }

        public void AddUserToGroupAsync(int groupId, int userId)
        {
            using (var wc = CreateWebClient(null))
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.UploadStringAsync(new Uri(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users", responseType)), POST, "<user_id>" + userId + "</user_id>", new AsyncToken { Method = "AddUserToGroup",  Parameter = userId });
            }
        }

        /// <summary>
        /// Removes a user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void DeleteUserFromGroup(int groupId, int userId)
        {
            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users/" + userId, responseType), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, "User");
                }
            }
        }

        public void DeleteUserFromGroupAsync(int groupId, int userId)
        {
            using (var wc = CreateWebClient(null))
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.UploadStringAsync(new Uri(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users/" + userId, responseType)), DELETE, string.Empty, new AsyncToken { Method = "DeleteUserFromGroup",  Parameter = userId });
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
        public IList<T> GetObjectList<T>(NameValueCollection parameters) where T : class
        {
            int totalCount;
            return GetObjectList<T>(parameters, out totalCount);
        }

        /// <summary>
        /// Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>Returns a complete list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        public IList<T> GetTotalObjectList<T>(NameValueCollection parameters) where T : class
        {
            int totalCount, pageSize;
            List<T> resultList = null;
            if (parameters == null) parameters = new NameValueCollection();
            int offset = 0;
            int.TryParse(parameters["limit"], out pageSize);
            if(pageSize == default (int))
            {
                pageSize = PageSize;
                parameters.Set("limit", pageSize.ToString());
            }
            do
            {
                parameters.Set("offset", offset.ToString());
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
        /// Returns a paginated list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <param name="totalCount">Provide information about the total object count available in Redmine.</param>
        /// <returns>Returns a paginated list of objects.</returns>
        /// <remarks>By default only 25 results can be retrieved by request. Maximum is 100. To change the maximum value set in your Settings -> General, "Objects per page options".By adding (for instance) 9999 there would make you able to get that many results per request.</remarks>
        /// <exception cref="Redmine.Net.Api.RedmineException"></exception>
        /// <code></code>
        public IList<T> GetObjectList<T>(NameValueCollection parameters, out int totalCount) where T : class
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
                        string projectId = null;
                        var hasProjectId = false;
                        if (parameters != null)
                        {
                            projectId = parameters.Get("project_id");
                            if (!string.IsNullOrEmpty(projectId)) hasProjectId = true;
                        }

                        if (!hasProjectId)
                        {
                            throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");
                        }

                        result = wc.DownloadString(string.Format("{0}/projects/{1}/{2}.{3}", host, projectId, urls[type], responseType));
                    }
                    else
                    {
                        result = wc.DownloadString(string.Format(Format, host, urls[type], responseType));
                    }

                    using (var text = new StringReader(result))
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
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
                return null;
            }
        }

        /// <summary>
        /// 
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
        public T GetObject<T>(string id, NameValueCollection parameters) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return null;

            using (var wc = CreateWebClient(parameters))
            {
                try
                {
                    var xml = wc.DownloadString(string.Format(RequestFormat, host, urls[type], id, responseType));
                    return RedmineSerialization.FromXML<T>(xml);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
                return null;
            }
        }

        public void GetObjectAsync<T>(string id, NameValueCollection parameters) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return;

            using (var wc = CreateWebClient(parameters))
            {
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri(string.Format(RequestFormat, host, urls[type], id, responseType)), new AsyncToken { Method = "GetObject", ResponsetType = type, Parameter = id });
            }
        }

        /// <summary>
        /// Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
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
        public T CreateObject<T>(T obj) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return null;

            var xml = RedmineSerialization.ToXML(obj);

            if (string.IsNullOrEmpty(xml)) return null;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    var xmlRet = wc.UploadString(string.Format(Format, host, urls[type], responseType), xml);
                    return RedmineSerialization.FromXML<T>(xmlRet);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
            }
            return null;
        }

        public void CreateObjectAsync<T>(T obj) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return;

            var xml = RedmineSerialization.ToXML(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.UploadStringAsync(new Uri(string.Format(Format, host, urls[type], responseType)), POST, xml, new AsyncToken { Method = "CreateObject", ResponsetType = type, Parameter = obj });
            }
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
        public void UpdateObject<T>(string id, T obj) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return;

            var xml = RedmineSerialization.ToXML(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(RequestFormat, host, urls[type], id, responseType), PUT, xml);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
            }
        }

        public void UpdateObjectAsync<T>(string id, T obj) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return;

            var xml = RedmineSerialization.ToXML(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.UploadStringAsync(new Uri(string.Format(RequestFormat, host, urls[type], id, responseType)), PUT, xml, new AsyncToken { Method = "UpdateObject", ResponsetType = type, Parameter = obj });
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
                    wc.UploadString(string.Format(RequestFormat, host, urls[type], id, responseType), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    HandleWebException(webException, type.Name);
                }
            }
        }

        public void DeleteObjectAsync<T>(string id, NameValueCollection parameters) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(typeof(T))) return;

            using (var wc = CreateWebClient(parameters))
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.UploadStringAsync(new Uri(string.Format(RequestFormat, host, urls[type], id, responseType)), DELETE, string.Empty, new AsyncToken { Method = "DeleteObject", ResponsetType = type, Parameter = id });
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

            if (!string.IsNullOrEmpty(apiKey)) webClient.QueryString["key"] = apiKey;
            else
            {
                if (cache != null) webClient.Credentials = cache;
            }

            webClient.Headers.Add("Content-Type", "application/xml; charset=utf-8");
            webClient.Encoding = Encoding.UTF8;
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

        private static void HandleWebException(WebException exception, string objectName)
        {
            if (exception == null) return;

            switch (exception.Status)
            {
                case WebExceptionStatus.NameResolutionFailure: throw new RedmineException("Bad domain name!");
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.NotFound:
                            case (int)HttpStatusCode.Forbidden:
                                throw new RedmineException(response.StatusDescription);

                            case 422:
                                var errors = ReadWebExceptionResponse(exception.Response);
                                string message = string.Empty;
                                foreach (Error error in errors) message += error.Info + "\n";

                                throw new RedmineException(objectName + " has invalid or missing attribute parameters: " + message);

                            case (int)HttpStatusCode.NotAcceptable: throw new RedmineException();
                        }
                    }
                    break;

                default: throw new RedmineException(exception.Message);
            }
        }

        private static IEnumerable<Error> ReadWebExceptionResponse(WebResponse webResponse)
        {
            List<Error> errors = null;

            using (var stream = webResponse.GetResponseStream())
            {
                var str = new StreamReader(stream);

                var r = str.ReadToEnd();

                if (r.Trim().Length > 0)
                {
                    try
                    {
                        using (var text = new StringReader(r))
                        {
                            using (var xmlReader = new XmlTextReader(text))
                            {
                                xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                                xmlReader.Read();
                                xmlReader.Read();
                                errors = xmlReader.ReadElementContentAsCollection<Error>();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
                return errors;
            }
        }
    }

    internal class AsyncToken
    {
        public string Method { get; set; }
        public Type ResponsetType { get; set; }
        public object Parameter { get; set; }

    }

    public class AsyncEventArgs : EventArgs
    {
        public string Error { get; set; }
        public object Result { get; set; }
    }

}