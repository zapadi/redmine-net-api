using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class RedmineManager
    {
        private readonly string host, login, password, apiKey;
        private readonly Dictionary<Type, String> urls = new Dictionary<Type, string>
                                                             {
                                                                 { typeof(Issue), "issues.xml" },
                                                                 { typeof(Project), "projects.xml" },
                                                                 { typeof(User), "users.xml" },
                                                                 { typeof(News), "news.xml" },
                                                                 { typeof(Query), "queries.xml" },
                                                             };

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        public RedmineManager(string host, string login, string password)
        {
            this.host = host;
            this.login = login;
            this.password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        public RedmineManager(string host, string apiKey)
        {
            this.host = host;
            this.apiKey = apiKey;
        }

        public IList<T> GetObjectList<T>(NameValueCollection filters) where T : class
        {
            if (!urls.ContainsKey(typeof(T))) return null;

            var wc = new WebClient();
            if (filters != null) wc.QueryString = filters;
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new NotSupportedException();
            }else
            {
                wc.QueryString["key"] = apiKey;
            }

            var xml = wc.DownloadString(host + "/" + urls[typeof(T)]);
            var xmlReader = new XmlTextReader(new StringReader(xml));
            xmlReader.Read();xmlReader.Read();
            return xmlReader.ReadElementContentAsCollection<T>();
        }
    }
}