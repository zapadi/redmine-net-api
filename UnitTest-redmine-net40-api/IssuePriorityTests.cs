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
    public class IssuePriorityTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_ISSUE_PRIORITIES = 5;

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
        public void Should_Get_All_Issue_Priority()
        {
            var issuePriorities = redmineManager.GetObjects<IssuePriority>(null);

            Assert.IsNotNull(issuePriorities, "Get issue priorities returned null.");
            Assert.IsTrue(issuePriorities.Count == NUMBER_OF_ISSUE_PRIORITIES, "Issue priorities count != " + NUMBER_OF_ISSUE_PRIORITIES);
            CollectionAssert.AllItemsAreNotNull(issuePriorities, "Issue priorities list contains null items.");
            CollectionAssert.AllItemsAreUnique(issuePriorities, "Issue priorities items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(issuePriorities, typeof(IssuePriority), "Not all items are of type IssuePriority.");
            Assert.IsTrue(issuePriorities.Exists(ip => ip.IsDefault), "List does not contain a default issue priority.");
        }
    }
}
