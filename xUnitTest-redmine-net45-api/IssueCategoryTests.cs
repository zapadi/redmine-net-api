using System;
using System.Collections.Specialized;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Exceptions;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class IssueCategoryTests
	{
		private const string PROJECT_ID = "redmine-net-testq";
		private const int NUMBER_OF_ISSUE_CATEGORIES = 2;

		//issueCategory data for insert
		private const string NEW_ISSUE_CATEGORY_NAME = "Test category";
		private const int NEW_ISSUE_CATEGORY_ASIGNEE_ID = 5;

		private const string ISSUE_CATEGORY_ID_TO_GET = "17";
		private const string ISSUE_CATEGORY_NAME_TO_GET = "Test category";
		private const string ISSUE_CATEGORY_PROJECT_NAME_TO_GET = "redmine-net-testq";
		private const string ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET = "Alina";

		private const string ISSUE_CATEGORY_ID_TO_UPDATE = "17";
		private const string ISSUE_CATEGORY_NAME_TO_UPDATE = "Category updated";
		private const int ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE = 2;

		private const string ISSUE_CATEGORY_ID_TO_DELETE = "16";

		private const string ISSUE_CATEGORY_ID_TO_COMPARE = "17";

		RedmineFixture fixture;
		public IssueCategoryTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_IssueCategories_By_ProjectId()
		{
			var issueCategories = fixture.redmineManager.GetObjects<IssueCategory>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

			Assert.NotNull(issueCategories);
			Assert.All (issueCategories, ic => Assert.IsType<IssueCategory> (ic));
			Assert.True(issueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Number of issue categories != " + NUMBER_OF_ISSUE_CATEGORIES);   
		}

		[Fact]
		public void Should_Create_IssueCategory()
		{
			IssueCategory issueCategory = new IssueCategory();
			issueCategory.Name = NEW_ISSUE_CATEGORY_NAME;
			issueCategory.AsignTo = new IdentifiableName { Id = NEW_ISSUE_CATEGORY_ASIGNEE_ID };

			IssueCategory savedIssueCategory = fixture.redmineManager.CreateObject<IssueCategory>(issueCategory, PROJECT_ID);

			Assert.NotNull(savedIssueCategory);
			Assert.True(savedIssueCategory.Name.Equals(NEW_ISSUE_CATEGORY_NAME), "Saved issue category name is invalid.");
		}

		[Fact]
		public void Should_Get_IssueCategory_By_Id()
		{
			IssueCategory issueCategory = fixture.redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_GET, null);

			Assert.NotNull(issueCategory);
			Assert.True(issueCategory.Name.Equals(ISSUE_CATEGORY_NAME_TO_GET), "Issue category name is invalid.");
			Assert.NotNull(issueCategory.AsignTo);
			Assert.True(issueCategory.AsignTo.Name.Contains(ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET), "Asignee name is invalid.");
			Assert.NotNull(issueCategory.Project);
			Assert.True(issueCategory.Project.Name.Equals(ISSUE_CATEGORY_PROJECT_NAME_TO_GET), "Project name is invalid.");
		}

		[Fact]
		public void Should_Update_IssueCategory()
		{
			IssueCategory issueCategory = fixture.redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, null);
			issueCategory.Name = ISSUE_CATEGORY_NAME_TO_UPDATE;
			issueCategory.AsignTo = new IdentifiableName { Id = ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE };

			fixture.redmineManager.UpdateObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, issueCategory);

			IssueCategory updatedIssueCategory = fixture.redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, null);

			Assert.NotNull(updatedIssueCategory);
			Assert.True(updatedIssueCategory.Name.Equals(ISSUE_CATEGORY_NAME_TO_UPDATE), "Issue category name was not updated.");
			Assert.NotNull(updatedIssueCategory.AsignTo);
			Assert.True(updatedIssueCategory.AsignTo.Id == ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE, "Issue category asignee was not updated.");
		}

		[Fact]
		public void Should_Delete_IssueCategory()
		{
			RedmineException exception = (RedmineException)Record.Exception(() =>fixture.redmineManager.DeleteObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null));
			Assert.Null (exception);
			Assert.Throws<NotFoundException>(() => fixture.redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null));
		}

		[Fact]
		public void Should_Compare_IssueCategories()
		{
			IssueCategory issueCategory = fixture.redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_COMPARE, null);
			IssueCategory issueCategoryToCompare = fixture.redmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_COMPARE, null);

			Assert.True(issueCategory.Equals(issueCategoryToCompare));
		}
	}
}

