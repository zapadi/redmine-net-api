using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

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