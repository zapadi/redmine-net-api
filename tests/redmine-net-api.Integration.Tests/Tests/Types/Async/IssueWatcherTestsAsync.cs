using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueWatcherTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Issue> CreateTestIssueAsync()
    {
        var issue = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue subject {Guid.NewGuid()}",
            Description = "Test issue description"
        };
        
        return await fixture.RedmineManager.CreateAsync(issue);
    }

    [Fact]
    public async Task AddWatcher_Should_Succeed()
    {
        // Arrange
        var issue = await CreateTestIssueAsync();
        Assert.NotNull(issue);
        
        // Assuming there's at least one user in the system (typically Admin with ID 1)
        var userId = 1;

        // Act
        await fixture.RedmineManager.AddWatcherToIssueAsync(issue.Id, userId);
        
        // Get updated issue with watchers
        var updatedIssue = await fixture.RedmineManager.GetAsync<Issue>(issue.Id.ToString(), RequestOptions.Include("watchers"));

        // Assert
        Assert.NotNull(updatedIssue);
        Assert.NotNull(updatedIssue.Watchers);
        Assert.Contains(updatedIssue.Watchers, w => w.Id == userId);
    }

    [Fact]
    public async Task RemoveWatcher_Should_Succeed()
    {
        // Arrange
        var issue = await CreateTestIssueAsync();
        Assert.NotNull(issue);
        
        // Assuming there's at least one user in the system (typically Admin with ID 1)
        var userId = 1;
        
        // Add watcher first
        await fixture.RedmineManager.AddWatcherToIssueAsync(issue.Id, userId);

        // Act
        await fixture.RedmineManager.RemoveWatcherFromIssueAsync(issue.Id, userId);
        
        // Get updated issue with watchers
        var updatedIssue = await fixture.RedmineManager.GetAsync<Issue>(issue.Id.ToString(), RequestOptions.Include("watchers"));

        // Assert
        Assert.NotNull(updatedIssue);
        Assert.DoesNotContain(updatedIssue.Watchers ?? [], w => w.Id == userId);
    }
}