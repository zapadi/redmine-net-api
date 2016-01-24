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
        private RedmineManager redmineManager;

        #region Constants
        //filters
        private const int NUMBER_OF_PAGINATED_ISSUES = 3;
        private const int OFFSET = 1;
        private const string PROJECT_ID = "redmine-test";
        private const string SUBPROJECT_ID = "test";
        private const string ALL_SUBPROJECTS = "!*";
        private const string TRACKER_ID = "2";
        private const string STATUS_ID = "*";
        private const string ASSIGNED_TO_ID = "me";
        private const string CUSTOM_FIELD_NAME = "cf_1";
        private const string CUSTOM_FIELD_VALUE = "Testx";
        private const string ISSUE_ID = "1";

        //issue data - used for create
        private const int NEW_ISSUE_PROJECT_ID = 1;
        private const int NEW_ISSUE_TRACKER_ID = 2;
        private const int NEW_ISSUE_STATUS_ID = 4;
        private const int NEW_ISSUE_PRIORITY_ID = 2;
        private const string NEW_ISSUE_SUBJECT = "Issue created using Rest API";
        private const string NEW_ISSUE_DESCRIPTION = "Issue description...";
        private const int NEW_ISSUE_CATEGORY_ID = 1;
        private const int NEW_ISSUE_FIXED_VERSION_ID = 9;
        private const int NEW_ISSUE_ASSIGNED_TO_ID = 5;
        private const int NEW_ISSUE_PARENT_ISSUE_ID = 1;
        private const int NEW_ISSUE_CUSTOM_FIELD_ID = 1;
        private const string NEW_ISSUE_CUSTOM_FIELD_VALUE = "Issue custom field completed";
        private const bool NEW_ISSUE_IS_PRIVATE = true;
        private const int NEW_ISSUE_ESTIMATED_HOURS = 12;
        private readonly DateTime newIssueStartDate = DateTime.Now;
        private readonly DateTime newIssueDueDate = DateTime.Now.AddDays(10);
        private const int NEW_ISSUE_FIRST_WATCHER_ID = 1;
        private const int NEW_ISSUE_SECOND_WATCHER_ID = 5;

        //issue data - used for update
        private const string UPDATED_ISSUE_ID = "7";
        private const string UPDATED_ISSUE_SUBJECT = "Issue updated subject";
        private const string UPDATED_ISSUE_DESCRIPTION = null;
        private readonly DateTime? updatedIssueStartDate = null;
        private readonly DateTime updatedIssueDueDate = DateTime.Now.AddMonths(1);
        private const int UPDATED_ISSUE_PROJECT_ID = 1;
        private const int UPDATED_ISSUE_TRACKER_ID = 3;
        private const int UPDATED_ISSUE_PRIORITY_ID = 2;
        private const int UPDATED_ISSUE_CATEGORY_ID = 1;
        private const int UPDATED_ISSUE_ASSIGNED_TO_ID = 1;
        private const int UPDATED_ISSUE_PARENT_ISSUE_ID = 2;
        private const int UPDATED_ISSUE_CUSTOM_FIELD_ID = 1;
        private const string UPDATED_ISSUE_CUSTOM_FIELD_VALUE = "Another custom field completed";
        private const int UPDATED_ISSUE_ESTIMATED_HOURS = 23;
        private const string UPDATED_ISSUE_NOTES = "A lot is changed";
        private const bool UPDATED_ISSUE_PRIVATE_NOTES = true;

        private const string DELETED_ISSUE_ID = "7";

        //watcher
        private const int WATCHER_ISSUE_ID = 1;
        private const int WATCHER_USER_ID = 5;

        //issue data - for clone
        private const string ISSUE_TO_CLONE_SUBJECT = "Issue to clone";
        private const int ISSUE_TO_CLONE_CUSTOM_FIELD_ID = 1;
        private const string ISSUE_TO_CLONE_CUSTOM_FIELD_VALUE = "Issue to clone custom field value";
        private const int CLONED_ISSUE_CUSTOM_FIELD_ID = 1;
        private const string CLONED_ISSUE_CUSTOM_FIELD_VALUE = "Cloned issue custom field value";
        #endregion Constants

        #region Initializes
        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeJson();
            SetMimeTypeXml();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJson()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXml()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void Should_Get_All_Issues()
        {
            var issues = redmineManager.GetObjects<Issue>(null);

            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Paginated_Issues()
        {
            var issues = redmineManager.GetPaginatedObjects<Issue>(new NameValueCollection { { RedmineKeys.OFFSET, OFFSET.ToString() }, { RedmineKeys.LIMIT, NUMBER_OF_PAGINATED_ISSUES.ToString() }, { "sort", "id:desc" } });

            Assert.IsNotNull(issues.Objects, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues.Objects, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues.Objects, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues.Objects, "Issues are not unique.");
            Assert.IsTrue(issues.Objects.Count <= NUMBER_OF_PAGINATED_ISSUES, "number of issues <= " + NUMBER_OF_PAGINATED_ISSUES.ToString());
        }

        [TestMethod]
        public void Should_Get_Issues_By_Project_Id()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issues_By_subproject_Id()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.SUBPROJECT_ID, SUBPROJECT_ID } });
            
            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issues_By_Project_Without_Subproject()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID }, { RedmineKeys.SUBPROJECT_ID, ALL_SUBPROJECTS } });

            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issues_By_Tracker()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.TRACKER_ID, TRACKER_ID } });

            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issues_By_Status()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.STATUS_ID, STATUS_ID } });
            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issues_By_Asignee()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.ASSIGNED_TO_ID, ASSIGNED_TO_ID } });

            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issues_By_Custom_Field()
        {
            var issues = redmineManager.GetObjects<Issue>(new NameValueCollection { { CUSTOM_FIELD_NAME, CUSTOM_FIELD_VALUE } });

            Assert.IsNotNull(issues, "Issues is null.");
            CollectionAssert.AllItemsAreInstancesOfType(issues, typeof(Issue), "Not all items are of type issue.");
            CollectionAssert.AllItemsAreNotNull(issues, "Issues list contains null items.");
            CollectionAssert.AllItemsAreUnique(issues, "Issues are not unique.");
        }

        [TestMethod]
        public void Should_Get_Issue_By_Id()
        {
            var issue = redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.CHILDREN + "," + RedmineKeys.ATTACHMENTS + "," + RedmineKeys.RELATIONS + "," + RedmineKeys.CHANGESETS + "," + RedmineKeys.JOURNALS + "," + RedmineKeys.WATCHERS } });

            Assert.IsNotNull(issue, "Returned issue is null.");
            //TODO: add conditions for all associated data if nedeed
        }

        [TestMethod]
        public void Should_Add_Issue()
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

            Assert.IsNotNull(savedIssue, "Saved issue is null.");
            Assert.AreEqual(issue.Subject, savedIssue.Subject, "Issue subject is invalid.");
            Assert.AreNotEqual(issue, savedIssue, "Saved issue was not updated.");
        }

        [TestMethod]
        public void Should_Update_Issue()
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

            if (issue.CustomFields != null)
                issue.CustomFields.Add(new IssueCustomField { Id = UPDATED_ISSUE_CUSTOM_FIELD_ID, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = UPDATED_ISSUE_CUSTOM_FIELD_VALUE } } });
            issue.EstimatedHours = UPDATED_ISSUE_ESTIMATED_HOURS;
            issue.Notes = UPDATED_ISSUE_NOTES;
            issue.PrivateNotes = UPDATED_ISSUE_PRIVATE_NOTES;

            redmineManager.UpdateObject(UPDATED_ISSUE_ID, issue);

            var updatedIssue = redmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

            Assert.IsNotNull(updatedIssue, "Updated issue is null.");
            Assert.AreEqual(issue.Subject, updatedIssue.Subject, "Issue subject is invalid.");
          
        }

        [TestMethod]
        public void Should_Delete_Issue()
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
        public void Should_Add_Watcher_To_Issue()
        {
            redmineManager.AddWatcherToIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

            Issue issue = redmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

            Assert.IsNotNull(issue, "Issue is null.");
            Assert.IsNotNull(issue.Watchers, "Watchers list is null.");
            Assert.IsTrue(((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) != null, "Watcher was not added.");
        }

        [TestMethod]
        public void Should_Remove_Watcher_From_Issue()
        {
            redmineManager.RemoveWatcherFromIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

            Issue issue = redmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

            Assert.IsNotNull(issue, "Issue is null.");
            Assert.IsTrue(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) == null, "Watcher was not removed.");
        }

        [TestMethod]
        public void Should_Compare_Issues()
        {
            var issue = redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.CHILDREN+","+RedmineKeys.ATTACHMENTS+","+RedmineKeys.RELATIONS+","+RedmineKeys.CHANGESETS+","+RedmineKeys.JOURNALS+","+RedmineKeys.WATCHERS } });
            var issueToCompare = redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.CHILDREN + "," + RedmineKeys.ATTACHMENTS + "," + RedmineKeys.RELATIONS + "," + RedmineKeys.CHANGESETS + "," + RedmineKeys.JOURNALS + "," + RedmineKeys.WATCHERS } });

            Assert.IsNotNull(issue, "Issue is null");
            Assert.IsTrue(issue.Equals(issueToCompare), "Issues are not equal.");
        }

        [TestMethod]
        public void Should_Clone_Issue()
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
