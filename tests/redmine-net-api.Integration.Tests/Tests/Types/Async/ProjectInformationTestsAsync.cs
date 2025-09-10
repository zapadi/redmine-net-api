using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Async;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectInformationTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetCurrentUserInfo_Should_Succeed()
    {
        // Act
        var currentUser = await fixture.RedmineManager.GetCurrentUserAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(currentUser);
        Assert.True(currentUser.Id > 0);
    }
}