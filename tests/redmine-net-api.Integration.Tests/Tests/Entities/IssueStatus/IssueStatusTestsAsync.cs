using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.IssueStatus;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueStatusAsyncTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllIssueStatuses_Should_Succeed()
    {
        // Act
        var statuses = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.IssueStatus>();

        // Assert
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }
}