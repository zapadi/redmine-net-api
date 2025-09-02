using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueRelationTests(RedmineTestContainerFixture fixture)
{
    private (Issue first, Issue second) CreateTwoIssues()
    {
        Issue Build(string subject) => fixture.RedmineManager.Create(new Issue
        {
            Project     = new IdentifiableName { Id = 1 },
            Tracker     = new IdentifiableName { Id = 1 },
            Status      = new IssueStatus { Id = 1 },
            Priority    = new IdentifiableName { Id = 4 },
            Subject     = subject,
            Description = "desc"
        });

        return (Build($"Issue1 {Guid.NewGuid()}"), Build($"Issue2 {Guid.NewGuid()}"));
    }

    private IssueRelation CreateRelation()
    {
        var (i1, i2) = CreateTwoIssues();
        var rel      = new IssueRelation { IssueId = i1.Id, IssueToId = i2.Id, Type = IssueRelationType.Relates };
        return fixture.RedmineManager.Create(rel, i1.Id.ToString());
    }

    [Fact]
    public void CreateIssueRelation_Should_Succeed()
    {
        var (i1, i2) = CreateTwoIssues();
        var rel = fixture.RedmineManager.Create(
            new IssueRelation { IssueId = i1.Id, IssueToId = i2.Id, Type = IssueRelationType.Relates },
            i1.Id.ToString());

        Assert.NotNull(rel);
        Assert.True(rel.Id > 0);
        Assert.Equal(i1.Id, rel.IssueId);
        Assert.Equal(i2.Id, rel.IssueToId);
    }

    [Fact]
    public void DeleteIssueRelation_Should_Succeed()
    {
        var rel = CreateRelation();
        fixture.RedmineManager.Delete<IssueRelation>(rel.Id.ToString());

        var issue = fixture.RedmineManager.Get<Issue>(
            rel.IssueId.ToString(),
            RequestOptions.Include("relations"));

        Assert.Null(issue.Relations?.FirstOrDefault(r => r.Id == rel.Id));
    }
}