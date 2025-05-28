using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueWatcherTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Redmine.Net.Api.Types.Issue> CreateTestIssueAsync()
    {
        var issue = new Redmine.Net.Api.Types.Issue
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
        
        const int userId = 1;

        // Act
        await fixture.RedmineManager.AddWatcherToIssueAsync(issue.Id, userId);
        
        var updatedIssue = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(issue.Id.ToString(), RequestOptions.Include(RedmineKeys.WATCHERS));

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
        
        const int userId = 1;
        
        await fixture.RedmineManager.AddWatcherToIssueAsync(issue.Id, userId);

        // Act
        await fixture.RedmineManager.RemoveWatcherFromIssueAsync(issue.Id, userId);
        
        var updatedIssue = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(issue.Id.ToString(), RequestOptions.Include(RedmineKeys.WATCHERS));

        // Assert
        Assert.NotNull(updatedIssue);
        Assert.DoesNotContain(updatedIssue.Watchers ?? [], w => w.Id == userId);
    }
}