using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.IssueRelation;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueRelationTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task CreateIssueRelation_Should_Succeed()
    {
        // Arrange
        var (issue1, issue2) = await IssueTestHelper.CreateRandomTwoIssuesAsync(fixture.RedmineManager);
        
        var relation = new Redmine.Net.Api.Types.IssueRelation
        {
            IssueId = issue1.Id,
            IssueToId = issue2.Id,
            Type = IssueRelationType.Relates
        };

        // Act
        var createdRelation = await fixture.RedmineManager.CreateAsync(relation, issue1.Id.ToString());

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
        var (relation, _, _) = await IssueTestHelper.CreateRandomIssueRelationAsync(fixture.RedmineManager);
        Assert.NotNull(relation);

        // Act & Assert
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.IssueRelation>(relation.Id.ToString());
        
        var issue = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(relation.IssueId.ToString(), RequestOptions.Include(RedmineKeys.RELATIONS));
        
        Assert.Null(issue.Relations?.FirstOrDefault(r => r.Id == relation.Id));
    }
}