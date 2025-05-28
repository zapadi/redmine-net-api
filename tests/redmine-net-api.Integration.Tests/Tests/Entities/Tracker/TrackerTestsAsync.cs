using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Tracker;

[Collection(Constants.RedmineTestContainerCollection)]
public class TrackerTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Get_All_Trackers_Should_Succeed()
    {
        //Act
        var trackers = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Tracker>();

        //Assert
        Assert.NotNull(trackers);
        Assert.NotEmpty(trackers);
    }
}