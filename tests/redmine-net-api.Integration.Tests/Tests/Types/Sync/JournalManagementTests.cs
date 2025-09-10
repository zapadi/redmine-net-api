using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class JournalTests(RedmineTestContainerFixture fixture)
{
    private Issue CreateTestIssue()
    {
        var issue = IssueTestHelper.CreateIssue();
        return fixture.RedmineManager.Create(issue);
    }

    [Fact]
    public void Get_Issue_With_Journals_Should_Succeed()
    {
        // Arrange
        var testIssue = CreateTestIssue();
        Assert.NotNull(testIssue);

        testIssue.Notes = "This is a test note to create a journal entry.";
        fixture.RedmineManager.Update(testIssue.Id.ToInvariantString(), testIssue);

        // Act
        var issueWithJournals = fixture.RedmineManager.Get<Issue>(
            testIssue.Id.ToInvariantString(),
            RequestOptions.Include(RedmineKeys.JOURNALS));

        // Assert
        Assert.NotNull(issueWithJournals);
        Assert.NotNull(issueWithJournals.Journals);
        Assert.True(issueWithJournals.Journals.Count > 0);
        Assert.Contains(issueWithJournals.Journals, j => j.Notes == testIssue.Notes);
    }
}