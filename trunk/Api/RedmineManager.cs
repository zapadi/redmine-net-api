/*
   Copyright 2011 Adrian Popescu, Dorin Huzum.

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

namespace Redmine.Net.Api
{
    /// <summary>
    /// The main class to access Redmine API.
    /// </summary>
    public class RedmineManager
    {
        private const string RequestFormat = "{0}/{1}/{2}.xml";
        private const string Format = "{0}/{1}.xml";
        private const string CurrentUserUri = "current";
        private const string PUT = "PUT";
        private const string POST = "POST";
        private const string DELETE = "DELETE";

        private readonly string apiKey, basicAuthorization;
        private readonly CredentialCache cache;
        private readonly string host;

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
            this.host = host;

            if (!verifyServerCert)
            {
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertValidate;
            }
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
                var xml = wc.DownloadString(string.Format(RequestFormat, host, urls[typeof(User)], CurrentUserUri));
                return Deserialize<User>(xml);
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

                var response = wc.UploadData(string.Format(Format, host, "uploads"), data);

                var responseString = Encoding.ASCII.GetString(response);

                return Deserialize<Upload>(responseString);
            }
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
                    wc.UploadString(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users"), POST, "<user_id>" + userId + "</user_id>");
                }
                catch (WebException webException)
                {
                    ParseWebClientException(webException, "User");
                }
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
                    wc.UploadString(string.Format(RequestFormat, host, urls[typeof(Group)], groupId + "/users/" + userId), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    ParseWebClientException(webException, "User");
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
        public IList<T> GetObjectList<T>(NameValueCollection parameters) where T : class
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

                        result = wc.DownloadString(string.Format("{0}/projects/{1}/{2}.xml", host, projectId, urls[type]));
                    }
                    else
                    {
                        result = wc.DownloadString(string.Format(Format, host, urls[type]));
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
                    ParseWebClientException(webException, type.Name);
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
                    var xml = wc.DownloadString(string.Format(RequestFormat, host, urls[type], id));
                    return Deserialize<T>(xml);
                }
                catch (WebException webException)
                {
                    ParseWebClientException(webException, type.Name);
                }
                return null;
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
        public void CreateObject<T>(T obj) where T : class
        {
            var type = typeof(T);

            if (!urls.ContainsKey(type)) return;

            var xml = Serialize(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(Format, host, urls[type]), xml);
                }
                catch (WebException webException)
                {
                    ParseWebClientException(webException, type.Name);
                }
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

            var xml = Serialize(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                try
                {
                    wc.UploadString(string.Format(RequestFormat, host, urls[type], id), PUT, xml);
                }
                catch (WebException webException)
                {
                    ParseWebClientException(webException, type.Name);
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
                    wc.UploadString(string.Format(RequestFormat, host, urls[type], id), DELETE, string.Empty);
                }
                catch (WebException webException)
                {
                    ParseWebClientException(webException, type.Name);
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

            if (!string.IsNullOrEmpty(apiKey)) webClient.QueryString["key"] = apiKey;
            else
            {
                if (cache != null) webClient.Credentials = cache;
            }

            webClient.Headers.Add("Content-Type", "text/xml; charset=utf-8");
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

        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The System.String that contains the XML document.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string Serialize<T>(T obj) where T : class
        {
            var xws = new XmlWriterSettings { OmitXmlDeclaration = true };

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, xws))
                {
                    var sr = new XmlSerializer(typeof(T));
                    sr.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize.</typeparam>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <returns>The System.Object being deserialized.</returns>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        public static T Deserialize<T>(string xml) where T : class
        {
            using (var text = new StringReader(xml))
            {
                var sr = new XmlSerializer(typeof(T));
                return sr.Deserialize(text) as T;
            }
        }

        private static void ParseWebClientException(WebException exception, string objectName)
        {
            if (exception == null) return;

            var errors = ReadWebExceptionResponse(exception.Response);

            switch (exception.Status)
            {
                case WebExceptionStatus.NameResolutionFailure: throw new RedmineException("Bad domain name!");
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.NotFound:
                                throw new RedmineException(response.StatusDescription + (errors != null ? "\n Errors: " + errors : null));
                            case (int)HttpStatusCode.Forbidden:
                                throw new RedmineException(response.StatusDescription + (errors != null ? "\n Errors: " + errors : null));
                            case 422:
                                string message = string.Empty;
                                foreach (Error error in errors) message += error.Info + "\n";
                                throw new RedmineException(objectName + " has invalid or missing attribute parameters: " + message);//+ (errors != null ? "\n" + errors.Aggregate(string.Empty, (s, error) => s + error.Info + "\n") : null));
                            case (int)HttpStatusCode.NotAcceptable:
                                throw new RedmineException();
                        }
                    }
                    break;
                case WebExceptionStatus.UnknownError: throw new RedmineException(exception.Message);

                default: throw exception;
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
}