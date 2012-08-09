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
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class RedmineManager
    {
        private const string REQUESTFORMAT = "{0}/{1}/{2}.xml";
        private const string FORMAT = "{0}/{1}.xml";
        private const string CURRENT_USER_URI = "current";
        private readonly string host, apiKey, basicAuthorization;
        private readonly CredentialCache cache;
        private readonly Dictionary<Type, String> urls = new Dictionary<Type, string>
                                                             {
                                                                 { typeof(Issue), "issues" },
                                                                 { typeof(Project), "projects" },
                                                                 { typeof(User), "users" },
                                                                 { typeof(News), "news" },
                                                                 { typeof(Query), "queries" },
                                                                 { typeof(Version), "versions"},
                                                                 { typeof(Attachment),"attachments"},
                                                                 { typeof(IssueRelation), "relations"},
                                                                 { typeof(TimeEntry),"time_entries"},
                                                                 { typeof(IssueStatus),"issue_statuses"},
                                                                 { typeof(Tracker),"trackers"},
                                                                 { typeof(IssueCategory),"issue_categories"},
                                                                 { typeof(Role),"roles"},
                                                                 { typeof(ProjectMembership),"memberships"},
                                                                 { typeof(Group),"groups"}
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
        public User GetCurrentUser(NameValueCollection parameters = null)
        {
            using (var wc = CreateWebClient(parameters))
            {
                var xml = wc.DownloadString(string.Format(REQUESTFORMAT, host, urls[typeof(User)], CURRENT_USER_URI));
                return Deserialize<User>(xml);
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
                wc.UploadString(string.Format(REQUESTFORMAT, host, urls[typeof(Group)], groupId + "/users"), "POST", "<user_id>" + userId.ToString() + "</user_id>");
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
                wc.UploadString(string.Format(REQUESTFORMAT, host, urls[typeof(Group)], groupId + "/users/" + userId), "DELETE", string.Empty);
            }
        }

        /// <summary>
        /// Gets the object list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public IList<T> GetObjectList<T>(NameValueCollection parameters) where T : class
        {
            if (!urls.ContainsKey(typeof(T))) return null;

            using (var wc = CreateWebClient(parameters))
            {
                var type = typeof(T);
                string result;

                if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
                {
                    string projectId = null;
                    if (parameters != null)
                        projectId = parameters.Get("project_id");

                    result = wc.DownloadString(string.Format("{0}/projects/{1}/{2}.xml", host, projectId, urls[type]));
                }
                else
                {
                    result = wc.DownloadString(string.Format(FORMAT, host, urls[type]));
                }

                using (var text = new StringReader(result))
                {
                    using (var xmlReader = new XmlTextReader(text))
                    {
                        xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                        xmlReader.Read();
                        xmlReader.Read();
                        return xmlReader.ReadElementContentAsCollection<T>();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the object list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        public IList<T> GetObjectList<T>(NameValueCollection parameters, out int totalCount) where T : class
        {
            totalCount = -1;
            if (!urls.ContainsKey(typeof(T))) return null;

            using (var wc = CreateWebClient(parameters))
            {
                var xml = wc.DownloadString(string.Format(FORMAT, host, urls[typeof(T)]));
                using (var text = new StringReader(xml))
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

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public T GetObject<T>(string id, NameValueCollection parameters) where T : class
        {
            if (!urls.ContainsKey(typeof(T))) return null;

            using (var wc = CreateWebClient(parameters))
            {
                var xml = wc.DownloadString(string.Format(REQUESTFORMAT, host, urls[typeof(T)], id));
                return Deserialize<T>(xml);
            }
        }

        /// <summary>
        /// Creates the object.
        /// When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable Entity response. That means that the object could not be created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        public void CreateObject<T>(T obj) where T : class
        {
            if (!urls.ContainsKey(typeof(T))) return;

            var xml = Serialize(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                string result = wc.UploadString(string.Format(FORMAT, host, urls[typeof(T)]), xml);
            }
        }

        /// <summary>
        /// Upload data on server.
        /// </summary>
        /// <param name="data">Data which will be uploaded on server</param>
        /// <returns>Returns 'Upload' object with inialized 'Token' by server response.</returns>
        public Upload UploadData(byte[] data)
        {
            using (WebClient wc = new WebClient())
            {
                wc.UseDefaultCredentials = false;

                wc.Headers.Add("Content-Type", "application/octet-stream");
                // Workaround - it seems that WebClient doesn't send credentials in each POST request
                wc.Headers.Add("Authorization", basicAuthorization);

                byte[] response = wc.UploadData(string.Format(FORMAT, host, "uploads"), data);

                string responseString = Encoding.ASCII.GetString(response);

                return Deserialize<Upload>(responseString);
            }
        }

        /// <summary>
        /// Updates the object.
        /// When trying to update an object with invalid or missing attribute parameters, you will get a 422 Unprocessable Entity response. That means that the object could not be updated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The id.</param>
        /// <param name="obj">The obj.</param>
        public void UpdateObject<T>(string id, T obj) where T : class
        {
            if (!urls.ContainsKey(typeof(T))) return;

            var xml = Serialize(obj);

            if (string.IsNullOrEmpty(xml)) return;

            using (var wc = CreateWebClient(null))
            {
                string result = wc.UploadString(string.Format(REQUESTFORMAT, host, urls[typeof(T)], id), "PUT", xml);
            }
        }

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The id.</param>
        /// <param name="parameters">The parameters.</param>
        public void DeleteObject<T>(string id, NameValueCollection parameters) where T : class
        {
            if (!urls.ContainsKey(typeof(T))) return;

            using (var wc = CreateWebClient(parameters))
            {
                wc.UploadString(string.Format(REQUESTFORMAT, host, urls[typeof(T)], id), "DELETE", string.Empty);
            }
        }

        /// <summary>
        /// Creates the web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected WebClient CreateWebClient(NameValueCollection parameters)
        {
            var webClient = new WebClient();

            if (parameters != null)
                webClient.QueryString = parameters;

            if (!string.IsNullOrEmpty(apiKey))
            {
                webClient.QueryString["key"] = apiKey;
            }
            else if (cache != null)
            {
                webClient.Credentials = cache;
            }

            webClient.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            webClient.Encoding = Encoding.UTF8;
            return webClient;
        }

        //This is to take care of SSL certification validation which are not issued by Trusted Root CA. Recommended for testing  only.
        protected bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //Cert Validation Logic
            return true;
        }

        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        protected static string Serialize<T>(T obj) where T : class
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
        /// Deserializes the specified XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml) where T : class
        {
            using (var text = new StringReader(xml))
            {
                var sr = new XmlSerializer(typeof(T));
                return sr.Deserialize(text) as T;
            }
        }
    }
}