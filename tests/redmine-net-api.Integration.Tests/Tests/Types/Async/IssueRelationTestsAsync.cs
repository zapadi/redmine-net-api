using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueRelationTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<(Issue firstIssue, Issue secondIssue)> CreateTestIssuesAsync()
    {
        var issue1 = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue 1 subject {Guid.NewGuid()}",
            Description = "Test issue 1 description"
        };
        
        var issue2 = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue 2 subject {Guid.NewGuid()}",
            Description = "Test issue 2 description"
        };
        
        var createdIssue1 = await fixture.RedmineManager.CreateAsync(issue1);
        var createdIssue2 = await fixture.RedmineManager.CreateAsync(issue2);
        
        return (createdIssue1, createdIssue2);
    }

    private async Task<IssueRelation> CreateTestIssueRelationAsync()
    {
        var (issue1, issue2) = await CreateTestIssuesAsync();
        
        var relation = new IssueRelation
        {
            IssueId = issue1.Id,
            IssueToId = issue2.Id,
            Type = IssueRelationType.Relates
        };
        
        return await fixture.RedmineManager.CreateAsync( relation, issue1.Id.ToString());
    }

    [Fact]
    public async Task CreateIssueRelation_Should_Succeed()
    {
        // Arrange
        var (issue1, issue2) = await CreateTestIssuesAsync();
        
        var relation = new IssueRelation
        {
            IssueId = issue1.Id,
            IssueToId = issue2.Id,
            Type = IssueRelationType.Relates
        };

        // Act
        var createdRelation = await fixture.RedmineManager.CreateAsync(relation, issue1.Id.ToString(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(createdRelation);
        Assert.True(createdRelation.Id > 0);
        Assert.Equal(relation.IssueId, createdRelation.IssueId);
        Assert.Equal(relation.IssueToId, createdRelation.IssueToId);
        Assert.Equal(relation.Type, createdRelation.Type);
    }

    [Fact]
    public async Task DeleteIssueRelation_Should_Succeed()
    {
        // Arrange
        var relation = await CreateTestIssueRelationAsync();
        Assert.NotNull(relation);

        // Act & Assert
        await fixture.RedmineManager.DeleteAsync<IssueRelation>(relation.Id.ToString(), cancellationToken: TestContext.Current.CancellationToken);
        
        // Verify the relation no longer exists by checking the issue doesn't have it
        var issue = await fixture.RedmineManager.GetAsync<Issue>(relation.IssueId.ToString(), RequestOptions.Include("relations"), TestContext.Current.CancellationToken);
        
        Assert.Null(issue.Relations?.FirstOrDefault(r => r.Id == relation.Id));
    }
}