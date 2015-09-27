using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class NewsTests
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            redmineManager = new RedmineManager(uri, apiKey);
        }


        [TestMethod]
        public void GetAllNews()
        {
            var result = redmineManager.GetObjectList<News>(null);

            Assert.IsNotNull(result);
        }
    }
}
