using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class IssueCategoryTests
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
        public void RedmineIssueCategories_ShouldGetAllIssueCategories()
        {
            string projectId = "9";

            var issueCategories = redmineManager.GetObjectList<IssueCategory>(new NameValueCollection { { "project_id", projectId } });

            Assert.IsTrue(issueCategories.Count == 1);
        }

        [TestMethod]
        public void RedmineIssueCategories_ShouldCreateIssueCategory()
        {
            IssueCategory issueCategory = new IssueCategory();
            issueCategory.Name = "Feature";
            issueCategory.AsignTo = new IdentifiableName { Id = 8 };

            IssueCategory savedIssueCategory = redmineManager.CreateObject<IssueCategory>(issueCategory, "9");

            Assert.AreEqual(issueCategory.Name, savedIssueCategory.Name);
        }

        [TestMethod]
        public void RedmineIssueCategory_ShouldGetIssueCategoryById()
        {
            var issueCategoryId = "12";

            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryId, null);

            Assert.IsNotNull(issueCategory);
            Assert.IsTrue(issueCategory.AsignTo.Name.Contains("Alina"));
        }

        [TestMethod]
        public void RedmineIssueCategories_ShouldUpdateIssueCategory()
        {
            var issueCategoryId = "12";

            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryId, null);
            issueCategory.Name = "Feature updated";
            issueCategory.AsignTo = new IdentifiableName { Id = 2 };

            redmineManager.UpdateObject<IssueCategory>(issueCategoryId, issueCategory);

            IssueCategory updatedIssueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryId, null);

            Assert.AreEqual(issueCategory.Name, updatedIssueCategory.Name);
        }

        [TestMethod]
        public void RedmineIssueCAtegory_ShouldDeleteIssueCategory()
        {
            var issueCategoryId = "12";

            try
            {
                redmineManager.DeleteObject<IssueCategory>(issueCategoryId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Issue category could not be deleted.");
                return;
            }

            try
            {
                IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }
    }
}
