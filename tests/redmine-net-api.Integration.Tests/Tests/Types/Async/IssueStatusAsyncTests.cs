using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueStatusTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllIssueStatuses_Should_Succeed()
    {
        // Act
        var statuses = await fixture.RedmineManager.GetAsync<IssueStatus>();

        // Assert
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }
}