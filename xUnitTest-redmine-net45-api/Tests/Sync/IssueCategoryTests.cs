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

	    private const string PROJECT_ID = "redmine-net-testq";

        [Fact, Order(1)]
        public void Should_Create_IssueCategory()
        {
	        const string NEW_ISSUE_CATEGORY_NAME = "Test category";
	        const int NEW_ISSUE_CATEGORY_ASIGNEE_ID = 5;
	        var issueCategory = new IssueCategory
            {
                Name = NEW_ISSUE_CATEGORY_NAME,
                AsignTo = new IdentifiableName {Id = NEW_ISSUE_CATEGORY_ASIGNEE_ID}
            };

            var savedIssueCategory = fixture.RedmineManager.CreateObject(issueCategory, PROJECT_ID);

            Assert.NotNull(savedIssueCategory);
            Assert.True(savedIssueCategory.Name.Equals(NEW_ISSUE_CATEGORY_NAME), "Saved issue category name is invalid.");
        }

        [Fact, Order(99)]
        public void Should_Delete_IssueCategory()
        {
	        const string ISSUE_CATEGORY_ID_TO_DELETE = "16";
	        var exception =
                (RedmineException)
                    Record.Exception(
                        () => fixture.RedmineManager.DeleteObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_DELETE, null));
        }

        [Fact, Order(2)]
        public void Should_Get_All_IssueCategories_By_ProjectId()
        {
	        const int NUMBER_OF_ISSUE_CATEGORIES = 2;
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
	        const string ISSUE_CATEGORY_ID_TO_GET = "17";
	        const string ISSUE_CATEGORY_NAME_TO_GET = "Test category";
	        const string ISSUE_CATEGORY_PROJECT_NAME_TO_GET = "redmine-net-testq";
	        const string ISSUE_CATEGORY_ASIGNEE_NAME_TO_GET = "Alina";

	        var issueCategory = fixture.RedmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_GET, null);

            Assert.NotNull(issueCategory);
            Assert.True(issueCategory.Name.Equals(ISSUE_CATEGORY_NAME_TO_GET), "Issue category name is invalid.");
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
	        const string ISSUE_CATEGORY_ID_TO_UPDATE = "17";
	        const string ISSUE_CATEGORY_NAME_TO_UPDATE = "Category updated";
	        const int ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE = 2;

	        var issueCategory = fixture.RedmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, null);
            issueCategory.Name = ISSUE_CATEGORY_NAME_TO_UPDATE;
            issueCategory.AsignTo = new IdentifiableName {Id = ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE};

            fixture.RedmineManager.UpdateObject(ISSUE_CATEGORY_ID_TO_UPDATE, issueCategory);

            var updatedIssueCategory = fixture.RedmineManager.GetObject<IssueCategory>(ISSUE_CATEGORY_ID_TO_UPDATE, null);

            Assert.NotNull(updatedIssueCategory);
            Assert.True(updatedIssueCategory.Name.Equals(ISSUE_CATEGORY_NAME_TO_UPDATE),
                "Issue category name was not updated.");
            Assert.NotNull(updatedIssueCategory.AsignTo);
            Assert.True(updatedIssueCategory.AsignTo.Id == ISSUE_CATEGORY_ASIGNEE_ID_TO_UPDATE,
                "Issue category asignee was not updated.");
        }
    }
}