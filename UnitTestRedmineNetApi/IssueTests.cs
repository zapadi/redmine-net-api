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
    public class IssueTests
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
        public void RedmineIssues_ShouldReturnAllIssues()
        {
            var issues = redmineManager.GetObjectList<Issue>(null);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnPaginatedIssues()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "offset", "1" }, { "limit", "3" }, { "sort", "id:desc" } });

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count <= 3, "number of issues <= 3");
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByProjectId()
        {
            var projectId = "9";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "project_id", projectId } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesBySubprojectId()
        {
            var subprojectId = "10";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "subproject_id", subprojectId } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssues_ByProjectWithoutSubprojects()
        {
            var projectId = "9";
            var subprojectId = "!*";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "project_id", projectId }, { "subproject_id", subprojectId } });
            
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByTracker()
        {
            var trackerId = "4";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "tracker_id", trackerId } });
           
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByStatus()
        {
            var status_id = "*";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "status_id", status_id } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByAsignee()
        {
            var assigned_to_id = "me";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "assigned_to_id", assigned_to_id } });
          
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByCustomField()
        {
            var cf_13 = "xxx";
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "cf_13", cf_13 } });

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnIssueById()
        {
            var issueId = "19";

            var issue = redmineManager.GetObject<Issue>(issueId,  new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.IsNotNull(issue);
           //TODO: add conditions for all associated data
        }

        [TestMethod]
        public void RedmineIssues_ShouldCreateIssue()
        {
            Issue issue = new Issue();
            issue.Project = new Project { Id = 10 };
            issue.Tracker = new IdentifiableName { Id = 4 };
            issue.Status = new IdentifiableName { Id = 5 };
            issue.Priority = new IdentifiableName { Id = 8 };
            issue.Subject = "Issue created using Rest API";
            issue.Description = "Issue description...";
            issue.Category = new IdentifiableName { Id = 11 };
            issue.FixedVersion = new IdentifiableName { Id = 9 };
            issue.AssignedTo = new IdentifiableName { Id = 8 };
            issue.ParentIssue = new IdentifiableName { Id = 19 };
            issue.CustomFields = new List<IssueCustomField>();
            issue.CustomFields.Add(new IssueCustomField { Id = 13, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "Issue custom field completed" } } });

            issue.IsPrivate = true;
            issue.EstimatedHours = 12;
            issue.StartDate = DateTime.Now;
            issue.DueDate = DateTime.Now.AddMonths(1);
            issue.Watchers = new List<Watcher>();

            issue.Watchers.Add(new Watcher { Id = 8 });
            issue.Watchers.Add(new Watcher { Id = 2 });
            Issue savedIssue = redmineManager.CreateObject<Issue>(issue);

            Assert.AreEqual(issue.Subject, savedIssue.Subject);
        }

        [TestMethod]
        public void RedmineIssues_ShouldUpdateIssue()
        {
            var issueId = "75";

            var issue = redmineManager.GetObject<Issue>(issueId, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });
            issue.Subject = "Issue updated subject";
            issue.Description = null;
            issue.StartDate = null;
            issue.DueDate = DateTime.Now.AddDays(10);
            issue.Project.Id = 10;
            issue.Tracker.Id = 3;
            issue.Priority.Id = 9;
            issue.Category.Id = 10;
            issue.AssignedTo.Id = 2;
            issue.ParentIssue.Id = 20;

            issue.CustomFields.Add(new IssueCustomField { Id = 13, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "Another custom field completed" } } });
            issue.EstimatedHours = 22;
            issue.Notes = "A lot is changed";
            issue.PrivateNotes = true;

            redmineManager.UpdateObject<Issue>(issueId, issue);

            var updatedIssue = redmineManager.GetObject<Issue>(issueId, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.AreEqual(issue.Subject, issue.Subject);
        }

        [TestMethod]
        public void RedmineIssues_ShouldDeleteIssue()
        {
            var issueId = "75";

            try
            {
                redmineManager.DeleteObject<Issue>(issueId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Issue could not be deleted.");
                return;
            }

            try
            {
                Issue issue = redmineManager.GetObject<Issue>(issueId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void RedmineIssues_ShouldAddWatcher()
        {
            redmineManager.AddWatcher(19, 2);

            Issue issue =  redmineManager.GetObject<Issue>("19", new NameValueCollection { { "include", "watchers" } });

            Assert.IsTrue(((List<Watcher>)issue.Watchers).Find(w => w.Id == 2) != null);
        }

        [TestMethod]
        public void RedmineIssues_ShouldRemoveWatcher()
        {
            redmineManager.RemoveWatcher(19, 2);

            Issue issue = redmineManager.GetObject<Issue>("19",  new NameValueCollection { { "include", "watchers" } });

            Assert.IsTrue(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == 2) == null);
        }

    }
}
