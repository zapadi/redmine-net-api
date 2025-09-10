using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class RoleTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Get_All_Roles_Should_Succeed()
    {
        //Act
        var roles = await fixture.RedmineManager.GetAsync<Role>(cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.NotNull(roles);
        Assert.NotEmpty(roles);
    }
}