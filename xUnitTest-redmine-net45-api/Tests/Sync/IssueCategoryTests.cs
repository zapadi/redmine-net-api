using System;
using System.Collections.Specialized;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
	[Trait("Redmine-Net-Api", "IssueCategories")]
	[Collection("RedmineCollection")]
    public class IssueCategoryTests
    {
        public IssueCategoryTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

	    private readonly RedmineFixture fixture;

	    const string PROJECT_ID = "redmine-net-testq";
	    const string NEW_ISSUE_CATEGORY_NAME = "Test category";
	    const int NEW_ISSUE_CATEGORY_ASIGNEE_ID = 1;

	    private static string CREATED_ISSUE_CATEGORY_ID;

        [Fact, Order(1)]
        public void Should_Create_IssueCategory()
        {
	        var issueCategory = new IssueCategory
            {
                Name = NEW_ISSUE_CATEGORY_NAME,
                AsignTo = new IdentifiableName {Id = NEW_ISSUE_CATEGORY_ASIGNEE_ID}
            };

            var savedIssueCategory = fixture.RedmineManager.CreateObject(issueCategory, PROJECT_ID);

	        CREATED_ISSUE_CATEGORY_ID = savedIssueCategory.Id.ToString();

            Assert.NotNull(savedIssueCategory);
            Assert.True(savedIssueCategory.Name.Equals(NEW_ISSUE_CATEGORY_NAME), "Saved issue category name is invalid.");
        }

        [Fact, Order(99)]
        public void Should_Delete_IssueCategory()
        {
	        var exception =
                (RedmineException)
                    Record.Exception(
                        () => fixture.RedmineManager.DeleteObject<IssueCategory>(CREATED_ISSUE_CATEGORY_ID));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.GetObject<IssueCategory>(CREATED_ISSUE_CATEGORY_ID, null));
        }

        [Fact, Order(2)]
        public void Should_Get_All_IssueCategories_By_ProjectId()
        {
	        const int NUMBER_OF_ISSUE_CATEGORIES = 3;
	        var issueCategories =
                fixture.RedmineManager.GetObjects<IssueCategory>(new NameValueCollection
                {
                    {RedmineKeys.PROJECT_ID, PROJECT_ID}
                });

            Assert.NotNull(issueCategories);
            Assert.All(issueCategories, ic => Assert.IsType<IssueCategory>(ic));
            Assert.True(issueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES,
                "Number of issue categories ( "+issueCategories.Count+" ) != " + NUMBER_OF_ISSUE_CATEGORIES);
        }

        [Fact, Order(3)]
        public void Should_Get_IssueCategory_By_Id()
        {
	        const string ISSUE_CATEGORY_PROJECT_NAME_TO_GET = "Redmine tests";
	        const string ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET = "Redmine";

	        var issueCategory = fixture.RedmineManager.GetObject<IssueCategory>(CREATED_ISSUE_CATEGORY_ID, null);

            Assert.NotNull(issueCategory);
            Assert.True(issueCategory.Name.Equals(NEW_ISSUE_CATEGORY_NAME), "Issue category name is invalid.");
            Assert.NotNull(issueCategory.AsignTo);
            Assert.True(issueCategory.AsignTo.Name.Contains(ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET),
                "Asignee name is invalid.");
            Assert.NotNull(issueCategory.Project);
            Assert.True(issueCategory.Project.Name.Equals(ISSUE_CATEGORY_PROJECT_NAME_TO_GET),
                "Project name is invalid.");
        }

        [Fact, Order(4)]
        public void Should_Update_IssueCategory()
        {
	        const string ISSUE_CATEGORY_NAME_TO_UPDATE = "Category updated";
	        const int ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE = 2;

	        var issueCategory = fixture.RedmineManager.GetObject<IssueCategory>(CREATED_ISSUE_CATEGORY_ID, null);
            issueCategory.Name = ISSUE_CATEGORY_NAME_TO_UPDATE;
            issueCategory.AsignTo = new IdentifiableName {Id = ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE};

            fixture.RedmineManager.UpdateObject(CREATED_ISSUE_CATEGORY_ID, issueCategory);

            var updatedIssueCategory = fixture.RedmineManager.GetObject<IssueCategory>(CREATED_ISSUE_CATEGORY_ID, null);

            Assert.NotNull(updatedIssueCategory);
            Assert.True(updatedIssueCategory.Name.Equals(ISSUE_CATEGORY_NAME_TO_UPDATE),
                "Issue category name was not updated.");
            Assert.NotNull(updatedIssueCategory.AsignTo);
            Assert.True(updatedIssueCategory.AsignTo.Id == ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE,
                "Issue category asignee was not updated.");
        }

	    [Fact, Order(5)]
	    public void Should_Reassign_Issue_After_Issue_Category_Is_Deleted()
	    {
		    const string ISSUE_CATEGORY_ID_TO_DELETE = "8";
		    const string ISSUE_CATEGORY_ID_TO_REASSIGN = "10";
		    const string ISSUE_ID_REASSIGNED = "9";

		    var exception =
			    (RedmineException)
			    Record.Exception(
				    () => fixture.RedmineManager.DeleteObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, new NameValueCollection{{RedmineKeys.REASSIGN_TO_ID, ISSUE_CATEGORY_ID_TO_REASSIGN}}));
		    Assert.Null(exception);
		    Assert.Throws<NotFoundException>(
			    () => fixture.RedmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null));

		    var issue = fixture.RedmineManager.GetObject<Issue>(ISSUE_ID_REASSIGNED, null);
		    Assert.NotNull(issue.Category);
		    Assert.True(ISSUE_CATEGORY_ID_TO_REASSIGN.Equals(issue.Category.Id.ToString()), string.Format("Issue was not reassigned. Issue category id {0} != {1}", issue.Category.Id, ISSUE_CATEGORY_ID_TO_REASSIGN));
	    }
    }
}