using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

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