using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class TimeEntryActivityTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllTimeEntryActivities_Should_Succeed()
    {
        // Act
        var activities = await fixture.RedmineManager.GetAsync<TimeEntryActivity>(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(activities);
        Assert.NotEmpty(activities);
    }
}