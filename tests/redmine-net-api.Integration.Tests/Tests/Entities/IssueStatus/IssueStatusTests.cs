using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.IssueStatus;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueStatusTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllIssueStatuses_Should_Succeed()
    {
        var statuses = fixture.RedmineManager.Get<Redmine.Net.Api.Types.IssueStatus>();
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }
}