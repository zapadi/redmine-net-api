using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueJournalTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetIssueWithJournals_Should_Succeed()
    {
        // Arrange
        // Create an issue
        var issue = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue for journals {Guid.NewGuid()}",
            Description = "Test issue description"
        };
        
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        Assert.NotNull(createdIssue);
        
        // Update the issue to create a journal entry
        var updateIssue = new Issue
        {
            Notes = "This is a test note that should appear in journals",
            Subject = $"Updated subject {Guid.NewGuid()}"
        };
        
        await fixture.RedmineManager.UpdateAsync(createdIssue.Id.ToString(), updateIssue);

        // Act
        // Get the issue with journals
        var retrievedIssue =
            await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(),
                RequestOptions.Include("journals"));

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Journals);
        Assert.NotEmpty(retrievedIssue.Journals);
        Assert.Contains(retrievedIssue.Journals, j => j.Notes?.Contains("test note") == true);
    }
}