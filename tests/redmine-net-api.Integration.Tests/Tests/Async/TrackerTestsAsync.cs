using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class TrackerTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Get_All_Trackers_Should_Succeed()
    {
        //Act
        var trackers = await fixture.RedmineManager.GetAsync<Tracker>();

        //Assert
        Assert.NotNull(trackers);
        Assert.NotEmpty(trackers);
    }
}