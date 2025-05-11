using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class TimeEntryActivityTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllTimeEntryActivities_Should_Succeed()
    {
        // Act
        var activities = await fixture.RedmineManager.GetAsync<TimeEntryActivity>();

        // Assert
        Assert.NotNull(activities);
        Assert.NotEmpty(activities);
    }
}