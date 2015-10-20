using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;
using System.Diagnostics;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class NewsTests
    {
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;

        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            apiKey = ConfigurationManager.AppSettings["apiKey"];

            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.xml);
        }

        [TestMethod]
        public void GetAllNews()
        {
            var result = redmineManager.GetObjectList<News>(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RedmineNews_ShouldGetSpecificProjectNews()
        {
            int projectId = 6;

            var news = redmineManager.GetObjectList<News>(new NameValueCollection { { "project_id", projectId.ToString() } });

            Assert.IsNotNull(news);
        }
    }
}
