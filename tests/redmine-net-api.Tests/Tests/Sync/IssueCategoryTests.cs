using System.Collections.Specialized;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
	[Trait("Redmine-Net-Api", "IssueCategories")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class IssueCategoryTests
    {
        private readonly RedmineFixture fixture;

        private const string PROJECT_ID = "redmine-net-testq";
        private const string NEW_ISSUE_CATEGORY_NAME = "Test category";
        private const int NEW_ISSUE_CATEGORY_ASIGNEE_ID = 1;
        private static string createdIssueCategoryId;

        public IssueCategoryTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact, Order(1)]
        public void Should_Create_IssueCategory()
        {
	        var issueCategory = new IssueCategory
            {
                Name = NEW_ISSUE_CATEGORY_NAME,
                AssignTo =  IdentifiableName.Create<IdentifiableName>(NEW_ISSUE_CATEGORY_ASIGNEE_ID)
            };

            var savedIssueCategory = fixture.RedmineManager.CreateObject(issueCategory, PROJECT_ID);

	        createdIssueCategoryId = savedIssueCategory.Id.ToString();

            Assert.NotNull(savedIssueCategory);
            Assert.True(savedIssueCategory.Name.Equals(NEW_ISSUE_CATEGORY_NAME), "Saved issue category name is invalid.");
        }

        [Fact, Order(99)]
        public void Should_Delete_IssueCategory()
        {
	        var exception =
                (RedmineException)
                    Record.Exception(
                        () => fixture.RedmineManager.DeleteObject<IssueCategory>(createdIssueCategoryId));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.GetObject<IssueCategory>(createdIssueCategoryId, null));
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
            Assert.True(issueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES,
                "Number of issue categories ( "+issueCategories.Count+" ) != " + NUMBER_OF_ISSUE_CATEGORIES);
        }

        [Fact, Order(3)]
        public void Should_Get_IssueCategory_By_Id()
        {
	        const string ISSUE_CATEGORY_PROJECT_NAME_TO_GET = "Redmine tests";
	        const string ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET = "Redmine";

	        var issueCategory = fixture.RedmineManager.GetObject<IssueCategory>(createdIssueCategoryId, null);

            Assert.NotNull(issueCategory);
            Assert.True(issueCategory.Name.Equals(NEW_ISSUE_CATEGORY_NAME), "Issue category name is invalid.");
            Assert.NotNull(issueCategory.AssignTo);
            Assert.True(issueCategory.AssignTo.Name.Contains(ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET),
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

	        var issueCategory = fixture.RedmineManager.GetObject<IssueCategory>(createdIssueCategoryId, null);
            issueCategory.Name = ISSUE_CATEGORY_NAME_TO_UPDATE;
            issueCategory.AssignTo = IdentifiableName.Create<IdentifiableName>(ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE);

            fixture.RedmineManager.UpdateObject(createdIssueCategoryId, issueCategory);

            var updatedIssueCategory = fixture.RedmineManager.GetObject<IssueCategory>(createdIssueCategoryId, null);

            Assert.NotNull(updatedIssueCategory);
            Assert.True(updatedIssueCategory.Name.Equals(ISSUE_CATEGORY_NAME_TO_UPDATE),
                "Issue category name was not updated.");
            Assert.NotNull(updatedIssueCategory.AssignTo);
            Assert.True(updatedIssueCategory.AssignTo.Id == ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE,
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