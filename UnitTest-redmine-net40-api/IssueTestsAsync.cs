using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using System.Diagnostics;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class IssueTestsAsync
    {
        private RedmineManager redmineManager;

        private const int ISSUE_ID = 1;
        private const int USER_ID = 5;

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }

        [TestMethod]
        public async Task Should_Add_Watcher_To_Issue()
        {
            await redmineManager.AddWatcherToIssueAsync(ISSUE_ID, USER_ID);

            Issue issue = await redmineManager.GetObjectAsync<Issue>(ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS} });

            Assert.IsNotNull(issue, "Get issue returned null.");
            Assert.IsTrue(issue.Watchers.Count == 1, "Number of watchers != 1");
            CollectionAssert.AllItemsAreNotNull(issue.Watchers.ToList(), "Watchers contains null items.");
            CollectionAssert.AllItemsAreUnique(issue.Watchers.ToList(), "Watcher items are not unique.");
            Assert.IsTrue(((List<Watcher>)issue.Watchers).Find(w => w.Id == USER_ID) != null, "Watcher not added to issue.");
        }

        [TestMethod]
        public async Task Should_Remove_Watcher_From_Issue()
        {
            await redmineManager.RemoveWatcherFromIssueAsync(ISSUE_ID, USER_ID);

            Issue issue = await redmineManager.GetObjectAsync<Issue>(ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

            Assert.IsTrue(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == USER_ID) == null);
        }
    }
}
