using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class TrackerTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Get_All_Trackers_Should_Succeed()
    {
        //Act
        var trackers = await fixture.RedmineManager.GetAsync<Tracker>(cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.NotNull(trackers);
        Assert.NotEmpty(trackers);
    }
}