using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
    [Trait("Redmine-Net-Api", "Issues")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class IssueTests
    {
        public IssueTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }
        private readonly RedmineFixture fixture;

        //filters
        private const string PROJECT_ID = "redmine-net-testq";

        //watcher
        private const int WATCHER_ISSUE_ID = 96;
        private const int WATCHER_USER_ID = 8;


        [Fact, Order(1)]
        public void Should_Get_All_Issues()
        {
            var issues = fixture.RedmineManager.GetObjects<Issue>();

            Assert.NotNull(issues);
        }

        [Fact, Order(2)]
        public void Should_Get_Paginated_Issues()
        {
            const int NUMBER_OF_PAGINATED_ISSUES = 3;
            const int OFFSET = 1;

            var issues = fixture.RedmineManager.GetPaginatedObjects<Issue>(new NameValueCollection
            {
                { RedmineKeys.OFFSET, OFFSET.ToString() }, { RedmineKeys.LIMIT, NUMBER_OF_PAGINATED_ISSUES.ToString() }, { "sort", "id:desc" }
            });

            Assert.NotNull(issues.Items);
            //Assert.True(issues.Items.Count <= NUMBER_OF_PAGINATED_ISSUES, "number of issues ( "+ issues.Items.Count +" ) != " + NUMBER_OF_PAGINATED_ISSUES.ToString());
        }

        [Fact, Order(3)]
        public void Should_Get_Issues_By_Project_Id()
        {
            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

            Assert.NotNull(issues);
        }

        [Fact, Order(4)]
        public void Should_Get_Issues_By_SubProject_Id()
        {
            const string SUB_PROJECT_ID_VALUE = "redmine-net-testr";

            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.SUB_PROJECT_ID, SUB_PROJECT_ID_VALUE } });

            Assert.NotNull(issues);
        }

        [Fact, Order(5)]
        public void Should_Get_Issues_By_Project_Without_SubProject()
        {
            const string ALL_SUB_PROJECTS = "!*";

            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection
            {
                { RedmineKeys.PROJECT_ID, PROJECT_ID }, { RedmineKeys.SUB_PROJECT_ID, ALL_SUB_PROJECTS }
            });

            Assert.NotNull(issues);
        }

        [Fact, Order(6)]
        public void Should_Get_Issues_By_Tracker()
        {
            const string TRACKER_ID = "3";
            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.TRACKER_ID, TRACKER_ID } });

            Assert.NotNull(issues);
        }

        [Fact, Order(7)]
        public void Should_Get_Issues_By_Status()
        {
            const string STATUS_ID = "*";
            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.STATUS_ID, STATUS_ID } });
            Assert.NotNull(issues);
        }

        [Fact, Order(8)]
        public void Should_Get_Issues_By_Assignee()
        {
            const string ASSIGNED_TO_ID = "me";
            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.ASSIGNED_TO_ID, ASSIGNED_TO_ID } });

            Assert.NotNull(issues);
        }

        [Fact, Order(9)]
        public void Should_Get_Issues_By_Custom_Field()
        {
            const string CUSTOM_FIELD_NAME = "cf_13";
            const string CUSTOM_FIELD_VALUE = "Testx";

            var issues = fixture.RedmineManager.GetObjects<Issue>(new NameValueCollection { { CUSTOM_FIELD_NAME, CUSTOM_FIELD_VALUE } });

            Assert.NotNull(issues);
        }

        [Fact, Order(10)]
        public void Should_Get_Issue_By_Id()
        {
            const string ISSUE_ID = "96";

            var issue = fixture.RedmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection
            {
                { RedmineKeys.INCLUDE, $"{RedmineKeys.CHILDREN},{RedmineKeys.ATTACHMENTS},{RedmineKeys.RELATIONS},{RedmineKeys.CHANGE_SETS},{RedmineKeys.JOURNALS},{RedmineKeys.WATCHERS}" }
            });

            Assert.NotNull(issue);
        }

        [Fact, Order(11)]
        public void Should_Add_Issue()
        {
            const bool NEW_ISSUE_IS_PRIVATE = true;

            const int NEW_ISSUE_PROJECT_ID = 1;
            const int NEW_ISSUE_TRACKER_ID = 1;
            const int NEW_ISSUE_STATUS_ID = 1;
            const int NEW_ISSUE_PRIORITY_ID = 9;
            const int NEW_ISSUE_CATEGORY_ID = 18;
            const int NEW_ISSUE_FIXED_VERSION_ID = 9;
            const int NEW_ISSUE_ASSIGNED_TO_ID = 8;
            const int NEW_ISSUE_PARENT_ISSUE_ID = 96;
            const int NEW_ISSUE_CUSTOM_FIELD_ID = 13;
            const int NEW_ISSUE_ESTIMATED_HOURS = 12;
            const int NEW_ISSUE_FIRST_WATCHER_ID = 2;
            const int NEW_ISSUE_SECOND_WATCHER_ID = 8;

            const string NEW_ISSUE_CUSTOM_FIELD_VALUE = "Issue custom field completed";
            const string NEW_ISSUE_SUBJECT = "Issue created using Rest API";
            const string NEW_ISSUE_DESCRIPTION = "Issue description...";

            var newIssueStartDate = DateTime.Now;
            var newIssueDueDate = DateTime.Now.AddDays(10);

            var  icf = IdentifiableName.Create<IssueCustomField>(NEW_ISSUE_CUSTOM_FIELD_ID);
            if (icf != null)
            {
                icf.Values = new List<CustomFieldValue> {new CustomFieldValue {Info = NEW_ISSUE_CUSTOM_FIELD_VALUE}};
            }

            var issue = new Issue
                {
                    Project = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_PROJECT_ID),
                    Tracker = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_TRACKER_ID),
                    Status = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_STATUS_ID),
                    Priority = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_PRIORITY_ID),
                    Subject = NEW_ISSUE_SUBJECT,
                    Description = NEW_ISSUE_DESCRIPTION,
                    Category = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_CATEGORY_ID),
                    FixedVersion = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_FIXED_VERSION_ID),
                    AssignedTo = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_ASSIGNED_TO_ID),
                    ParentIssue = IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_PARENT_ISSUE_ID),

                    CustomFields = new List<IssueCustomField> {icf},
                    IsPrivate = NEW_ISSUE_IS_PRIVATE,
                    EstimatedHours = NEW_ISSUE_ESTIMATED_HOURS,
                    StartDate = newIssueStartDate,
                    DueDate = newIssueDueDate,
                    Watchers = new List<Watcher>
                    {
                         IdentifiableName.Create<Watcher>(NEW_ISSUE_FIRST_WATCHER_ID),
                         IdentifiableName.Create<Watcher>(NEW_ISSUE_SECOND_WATCHER_ID)
                    }
                };

                var savedIssue = fixture.RedmineManager.CreateObject(issue);

                Assert.NotNull(savedIssue);
                Assert.True(issue.Subject.Equals(savedIssue.Subject), "Issue subject is invalid.");
                Assert.NotEqual(issue, savedIssue);
            
        }

        [Fact, Order(12)]
        public void Should_Update_Issue()
        {
            const string UPDATED_ISSUE_ID = "98";
            const string UPDATED_ISSUE_SUBJECT = "Issue updated subject";
            const string UPDATED_ISSUE_DESCRIPTION = null;
            const int UPDATED_ISSUE_PROJECT_ID = 9;
            const int UPDATED_ISSUE_TRACKER_ID = 3;
            const int UPDATED_ISSUE_PRIORITY_ID = 8;
            const int UPDATED_ISSUE_CATEGORY_ID = 18;
            const int UPDATED_ISSUE_ASSIGNED_TO_ID = 2;
            const int UPDATED_ISSUE_PARENT_ISSUE_ID = 91;
            const int UPDATED_ISSUE_CUSTOM_FIELD_ID = 13;
            const string UPDATED_ISSUE_CUSTOM_FIELD_VALUE = "Another custom field completed";
            const int UPDATED_ISSUE_ESTIMATED_HOURS = 23;
            const string UPDATED_ISSUE_NOTES = "A lot is changed";
            const bool UPDATED_ISSUE_PRIVATE_NOTES = true;

            DateTime? updatedIssueStartDate = default(DateTime?);

            var updatedIssueDueDate = DateTime.Now.AddMonths(1);

            var issue = fixture.RedmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection
            {
                { RedmineKeys.INCLUDE, $"{RedmineKeys.CHILDREN},{RedmineKeys.ATTACHMENTS},{RedmineKeys.RELATIONS},{RedmineKeys.CHANGE_SETS},{RedmineKeys.JOURNALS},{RedmineKeys.WATCHERS}" }
            });

            issue.Subject = UPDATED_ISSUE_SUBJECT;
            issue.Description = UPDATED_ISSUE_DESCRIPTION;
            issue.StartDate = updatedIssueStartDate;
            issue.DueDate = updatedIssueDueDate;
            issue.Project = IdentifiableName.Create<IdentifiableName>(UPDATED_ISSUE_PROJECT_ID);
            issue.Tracker = IdentifiableName.Create<IdentifiableName>(UPDATED_ISSUE_TRACKER_ID);
            issue.Priority = IdentifiableName.Create<IdentifiableName>(UPDATED_ISSUE_PRIORITY_ID);
            issue.Category = IdentifiableName.Create<IdentifiableName>(UPDATED_ISSUE_CATEGORY_ID);
            issue.AssignedTo = IdentifiableName.Create<IdentifiableName>(UPDATED_ISSUE_ASSIGNED_TO_ID);
            issue.ParentIssue = IdentifiableName.Create<IdentifiableName>(UPDATED_ISSUE_PARENT_ISSUE_ID);

            var icf = (IssueCustomField)IdentifiableName.Create<IssueCustomField>(UPDATED_ISSUE_CUSTOM_FIELD_ID);
            icf.Values = new List<CustomFieldValue> { new CustomFieldValue { Info = UPDATED_ISSUE_CUSTOM_FIELD_VALUE } };

            issue.CustomFields?.Add(icf);
            issue.EstimatedHours = UPDATED_ISSUE_ESTIMATED_HOURS;
            issue.Notes = UPDATED_ISSUE_NOTES;
            issue.PrivateNotes = UPDATED_ISSUE_PRIVATE_NOTES;

            fixture.RedmineManager.UpdateObject(UPDATED_ISSUE_ID, issue);

            var updatedIssue = fixture.RedmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection
            {
                { RedmineKeys.INCLUDE, $"{RedmineKeys.CHILDREN},{RedmineKeys.ATTACHMENTS},{RedmineKeys.RELATIONS},{RedmineKeys.CHANGE_SETS},{RedmineKeys.JOURNALS},{RedmineKeys.WATCHERS}" }
            });

            Assert.NotNull(updatedIssue);
            Assert.True(issue.Subject.Equals(updatedIssue.Subject), "Issue subject is invalid.");

        }

        [Fact, Order(99)]
        public void Should_Delete_Issue()
        {
            const string DELETED_ISSUE_ID = "90";

            var exception = (RedmineException)Record.Exception(() => fixture.RedmineManager.DeleteObject<Issue>(DELETED_ISSUE_ID));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Issue>(DELETED_ISSUE_ID, null));
        }

        [Fact, Order(13)]
        public void Should_Add_Watcher_To_Issue()
        {
            fixture.RedmineManager.AddWatcherToIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

            var issue = fixture.RedmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

            Assert.NotNull(issue);
            Assert.NotNull(issue.Watchers);
            Assert.True(((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) != null, "Watcher was not added.");
        }

        [Fact, Order(14)]
        public void Should_Remove_Watcher_From_Issue()
        {
            fixture.RedmineManager.RemoveWatcherFromIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

            var issue = fixture.RedmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

            Assert.NotNull(issue);
            Assert.True(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) == null, "Watcher was not removed.");
        }

        [Fact, Order(15)]
        public void Should_Clone_Issue()
        {
            const string ISSUE_TO_CLONE_SUBJECT = "Issue to clone";
            const int ISSUE_TO_CLONE_CUSTOM_FIELD_ID = 13;
            const string ISSUE_TO_CLONE_CUSTOM_FIELD_VALUE = "Issue to clone custom field value";
            const int CLONED_ISSUE_CUSTOM_FIELD_ID = 13;
            const string CLONED_ISSUE_CUSTOM_FIELD_VALUE = "Cloned issue custom field value";

            var icfc = (IssueCustomField)IdentifiableName.Create<IssueCustomField>(ISSUE_TO_CLONE_CUSTOM_FIELD_ID);
            icfc.Values = new List<CustomFieldValue> { new CustomFieldValue { Info = ISSUE_TO_CLONE_CUSTOM_FIELD_VALUE } };

            var issueToClone = new Issue
            {
                Subject = ISSUE_TO_CLONE_SUBJECT,
                CustomFields = new List<IssueCustomField>() { icfc }
            };

            var clonedIssue = (Issue)issueToClone.Clone();

            var icf = (IssueCustomField)IdentifiableName.Create<IssueCustomField>(CLONED_ISSUE_CUSTOM_FIELD_ID);
            icf.Values = new List<CustomFieldValue> { new CustomFieldValue { Info = CLONED_ISSUE_CUSTOM_FIELD_VALUE } };

            clonedIssue.CustomFields.Add(icf);

            Assert.True(issueToClone.CustomFields.Count != clonedIssue.CustomFields.Count);
        }

        [Fact]
        public void Should_Get_Issue_With_Hours()
        {
            const string ISSUE_ID = "1";

            var issue = fixture.RedmineManager.GetObject<Issue>(ISSUE_ID, null);

            Assert.Equal(8.0f, issue.EstimatedHours);
            Assert.Equal(8.0f, issue.TotalEstimatedHours);
            Assert.Equal(5.0f, issue.TotalSpentHours);
            Assert.Equal(5.0f, issue.SpentHours);
        }
    }
}