using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class UserTestsJson
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
        public void Should_Add_User()
        {
            redmineManager.AddUser(44, 8);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Remove_User()
        {
            redmineManager.DeleteUser(44, 8);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Return_Current_User()
        {
            var result = redmineManager.GetCurrentUser();
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Return_All_Users()
        {
            var result = redmineManager.GetUsers();
            Assert.Inconclusive();
        }
    }
}