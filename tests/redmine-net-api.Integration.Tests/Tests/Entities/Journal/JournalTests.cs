using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class JournalTests(RedmineTestContainerFixture fixture)
{
    private Redmine.Net.Api.Types.Issue CreateRandomIssue()
    {
        var issue = TestEntityFactory.CreateRandomIssuePayload();
        return fixture.RedmineManager.Create(issue);
    }

    [Fact]
    public void Get_Issue_With_Journals_Should_Succeed()
    {
        // Arrange
        var testIssue = CreateRandomIssue();
        Assert.NotNull(testIssue);

        testIssue.Notes = "This is a test note to create a journal entry.";
        fixture.RedmineManager.Update(testIssue.Id.ToInvariantString(), testIssue);

        // Act
        var issueWithJournals = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(
            testIssue.Id.ToInvariantString(),
            RequestOptions.Include(RedmineKeys.JOURNALS));

        // Assert
        Assert.NotNull(issueWithJournals);
        Assert.NotNull(issueWithJournals.Journals);
        Assert.True(issueWithJournals.Journals.Count > 0);
        Assert.Contains(issueWithJournals.Journals, j => j.Notes == testIssue.Notes);
    }
}