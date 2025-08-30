using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectInformationTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetCurrentUserInfo_Should_Succeed()
    {
        // Act
        var currentUser = await fixture.RedmineManager.GetCurrentUserAsync();

        // Assert
        Assert.NotNull(currentUser);
        Assert.True(currentUser.Id > 0);
    }
}