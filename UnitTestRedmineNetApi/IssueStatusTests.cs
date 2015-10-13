using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class IssueStatusTests
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            var mimeFormat = (ConfigurationManager.AppSettings["mimeFormat"].Equals("xml")) ? MimeFormat.xml : MimeFormat.json;
            redmineManager = new RedmineManager(uri, apiKey, mimeFormat);
        }

        [TestMethod]
        public void RedmineIssueStatuses_ShouldGetAllIssueStatuses()
        {
            var issueStatuses = redmineManager.GetObjectList<IssueStatus>(null);

            Assert.IsTrue(issueStatuses.Count == 7);
        }
    }
}
