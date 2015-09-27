using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class ProjectTests
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
        public void GetProject_WithAll_AssociatedData()
        {
            var result = redmineManager.GetObject<Project>("9", new NameValueCollection()
            {
                {"include","trackers, issue_categories, enabled_modules" }
            });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Project));
            Assert.IsNotNull(result.Trackers, "result.Trackers != null");
            Assert.IsNotNull(result.IssueCategories, "result.IssueCategories != null");
            Assert.IsNotNull(result.EnabledModules,"result.EnabledModules != null");
        }

        [TestMethod]
        public void GetAllProjects_WithAll_AssociatedData()
        {
            IList<Project> result = redmineManager.GetTotalObjectList<Project>(new NameValueCollection()
            {
                {"include", "trackers, issue_categories, enabled_modules"}
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetProject_News()
        {
            var result = redmineManager.GetObjectList<News>(new NameValueCollection()
            {
                {"project_id","9" }
            });

            Assert.IsNotNull(result);
        }
    }
}
