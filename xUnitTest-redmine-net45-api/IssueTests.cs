using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class IssueTests
	{
		//filters
		private const int NUMBER_OF_PAGINATED_ISSUES = 3;
		private const int OFFSET = 1;
		private const string PROJECT_ID = "redmine-net-testq";
		private const string SUBPROJECT_ID = "redmine-net-testr";
		private const string ALL_SUBPROJECTS = "!*";
		private const string TRACKER_ID = "3";
		private const string STATUS_ID = "*";
		private const string ASSIGNED_TO_ID = "me";
		private const string CUSTOM_FIELD_NAME = "cf_13";
		private const string CUSTOM_FIELD_VALUE = "Testx";
		private const string ISSUE_ID = "96";

		//issue data - used for create
		private const int NEW_ISSUE_PROJECT_ID = 9;
		private const int NEW_ISSUE_TRACKER_ID = 3;
		private const int NEW_ISSUE_STATUS_ID = 6;
		private const int NEW_ISSUE_PRIORITY_ID = 9;
		private const string NEW_ISSUE_SUBJECT = "Issue created using Rest API";
		private const string NEW_ISSUE_DESCRIPTION = "Issue description...";
		private const int NEW_ISSUE_CATEGORY_ID = 18;
		private const int NEW_ISSUE_FIXED_VERSION_ID = 9;
		private const int NEW_ISSUE_ASSIGNED_TO_ID = 8;
		private const int NEW_ISSUE_PARENT_ISSUE_ID = 96;
		private const int NEW_ISSUE_CUSTOM_FIELD_ID = 13;
		private const string NEW_ISSUE_CUSTOM_FIELD_VALUE = "Issue custom field completed";
		private const bool NEW_ISSUE_IS_PRIVATE = true;
		private const int NEW_ISSUE_ESTIMATED_HOURS = 12;
		private readonly DateTime newIssueStartDate = DateTime.Now;
		private readonly DateTime newIssueDueDate = DateTime.Now.AddDays(10);
		private const int NEW_ISSUE_FIRST_WATCHER_ID = 2;
		private const int NEW_ISSUE_SECOND_WATCHER_ID = 8;

		//issue data - used for update
		private const string UPDATED_ISSUE_ID = "98";
		private const string UPDATED_ISSUE_SUBJECT = "Issue updated subject";
		private const string UPDATED_ISSUE_DESCRIPTION = null;
		private readonly DateTime? updatedIssueStartDate = null;
		private readonly DateTime updatedIssueDueDate = DateTime.Now.AddMonths(1);
		private const int UPDATED_ISSUE_PROJECT_ID = 9;
		private const int UPDATED_ISSUE_TRACKER_ID = 3;
		private const int UPDATED_ISSUE_PRIORITY_ID = 8;
		private const int UPDATED_ISSUE_CATEGORY_ID = 18;
		private const int UPDATED_ISSUE_ASSIGNED_TO_ID = 2;
		private const int UPDATED_ISSUE_PARENT_ISSUE_ID = 91;
		private const int UPDATED_ISSUE_CUSTOM_FIELD_ID = 13;
		private const string UPDATED_ISSUE_CUSTOM_FIELD_VALUE = "Another custom field completed";
		private const int UPDATED_ISSUE_ESTIMATED_HOURS = 23;
		private const string UPDATED_ISSUE_NOTES = "A lot is changed";
		private const bool UPDATED_ISSUE_PRIVATE_NOTES = true;

		private const string DELETED_ISSUE_ID = "90";

		//watcher
		private const int WATCHER_ISSUE_ID = 96;
		private const int WATCHER_USER_ID = 8;

		//issue data - for clone
		private const string ISSUE_TO_CLONE_SUBJECT = "Issue to clone";
		private const int ISSUE_TO_CLONE_CUSTOM_FIELD_ID = 13;
		private const string ISSUE_TO_CLONE_CUSTOM_FIELD_VALUE = "Issue to clone custom field value";
		private const int CLONED_ISSUE_CUSTOM_FIELD_ID = 13;
		private const string CLONED_ISSUE_CUSTOM_FIELD_VALUE = "Cloned issue custom field value";

		RedmineFixture fixture;
		public IssueTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_Issues()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(null);

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Paginated_Issues()
		{
			var issues = fixture.redmineManager.GetPaginatedObjects<Issue>(new NameValueCollection { { RedmineKeys.OFFSET, OFFSET.ToString() }, { RedmineKeys.LIMIT, NUMBER_OF_PAGINATED_ISSUES.ToString() }, { "sort", "id:desc" } });

			Assert.NotNull(issues.Objects);
			Assert.All (issues.Objects, i => Assert.IsType<Issue> (i));
			Assert.True(issues.Objects.Count <= NUMBER_OF_PAGINATED_ISSUES, "number of issues <= " + NUMBER_OF_PAGINATED_ISSUES.ToString());
		}

		[Fact]
		public void Should_Get_Issues_By_Project_Id()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issues_By_subproject_Id()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.SUBPROJECT_ID, SUBPROJECT_ID } });

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issues_By_Project_Without_Subproject()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID }, { RedmineKeys.SUBPROJECT_ID, ALL_SUBPROJECTS } });

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issues_By_Tracker()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.TRACKER_ID, TRACKER_ID } });

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issues_By_Status()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.STATUS_ID, STATUS_ID } });
			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issues_By_Asignee()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { RedmineKeys.ASSIGNED_TO_ID, ASSIGNED_TO_ID } });

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issues_By_Custom_Field()
		{
			var issues = fixture.redmineManager.GetObjects<Issue>(new NameValueCollection { { CUSTOM_FIELD_NAME, CUSTOM_FIELD_VALUE } });

			Assert.NotNull(issues);
			Assert.All (issues, i => Assert.IsType<Issue> (i));
		}

		[Fact]
		public void Should_Get_Issue_By_Id()
		{
			var issue = fixture.redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.CHILDREN + "," + RedmineKeys.ATTACHMENTS + "," + RedmineKeys.RELATIONS + "," + RedmineKeys.CHANGESETS + "," + RedmineKeys.JOURNALS + "," + RedmineKeys.WATCHERS } });

			Assert.NotNull(issue);
			//TODO: add conditions for all associated data if nedeed
		}

		[Fact]
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

			Issue savedIssue = fixture.redmineManager.CreateObject(issue);

			Assert.NotNull(savedIssue);
			Assert.True(issue.Subject.Equals(savedIssue.Subject), "Issue subject is invalid.");
			Assert.NotEqual(issue, savedIssue);
		}

		[Fact]
		public void Should_Update_Issue()
		{
			var issue = fixture.redmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });
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

			fixture.redmineManager.UpdateObject(UPDATED_ISSUE_ID, issue);

			var updatedIssue = fixture.redmineManager.GetObject<Issue>(UPDATED_ISSUE_ID, new NameValueCollection { { "include", "children,attachments,relations,changesets,journals,watchers" } });

			Assert.NotNull(updatedIssue);
			Assert.True(issue.Subject.Equals(updatedIssue.Subject), "Issue subject is invalid.");

		}

		[Fact]
		public void Should_Delete_Issue()
		{
			try
			{
				fixture.redmineManager.DeleteObject<Issue>(DELETED_ISSUE_ID, null);
			}
			catch (RedmineException)
			{
				Assert.True(false, "Issue could not be deleted.");

			}

			try
			{
				fixture.redmineManager.GetObject<Issue>(DELETED_ISSUE_ID, null);
			}
			catch (RedmineException exc)
			{
				Assert.Contains(exc.Message, "Not Found");
				return;
			}
			Assert.True(false, "Test failed");
		}

		[Fact]
		public void Should_Add_Watcher_To_Issue()
		{
			fixture.redmineManager.AddWatcherToIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

			Issue issue = fixture.redmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

			Assert.NotNull(issue);
			Assert.NotNull(issue.Watchers);
			Assert.True(((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) != null, "Watcher was not added.");
		}

		[Fact]
		public void Should_Remove_Watcher_From_Issue()
		{
			fixture.redmineManager.RemoveWatcherFromIssue(WATCHER_ISSUE_ID, WATCHER_USER_ID);

			Issue issue = fixture.redmineManager.GetObject<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS } });

			Assert.NotNull(issue);
			Assert.True(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) == null, "Watcher was not removed.");
		}

		[Fact]
		public void Should_Compare_Issues()
		{
			var issue = fixture.redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.CHILDREN+","+RedmineKeys.ATTACHMENTS+","+RedmineKeys.RELATIONS+","+RedmineKeys.CHANGESETS+","+RedmineKeys.JOURNALS+","+RedmineKeys.WATCHERS } });
			var issueToCompare = fixture.redmineManager.GetObject<Issue>(ISSUE_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.CHILDREN + "," + RedmineKeys.ATTACHMENTS + "," + RedmineKeys.RELATIONS + "," + RedmineKeys.CHANGESETS + "," + RedmineKeys.JOURNALS + "," + RedmineKeys.WATCHERS } });

			Assert.NotNull(issue);
			Assert.True(issue.Equals(issueToCompare), "Issues are not equal.");
		}

		[Fact]
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

			Assert.True(issueToClone.CustomFields.Count != clonedIssue.CustomFields.Count);
		}
	}
}

