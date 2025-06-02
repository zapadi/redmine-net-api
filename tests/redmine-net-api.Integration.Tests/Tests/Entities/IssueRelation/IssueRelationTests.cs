using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.IssueRelation;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueRelationTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void CreateIssueRelation_Should_Succeed()
    {
        var (relation, i1, i2) = IssueTestHelper.CreateRandomIssueRelation(fixture.RedmineManager);

        Assert.NotNull(relation);
        Assert.True(relation.Id > 0);
        Assert.Equal(i1.Id, relation.IssueId);
        Assert.Equal(i2.Id, relation.IssueToId);
    }

    [Fact]
    public void DeleteIssueRelation_Should_Succeed()
    {
        var (rel, _, _) = IssueTestHelper.CreateRandomIssueRelation(fixture.RedmineManager);
        fixture.RedmineManager.Delete<Redmine.Net.Api.Types.IssueRelation>(rel.Id.ToString());

        var issue = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(
            rel.IssueId.ToString(),
            RequestOptions.Include(RedmineKeys.RELATIONS));

        Assert.Null(issue.Relations?.FirstOrDefault(r => r.Id == rel.Id));
    }
}