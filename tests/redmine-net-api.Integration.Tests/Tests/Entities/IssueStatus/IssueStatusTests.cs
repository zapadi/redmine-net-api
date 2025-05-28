using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueStatusTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllIssueStatuses_Should_Succeed()
    {
        var statuses = fixture.RedmineManager.Get<IssueStatus>();
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }
}