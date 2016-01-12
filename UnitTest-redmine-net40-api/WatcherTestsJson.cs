using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class WatcherTestsJson
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [TestMethod]
        public void Should_Add_Watcher()
        {
            redmineManager.AddWatcher(44, 8);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Remove_Watcher()
        {
            redmineManager.RemoveWatcher(44, 8);
            Assert.Inconclusive();
        }
    }
}