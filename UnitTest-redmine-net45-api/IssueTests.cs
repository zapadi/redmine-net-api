using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using Redmine.Net.Api;
using System.Diagnostics;
using Redmine.Net.Api.Extensions;
using System.Collections.Specialized;

namespace UnitTest_redmine_net45_api
{
     [TestClass]
    public class IssueTests
    {
         private RedmineManager redmineManager;

         private const int watcherIssueId = 19;
         private const int watcherUserId = 2;

         [TestInitialize]
         public void Initialize()
         {
             SetMimeTypeXML();
             SetMimeTypeJSON();
             redmineManager.UseTraceLog();
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
             await redmineManager.AddWatcherAsync(watcherIssueId, watcherUserId);

             Issue issue = await redmineManager.GetObjectAsync<Issue>(watcherIssueId.ToString(), new NameValueCollection { { "include", "watchers" } });

             Assert.IsNotNull(issue, "Get issue returned null.");
             Assert.IsTrue(issue.Watchers.Count == 1, "Number of watchers != 1");
             CollectionAssert.AllItemsAreNotNull(issue.Watchers.ToList(), "Watchers contains null items.");
             CollectionAssert.AllItemsAreUnique(issue.Watchers.ToList(), "Watcher items are not unique.");
             Assert.IsTrue(((List<Watcher>)issue.Watchers).Find(w => w.Id == watcherUserId) != null, "Watcher not added to issue.");
         }

         [TestMethod]
         public async Task Should_Remove_Watcher_From_Issue()
         {
             await redmineManager.RemoveWatcherAsync(watcherIssueId, watcherUserId);

             Issue issue = await redmineManager.GetObjectAsync<Issue>(watcherIssueId.ToString(), new NameValueCollection { { "include", "watchers" } });

             Assert.IsTrue(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == watcherUserId) == null);
         }
    }
}
