using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class RedmineManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        public RedmineManager(string host, string login, string password)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineManager"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        public RedmineManager(string host, string apiKey)
        {

        }

        /// <summary>
        /// Desirializes the specified XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        private T Desirialize<T>(string xml) where T : class
        {
            var sr = new XmlSerializer(typeof (T));
            using (var str = new XmlTextReader(new StringReader(xml)))
            {
                str.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;
                return sr.Deserialize(str) as T;
            }
        }

    }
}