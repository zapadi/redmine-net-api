
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

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