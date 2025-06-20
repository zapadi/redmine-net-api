using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Role;

[Collection(Constants.RedmineTestContainerCollection)]
public class RoleTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Get_All_Roles_Should_Succeed()
    {
        //Act
        var roles = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Role>();

        //Assert
        Assert.NotNull(roles);
        Assert.NotEmpty(roles);
    }
}