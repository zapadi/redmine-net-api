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
        #region Constants
        private const string projectId = "9";

        //issueCategory data for insert
        private const string newIssueCategoryName = "New category";
        private const int newIssueCategoryAsigneeId = 8;

        private const string issueCategoryIdToGet = "11";
        private const string issueCategoryAsigneeNameToGet = "Alina";

        private const string issueCategoryIdToUpdate = "15";
        private const string issueCategoryNameToUpdate = "New category updated";
        private const int issueCategoryAsigneeIdToUpdate = 2;

        private const string issueCategoryIdToDelete = "15";

        private const string issueCategoryIdToCompare = "11";
        private const string issueCategoryNameToCompare = "Issue compared";
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
        public void RedmineIssueCategories_ShouldGetAllIssueCategories()
        {
            var issueCategories = redmineManager.GetObjects<IssueCategory>(new NameValueCollection { { "project_id", projectId } });

            Assert.IsNotNull(issueCategories);
        }

        [TestMethod]
        public void RedmineIssueCategories_ShouldCreateIssueCategory()
        {
            IssueCategory issueCategory = new IssueCategory();
            issueCategory.Name = newIssueCategoryName;
            issueCategory.AsignTo = new IdentifiableName { Id = newIssueCategoryAsigneeId };

            IssueCategory savedIssueCategory = redmineManager.CreateObject<IssueCategory>(issueCategory, projectId);

            Assert.AreEqual(issueCategory.Name, savedIssueCategory.Name);
        }

        [TestMethod]
        public void RedmineIssueCategory_ShouldGetIssueCategoryById()
        {
            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryIdToGet, null);

            Assert.IsNotNull(issueCategory);
            Assert.IsTrue(issueCategory.AsignTo.Name.Contains(issueCategoryAsigneeNameToGet));
        }

        [TestMethod]
        public void RedmineIssueCategories_ShouldUpdateIssueCategory()
        {
            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryIdToUpdate, null);
            issueCategory.Name = issueCategoryNameToUpdate;
            issueCategory.AsignTo = new IdentifiableName { Id = issueCategoryAsigneeIdToUpdate };

            redmineManager.UpdateObject<IssueCategory>(issueCategoryIdToUpdate, issueCategory);

            IssueCategory updatedIssueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryIdToUpdate, null);

            Assert.AreEqual(issueCategory.Name, updatedIssueCategory.Name);
        }

        [TestMethod]
        public void RedmineIssueCategory_ShouldDeleteIssueCategory()
        {
            try
            {
                redmineManager.DeleteObject<IssueCategory>(issueCategoryIdToDelete, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Issue category could not be deleted.");
                return;
            }

            try
            {
                IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryIdToDelete, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void RedmineIssueCategory_ShouldCompare()
        {
            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(issueCategoryIdToCompare, null);
            IssueCategory issueCategoryToCompare = redmineManager.GetObject<IssueCategory>(issueCategoryIdToCompare, null);

            Assert.IsTrue(issueCategory.Equals(issueCategoryToCompare));

            issueCategoryToCompare.Name = issueCategoryNameToCompare;
            Assert.IsFalse(issueCategory.Equals(issueCategoryToCompare));
        }
        #endregion Tests
    }
}
