using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class IssueStatusTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_ISSUE_STATUSES = 6;
        private const bool EXISTS_CLOSED_ISSUE_STATUSES = true;
        private const bool EXISTS_DEFAULT_ISSUE_STATUSES = false;

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
        public void Should_Get_All_Issue_Statuses()
        {
            var issueStatuses = redmineManager.GetObjects<IssueStatus>(null);

            Assert.IsNotNull(issueStatuses, "Get issue statuses returned null.");
            Assert.IsTrue(issueStatuses.Count == NUMBER_OF_ISSUE_STATUSES, "Issue statuses count != " + NUMBER_OF_ISSUE_STATUSES);
            CollectionAssert.AllItemsAreNotNull(issueStatuses, "Issue statuses list contains null items.");
            CollectionAssert.AllItemsAreUnique(issueStatuses, "Issue statuses items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(issueStatuses, typeof(IssueStatus), "Not all items are of type IssueStatus.");

            Assert.IsTrue(issueStatuses.Exists(i => i.IsClosed) == EXISTS_CLOSED_ISSUE_STATUSES, EXISTS_CLOSED_ISSUE_STATUSES ? "Closed issue statuses were expected to exist." : "Closed issue statuses were not expected to exist.");
            Assert.IsTrue(issueStatuses.Exists(i => i.IsDefault) == EXISTS_DEFAULT_ISSUE_STATUSES, EXISTS_DEFAULT_ISSUE_STATUSES ? "Default issue statuses were expected to exist." : "Default issue statuses were not expected to exist.");
        }
    }
}
