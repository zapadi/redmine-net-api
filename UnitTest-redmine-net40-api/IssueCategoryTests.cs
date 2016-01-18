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

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class IssueCategoryTests
    {
        private RedmineManager redmineManager;

        private const string PROJECT_ID = "redmine-test";
        private const int NUMBER_OF_ISSUE_CATEGORIES = 2;

        //issueCategory data for insert
        private const string NEW_ISSUE_CATEGORY_NAME = "Test category";
        private const int NEW_ISSUE_CATEGORY_ASIGNEE_ID = 5;

        private const string ISSUE_CATEGORY_ID_TO_GET = "3";
        private const string ISSUE_CATEGORY_NAME_TO_GET = "Test category";
        private const string ISSUE_CATEGORY_PROJECT_NAME_TO_GET = "redmine-test";
        private const string ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET = "Alina";

        private const string ISSUE_CATEGORY_ID_TO_UPDATE = "3";
        private const string ISSUE_CATEGORY_NAME_TO_UPDATE = "Category updated";
        private const int ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE = 1;

        private const string ISSUE_CATEGORY_ID_TO_DELETE = "3";

        private const string ISSUE_CATEGORY_ID_TO_COMPARE = "1";

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.xml);
        }

        [TestMethod]
        public void Should_Get_All_IssueCategories_By_ProjectId()
        {
            var issueCategories = redmineManager.GetObjects<IssueCategory>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

            Assert.IsNotNull(issueCategories, "Get issue categories returned null.");
            CollectionAssert.AllItemsAreInstancesOfType(issueCategories, typeof(IssueCategory), "Not all items are of type IssueCategory.");
            CollectionAssert.AllItemsAreNotNull(issueCategories, "Issue categories contains null items.");
            CollectionAssert.AllItemsAreUnique(issueCategories, "Issue categories items are not unique.");
            Assert.IsTrue(issueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Number of issue categories != " + NUMBER_OF_ISSUE_CATEGORIES);   
        }

        [TestMethod]
        public void Should_Create_IssueCategory()
        {
            IssueCategory issueCategory = new IssueCategory();
            issueCategory.Name = NEW_ISSUE_CATEGORY_NAME;
            issueCategory.AsignTo = new IdentifiableName { Id = NEW_ISSUE_CATEGORY_ASIGNEE_ID };

            IssueCategory savedIssueCategory = redmineManager.CreateObject<IssueCategory>(issueCategory, PROJECT_ID);

            Assert.IsNotNull(savedIssueCategory, "Create issue category returned null.");
            Assert.AreEqual(savedIssueCategory.Name, NEW_ISSUE_CATEGORY_NAME, "Saved issue category name is invalid.");
        }

        [TestMethod]
        public void Should_Get_IssueCategory_By_Id()
        {
            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_GET, null);

            Assert.IsNotNull(issueCategory, "Get issue category returned null.");
            Assert.AreEqual(issueCategory.Name, ISSUE_CATEGORY_NAME_TO_GET, "Issue category name is invalid.");
            Assert.IsNotNull(issueCategory.AsignTo, "Issue category asignee is null.");
            Assert.IsTrue(issueCategory.AsignTo.Name.Contains(ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET), "Asignee name is invalid.");
            Assert.IsNotNull(issueCategory.Project, "Issue category project is null.");
            Assert.IsTrue(issueCategory.Project.Name.Equals(ISSUE_CATEGORY_PROJECT_NAME_TO_GET), "Project name is invalid.");
        }

        [TestMethod]
        public void Should_Update_IssueCategory()
        {
            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, null);
            issueCategory.Name = ISSUE_CATEGORY_NAME_TO_UPDATE;
            issueCategory.AsignTo = new IdentifiableName { Id = ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE };

            redmineManager.UpdateObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, issueCategory);

            IssueCategory updatedIssueCategory = redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, null);

            Assert.IsNotNull(updatedIssueCategory, "Get updated issue category returned null.");
            Assert.AreEqual(updatedIssueCategory.Name, ISSUE_CATEGORY_NAME_TO_UPDATE, "Issue category name was not updated.");
            Assert.IsNotNull(updatedIssueCategory.AsignTo, "Issue category assignee is null.");
            Assert.AreEqual(updatedIssueCategory.AsignTo.Id, ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE, "Issue category asignee was not updated.");
        }

        [TestMethod]
        public void Should_Delete_IssueCategory()
        {
            try
            {
                redmineManager.DeleteObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Issue category could not be deleted.");
                return;
            }

            try
            {
                IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void Should_Compare_IssueCAtegories()
        {
            IssueCategory issueCategory = redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_COMPARE, null);
            IssueCategory issueCategoryToCompare = redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_COMPARE, null);

            Assert.IsTrue(issueCategory.Equals(issueCategoryToCompare));
        }
    }
}
