using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class ApiCoverageTestsAsync(RedmineTestContainerFixture fixture)
{
    private static User NewUser(bool isAdmin = false, UserStatus status = UserStatus.StatusActive)
    {
        return new User
        {
            Login = RandomHelper.GenerateText(12),
            FirstName = RandomHelper.GenerateText(8),
            LastName = RandomHelper.GenerateText(10),
            Email = $"{RandomHelper.GenerateText(5)}.{RandomHelper.GenerateText(4)}@gmail.com",
            Password = "password123",
            IsAdmin = isAdmin,
            Status = status
        };
    }

    [Fact]
    public async Task CreateUser_With_IsAdmin_Should_Create_Admin()
    {
        var ct = TestContext.Current.CancellationToken;

        var created = await fixture.RedmineManager.CreateAsync(NewUser(isAdmin: true), cancellationToken: ct);
        var user = await fixture.RedmineManager.GetAsync<User>(created.Id.ToInvariantString(), cancellationToken: ct);

        Assert.True(user.IsAdmin);
    }

    [Fact]
    public async Task UpdateUser_Should_Revoke_Admin()
    {
        var ct = TestContext.Current.CancellationToken;
        var created = await fixture.RedmineManager.CreateAsync(NewUser(isAdmin: true), cancellationToken: ct);
        var user = await fixture.RedmineManager.GetAsync<User>(created.Id.ToInvariantString(), cancellationToken: ct);

        user.IsAdmin = false;
        await fixture.RedmineManager.UpdateAsync(user.Id.ToInvariantString(), user, cancellationToken: ct);
        var updated = await fixture.RedmineManager.GetAsync<User>(user.Id.ToInvariantString(), cancellationToken: ct);

        Assert.False(updated.IsAdmin);
    }

    [Fact]
    public async Task CreateUser_Without_Status_Should_Get_Redmine_Default_Status()
    {
        var ct = TestContext.Current.CancellationToken;

        var created = await fixture.RedmineManager.CreateAsync(NewUser(status: UserStatus.StatusAnonymous), cancellationToken: ct);
        var user = await fixture.RedmineManager.GetAsync<User>(created.Id.ToInvariantString(), cancellationToken: ct);

        Assert.Equal(UserStatus.StatusActive, user.Status);
    }
}
