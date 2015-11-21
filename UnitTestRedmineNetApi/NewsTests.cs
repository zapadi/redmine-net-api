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
        #region Constants
        private const string projectId = "6";
        #endregion Constants

        #region Properties
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        #endregion Properties

        #region Initialize
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
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void GetAllNews()
        {
            var result = redmineManager.GetObjectList<News>(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RedmineNews_ShouldGetSpecificProjectNews()
        {
            var news = redmineManager.GetObjectList<News>(new NameValueCollection { { "project_id", projectId } });

            Assert.IsNotNull(news);
        }

        [TestMethod]
        public void RedmineNews_ShouldCompare()
        {
            var projectNews = redmineManager.GetObjectList<News>(new NameValueCollection { { "project_id", projectId } });
            if (projectNews != null)
            {
               var news = projectNews[0];
               var newsToCompare = projectNews[0];

                Assert.IsTrue(news.Equals(newsToCompare));
            }
            else
                Assert.Inconclusive();
        }
        #endregion Tests
    }
}
