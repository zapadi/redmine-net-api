using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class MembershipTests
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
        public void GetAllMemberships()
        {

        }
    }
}