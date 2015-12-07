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
        #region Constants
        //filters
        private const int numberOfPaginatedIssues = 3;
        private const string projectId = "9";
        private const string subprojectId = "10";
        private const string allSubprojects = "!*";
        private const string trackerId = "4";
        private const string status_id = "*";
        private const string assigned_to_id = "me";
        private const string customFieldName = "cf_13";
        private const string customFieldValue = "xxx";
        private const string issueId = "19";

        //issue data - used for create
        private const int newIssueProjectId = 10;
        private const int newIssueTrackerId = 4;
        private const int newIssueStatusId = 5;
        private const int newIssuePriorityId = 8;
        private const string newIssueSubject = "Issue created using Rest API";
        private const string newIssueDescription = "Issue description...";
        private const int newIssueCategoryId = 11;
        private const int newIssueFixedVersionId = 9;
        private const int newIssueAssignedToId = 8;
        private const int newIssueParentIssueId = 19;
        private const int newIssueCustomFieldId = 13;
        private const string newIssueCustomFieldValue = "Issue custom field completed";
        private const bool newIssueIsPrivate = true;
        private const int newIssueEstimatedHours = 12;
        private DateTime newIssueStartDate = DateTime.Now;
        private DateTime newIssueDueDate = DateTime.Now.AddDays(10);
        private const int newIssueFirstWatcherId = 2;
        private const int newIssueSecondWatcherId = 8;

        //issue data - used for update
        private const string updatedIssueId = "76";
        private const string updatedIssueSubject = "Issue updated subject";
        private const string updatedIssueDescription = null;
        private DateTime? updatedIssueStartDate = null;
        private DateTime updatedIssueDueDate = DateTime.Now.AddMonths(1);
        private const int updatedIssueProjectId = 10;
        private const int updatedIssueTrackerId = 3;
        private const int updatedIssuePriorityId = 9;
        private const int updatedIssueCategoryId = 10;
        private const int updatedIssueAssignedToId = 2;
        private const int updatedIssueParentIssueId = 20;
        private const int updatedIssueCustomFieldId = 13;
        private const string updatedIssueCustomFieldValue = "Another custom field completed";
        private const int updatedIssueEstimatedHours = 23;
        private const string updatedIssueNotes = "A lot is changed";
        private const bool updatedIssuePrivateNotes = true;

        private const string deletedIssueId = "76";

        //watcher
        private  const int watcherIssueId = 19;
        private const int watcherUserId = 2;

        //issue data - for clone
        private const string issueToCloneSubject = "Issue to clone";
        private const int issueToCloneCustomFieldId = 13;
        private const string issueToCloneCustomFieldValue = "Issue to clone custom field value";
        private const int clonedIssueCustomFieldId = 14;
        private const string clonedIssueCustomFieldValue = "Cloned issue custom field value";
        #endregion Constants

        #region Properties
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        #endregion Properties

        #region Initializes
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
        public void RedmineIssues_ShouldReturnAllIssues()
        {
            var issues = redmineManager.GetObjectList<Issue>(null);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnPaginatedIssues()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "offset", "1" }, { "limit", numberOfPaginatedIssues.ToString() }, { "sort", "id:desc" } });

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count <= numberOfPaginatedIssues, "number of issues <= "+ numberOfPaginatedIssues.ToString());
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByProjectId()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "project_id", projectId } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesBySubprojectId()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "subproject_id", subprojectId } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssues_ByProjectWithoutSubprojects()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "project_id", projectId }, { "subproject_id", allSubprojects } });
            
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByTracker()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "tracker_id", trackerId } });
           
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByStatus()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "status_id", status_id } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByAsignee()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { "assigned_to_id", assigned_to_id } });
          
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByCustomField()
        {
            var issues = redmineManager.GetObjectList<Issue>(new NameValueCollection { { customFieldName, customFieldValue } });

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnIssueById()
        {
            var issue = redmineManager.GetObject<Issue>(issueId,  new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.IsNotNull(issue);
           //TODO: add conditions for all associated data
        }

        [TestMethod]
        public void RedmineIssues_ShouldCreateIssue()
        {
            Issue issue = new Issue();
            issue.Project = new Project { Id = newIssueProjectId };
            issue.Tracker = new IdentifiableName { Id = newIssueTrackerId };
            issue.Status = new IdentifiableName { Id = newIssueStatusId };
            issue.Priority = new IdentifiableName { Id = newIssuePriorityId };
            issue.Subject = newIssueSubject;
            issue.Description = newIssueDescription;
            issue.Category = new IdentifiableName { Id = newIssueCategoryId };
            issue.FixedVersion = new IdentifiableName { Id = newIssueFixedVersionId };
            issue.AssignedTo = new IdentifiableName { Id = newIssueAssignedToId };
            issue.ParentIssue = new IdentifiableName { Id = newIssueParentIssueId };
            issue.CustomFields = new List<IssueCustomField>();
            issue.CustomFields.Add(new IssueCustomField { Id = newIssueCustomFieldId, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = newIssueCustomFieldValue } } });

            issue.IsPrivate = newIssueIsPrivate;
            issue.EstimatedHours = newIssueEstimatedHours;
            issue.StartDate = newIssueStartDate;
            issue.DueDate = newIssueDueDate;
            issue.Watchers = new List<Watcher>();

            issue.Watchers.Add(new Watcher { Id = newIssueFirstWatcherId });
            issue.Watchers.Add(new Watcher { Id = newIssueSecondWatcherId });
            Issue savedIssue = redmineManager.CreateObject<Issue>(issue);

            Assert.AreEqual(issue.Subject, savedIssue.Subject);
        }

        [TestMethod]
        public void RedmineIssues_ShouldUpdateIssue()
        {
            var issue = redmineManager.GetObject<Issue>(updatedIssueId, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });
            issue.Subject = updatedIssueSubject;
            issue.Description = updatedIssueDescription;
            issue.StartDate = updatedIssueStartDate;
            issue.DueDate = updatedIssueDueDate;
            issue.Project.Id = updatedIssueProjectId;
            issue.Tracker.Id = updatedIssueTrackerId;
            issue.Priority.Id = updatedIssuePriorityId;
            issue.Category.Id = updatedIssueCategoryId;
            issue.AssignedTo.Id = updatedIssueAssignedToId;
            issue.ParentIssue.Id = updatedIssueParentIssueId;

            issue.CustomFields.Add(new IssueCustomField { Id = updatedIssueCustomFieldId, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = updatedIssueCustomFieldValue } } });
            issue.EstimatedHours = updatedIssueEstimatedHours;
            issue.Notes = updatedIssueNotes;
            issue.PrivateNotes = updatedIssuePrivateNotes;

            redmineManager.UpdateObject<Issue>(updatedIssueId, issue);

            var updatedIssue = redmineManager.GetObject<Issue>(updatedIssueId, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.AreEqual(issue.Subject, issue.Subject);
        }

        [TestMethod]
        public void RedmineIssues_ShouldDeleteIssue()
        {
            try
            {
                redmineManager.DeleteObject<Issue>(deletedIssueId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Issue could not be deleted.");
                return;
            }

            try
            {
                Issue issue = redmineManager.GetObject<Issue>(deletedIssueId, null);
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
            redmineManager.AddWatcher(watcherIssueId, watcherUserId);

            Issue issue =  redmineManager.GetObject<Issue>(watcherIssueId.ToString(), new NameValueCollection { { "include", "watchers" } });

            Assert.IsTrue(((List<Watcher>)issue.Watchers).Find(w => w.Id == watcherUserId) != null);
        }

        [TestMethod]
        public void RedmineIssues_ShouldRemoveWatcher()
        {
            redmineManager.RemoveWatcher(watcherIssueId, watcherUserId);

            Issue issue = redmineManager.GetObject<Issue>(watcherIssueId.ToString(),  new NameValueCollection { { "include", "watchers" } });

            Assert.IsTrue(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == watcherUserId) == null);
        }

        [TestMethod]
        public void RedmineIssues_ShouldCompare()
        {
            var issue = redmineManager.GetObject<Issue>(issueId, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });
            var issueToCompare = redmineManager.GetObject<Issue>(issueId, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.IsTrue(issue.Equals(issueToCompare));
        }

        [TestMethod]
        public void RedmineIssues_ShouldClone()
        {
            Issue issueToClone, clonedIssue;
            issueToClone = new Issue();
            issueToClone.Subject = issueToCloneSubject;
            issueToClone.CustomFields = new List<IssueCustomField>();
            
            issueToClone.CustomFields.Add(new IssueCustomField
            {
                Id = issueToCloneCustomFieldId,
                Values = new List<CustomFieldValue> {new CustomFieldValue { Info = issueToCloneCustomFieldValue }}
            });
           
            clonedIssue = (Issue)issueToClone.Clone();
            clonedIssue.CustomFields.Add(new IssueCustomField
            {
                Id = clonedIssueCustomFieldId,
                Values = new List<CustomFieldValue> {new CustomFieldValue { Info = clonedIssueCustomFieldValue }}
            });

            Assert.IsTrue(issueToClone.CustomFields.Count != clonedIssue.CustomFields.Count);
        }
        #endregion Tests

    }
}
