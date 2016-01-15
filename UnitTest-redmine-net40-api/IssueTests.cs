using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class IssueTests
    {
        #region Constants
        //filters
        private const int NUMBER_OF_PAGINATED_ISSUES = 3;
        private const string PROJECT_ID = "9";
        private const string SUBPROJECT_ID = "10";
        private const string ALL_SUBPROJECTS = "!*";
        private const string TRACKER_ID = "4";
        private const string STATUS_ID = "*";
        private const string ASSIGNED_TO_ID = "me";
        private const string CUSTOM_FIELD_NAME = "cf_13";
        private const string CUSTOM_FIELD_VALUE = "xxx";
        private const string ISSUE_ID = "19";

        //issue data - used for create
        private const int NEW_ISSUE_PROJECT_ID = 10;
        private const int NEW_ISSUE_TRACKER_ID = 4;
        private const int NEW_ISSUE_STATUS_ID = 5;
        private const int NEW_ISSUE_PRIORITY_ID = 8;
        private const string NEW_ISSUE_SUBJECT = "Issue created using Rest API";
        private const string NEW_ISSUE_DESCRIPTION = "Issue description...";
        private const int NEW_ISSUE_CATEGORY_ID = 11;
        private const int NEW_ISSUE_FIXED_VERSION_ID = 9;
        private const int NEW_ISSUE_ASSIGNED_TO_ID = 8;
        private const int NEW_ISSUE_PARENT_ISSUE_ID = 19;
        private const int NEW_ISSUE_CUSTOM_FIELD_ID = 13;
        private const string NEW_ISSUE_CUSTOM_FIELD_VALUE = "Issue custom field completed";
        private const bool NEW_ISSUE_IS_PRIVATE = true;
        private const int NEW_ISSUE_ESTIMATED_HOURS = 12;
        private readonly DateTime newIssueStartDate = DateTime.Now;
        private readonly DateTime newIssueDueDate = DateTime.Now.AddDays(10);
        private const int NEW_ISSUE_FIRST_WATCHER_ID = 2;
        private const int NEW_ISSUE_SECOND_WATCHER_ID = 8;

        //issue data - used for update
        private const string UPDATED_ISSUE_ID = "76";
        private const string UPDATED_ISSUE_SUBJECT = "Issue updated subject";
        private const string UPDATED_ISSUE_DESCRIPTION = null;
        private readonly DateTime? updatedIssueStartDate = null;
        private readonly DateTime updatedIssueDueDate = DateTime.Now.AddMonths(1);
        private const int UPDATED_ISSUE_PROJECT_ID = 10;
        private const int UPDATED_ISSUE_TRACKER_ID = 3;
        private const int UPDATED_ISSUE_PRIORITY_ID = 9;
        private const int UPDATED_ISSUE_CATEGORY_ID = 10;
        private const int UPDATED_ISSUE_ASSIGNED_TO_ID = 2;
        private const int UPDATED_ISSUE_PARENT_ISSUE_ID = 20;
        private const int UPDATED_ISSUE_CUSTOM_FIELD_ID = 13;
        private const string UPDATED_ISSUE_CUSTOM_FIELD_VALUE = "Another custom field completed";
        private const int UPDATED_ISSUE_ESTIMATED_HOURS = 23;
        private const string UPDATED_ISSUE_NOTES = "A lot is changed";
        private const bool UPDATED_ISSUE_PRIVATE_NOTES = true;

        private const string DELETED_ISSUE_ID = "76";

        //watcher
        private const int WATCHER_ISSUE_ID = 19;
        private const int WATCHER_USER_ID = 2;

        //issue data - for clone
        private const string ISSUE_TO_CLONE_SUBJECT = "Issue to clone";
        private const int ISSUE_TO_CLONE_CUSTOM_FIELD_ID = 13;
        private const string ISSUE_TO_CLONE_CUSTOM_FIELD_VALUE = "Issue to clone custom field value";
        private const int CLONED_ISSUE_CUSTOM_FIELD_ID = 14;
        private const string CLONED_ISSUE_CUSTOM_FIELD_VALUE = "Cloned issue custom field value";
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
            uri = Helper.Uri;
            apiKey = Helper.ApiKey;

            SetMimeTypeJson();
            SetMimeTypeXml();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJson()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXml()
        {
            redmineManager = new RedmineManager(uri, apiKey);
        }
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void RedmineIssues_ShouldReturnAllIssues()
        {
            var issues = redmineManager.GetObjects<Issue>(null);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnPaginatedIssues()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "offset", "1" }, { "limit", NUMBER_OF_PAGINATED_ISSUES.ToString() }, { "sort", "id:desc" } });

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count <= NUMBER_OF_PAGINATED_ISSUES, "number of issues <= " + NUMBER_OF_PAGINATED_ISSUES.ToString());
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByProjectId()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "project_id", PROJECT_ID } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesBySubprojectId()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "subproject_id", SUBPROJECT_ID } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssues_ByProjectWithoutSubprojects()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "project_id", PROJECT_ID }, { "subproject_id", ALL_SUBPROJECTS } });

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByTracker()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "tracker_id", TRACKER_ID } });

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByStatus()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "status_id", STATUS_ID } });
            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByAsignee()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { "assigned_to_id", ASSIGNED_TO_ID } });

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnFilteredIssuesByCustomField()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { CUSTOM_FIELD_NAME, CUSTOM_FIELD_VALUE } });

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void RedmineIssues_ShouldReturnIssueById()
        {
            var issue = redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.IsNotNull(issue);
            //TODO: add conditions for all associated data
        }

        [TestMethod]
        public void RedmineIssues_ShouldCreateIssue()
        {
            Issue issue = new Issue
            {
                Project = new Project {Id = NEW_ISSUE_PROJECT_ID},
                Tracker = new IdentifiableName {Id = NEW_ISSUE_TRACKER_ID},
                Status = new IdentifiableName {Id = NEW_ISSUE_STATUS_ID},
                Priority = new IdentifiableName {Id = NEW_ISSUE_PRIORITY_ID},
                Subject = NEW_ISSUE_SUBJECT,
                Description = NEW_ISSUE_DESCRIPTION,
                Category = new IdentifiableName {Id = NEW_ISSUE_CATEGORY_ID},
                FixedVersion = new IdentifiableName {Id = NEW_ISSUE_FIXED_VERSION_ID},
                AssignedTo = new IdentifiableName {Id = NEW_ISSUE_ASSIGNED_TO_ID},
                ParentIssue = new IdentifiableName {Id = NEW_ISSUE_PARENT_ISSUE_ID},
                CustomFields = new List<IssueCustomField>
                {
                    new IssueCustomField
                    {
                        Id = NEW_ISSUE_CUSTOM_FIELD_ID,
                        Values = new List<CustomFieldValue> {new CustomFieldValue {Info = NEW_ISSUE_CUSTOM_FIELD_VALUE}}
                    }
                },
                IsPrivate = NEW_ISSUE_IS_PRIVATE,
                EstimatedHours = NEW_ISSUE_ESTIMATED_HOURS,
                StartDate = newIssueStartDate,
                DueDate = newIssueDueDate,
                Watchers = new List<Watcher>
                {
                    new Watcher {Id = NEW_ISSUE_FIRST_WATCHER_ID},
                    new Watcher {Id = NEW_ISSUE_SECOND_WATCHER_ID}
                }
            };

            Issue savedIssue = redmineManager.CreateObject(issue);

            Assert.AreEqual(issue.Subject, savedIssue.Subject);
            Assert.AreNotEqual(issue, savedIssue);
        }

        [TestMethod]
        public void RedmineIssues_ShouldUpdateIssue()
        {
            var issue = redmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });
            issue.Subject = UPDATED_ISSUE_SUBJECT;
            issue.Description = UPDATED_ISSUE_DESCRIPTION;
            issue.StartDate = updatedIssueStartDate;
            issue.DueDate = updatedIssueDueDate;
            issue.Project.Id = UPDATED_ISSUE_PROJECT_ID;
            issue.Tracker.Id = UPDATED_ISSUE_TRACKER_ID;
            issue.Priority.Id = UPDATED_ISSUE_PRIORITY_ID;
            issue.Category.Id = UPDATED_ISSUE_CATEGORY_ID;
            issue.AssignedTo.Id = UPDATED_ISSUE_ASSIGNED_TO_ID;
            issue.ParentIssue.Id = UPDATED_ISSUE_PARENT_ISSUE_ID;

            issue.CustomFields.Add(new IssueCustomField { Id = UPDATED_ISSUE_CUSTOM_FIELD_ID, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = UPDATED_ISSUE_CUSTOM_FIELD_VALUE } } });
            issue.EstimatedHours = UPDATED_ISSUE_ESTIMATED_HOURS;
            issue.Notes = UPDATED_ISSUE_NOTES;
            issue.PrivateNotes = UPDATED_ISSUE_PRIVATE_NOTES;

            redmineManager.UpdateObject(UPDATED_ISSUE_ID, issue);

            var updatedIssue = redmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.AreEqual(issue.Subject, updatedIssue.Subject);
        }

        [TestMethod]
        public void RedmineIssues_ShouldDeleteIssue()
        {
            try
            {
                redmineManager.DeleteObject<Issue>(DELETED_ISSUE_ID, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Issue could not be deleted.");
               
            }

            try
            {
                 redmineManager.GetObject<Issue>(DELETED_ISSUE_ID, null);
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
            redmineManager.AddWatcherToIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

            Issue issue = redmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { "include", "watchers" } });

            Assert.IsTrue(((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) != null);
        }

        [TestMethod]
        public void RedmineIssues_ShouldRemoveWatcher()
        {
            redmineManager.RemoveWatcherFromIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

            Issue issue = redmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { "include", "watchers" } });

            Assert.IsTrue(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) == null);
        }

        [TestMethod]
        public void RedmineIssues_ShouldCompare()
        {
            var issue = redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });
            var issueToCompare = redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.IsTrue(issue.Equals(issueToCompare));
        }

        [TestMethod]
        public void RedmineIssues_ShouldClone()
        {
            var issueToClone = new Issue
            {
                Subject = ISSUE_TO_CLONE_SUBJECT,
                CustomFields = new List<IssueCustomField>
                {
                    new IssueCustomField
                    {
                        Id = ISSUE_TO_CLONE_CUSTOM_FIELD_ID,
                        Values =
                            new List<CustomFieldValue> {new CustomFieldValue {Info = ISSUE_TO_CLONE_CUSTOM_FIELD_VALUE}}
                    }
                }
            };


            var clonedIssue = (Issue)issueToClone.Clone();
            clonedIssue.CustomFields.Add(new IssueCustomField
            {
                Id = CLONED_ISSUE_CUSTOM_FIELD_ID,
                Values = new List<CustomFieldValue> { new CustomFieldValue { Info = CLONED_ISSUE_CUSTOM_FIELD_VALUE } }
            });

            Assert.IsTrue(issueToClone.CustomFields.Count != clonedIssue.CustomFields.Count);
        }
        #endregion Tests

    }
}
