using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueJournalTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetIssueWithJournals_Should_Succeed()
    {
        // Arrange 
        var issue = IssueTestHelper.CreateIssue();
        var createdIssue = fixture.RedmineManager.Create(issue);
        Assert.NotNull(createdIssue);

        // Add note to create the journal
        var update = new Issue
        {
            Notes   = "This is a test note that should appear in journals",
            Subject = $"Updated subject {Guid.NewGuid()}"
        };
        fixture.RedmineManager.Update(createdIssue.Id.ToString(), update);

        // Act
        var retrievedIssue = fixture.RedmineManager.Get<Issue>(
            createdIssue.Id.ToString(),
            RequestOptions.Include("journals"));

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotEmpty(retrievedIssue.Journals);
        Assert.Contains(retrievedIssue.Journals, j => j.Notes?.Contains("test note") == true);
    }
}