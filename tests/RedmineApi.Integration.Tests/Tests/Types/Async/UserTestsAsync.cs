using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class UserTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<User> CreateTestUserAsync()
    {
        var user = new User
        {
            Login = RandomHelper.GenerateText(12),
            FirstName = RandomHelper.GenerateText(8),
            LastName = RandomHelper.GenerateText(10),
            Email = $"{RandomHelper.GenerateText(5)}.{RandomHelper.GenerateText(4)}@gmail.com",
            Password = "password123",
            AuthenticationModeId = null,
            MustChangePassword = false,
            Status = UserStatus.StatusActive
        };
        return await fixture.RedmineManager.CreateAsync(user);
    }

    [Fact]
    public async Task CreateUser_Should_Succeed()
    {
        //Arrange
        var userData = new User
        {
            Login = RandomHelper.GenerateText(5),
            FirstName = RandomHelper.GenerateText(5),
            LastName = RandomHelper.GenerateText(5),
            Password = "password123",
            MailNotification = "only_my_events",
            AuthenticationModeId = null,
            MustChangePassword = false,
            Status = UserStatus.StatusActive,
        };

        userData.Email = $"{userData.FirstName}.{userData.LastName}@gmail.com";

        //Act
        var createdUser = await fixture.RedmineManager.CreateAsync<User>(userData);

        //Assert
        Assert.NotNull(createdUser);
        Assert.True(createdUser.Id > 0);
        Assert.Equal(userData.Login, createdUser.Login);
        Assert.Equal(userData.FirstName, createdUser.FirstName);
        Assert.Equal(userData.LastName, createdUser.LastName);
        Assert.Equal(userData.Email, createdUser.Email);
    }

    [Fact]
    public async Task GetUser_Should_Succeed()
    {

        //Arrange
        var createdUser = await CreateTestUserAsync();
        Assert.NotNull(createdUser);

        //Act
        var retrievedUser =
            await fixture.RedmineManager.GetAsync<User>(createdUser.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(createdUser.Id, retrievedUser.Id);
        Assert.Equal(createdUser.Login, retrievedUser.Login);
        Assert.Equal(createdUser.FirstName, retrievedUser.FirstName);
    }

    [Fact]
    public async Task UpdateUser_Should_Succeed()
    {

        //Arrange
        var createdUser = await CreateTestUserAsync();
        Assert.NotNull(createdUser);

        var updatedFirstName = RandomHelper.GenerateText(10);
        createdUser.FirstName = updatedFirstName;

        //Act
        await fixture.RedmineManager.UpdateAsync(createdUser.Id.ToInvariantString(), createdUser);
        var retrievedUser =
            await fixture.RedmineManager.GetAsync<User>(createdUser.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(createdUser.Id, retrievedUser.Id);
        Assert.Equal(updatedFirstName, retrievedUser.FirstName);
    }

    [Fact]
    public async Task DeleteUser_Should_Succeed()
    {
        //Arrange
        var createdUser = await CreateTestUserAsync();
        Assert.NotNull(createdUser);
        var userId = createdUser.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<User>(userId);

        //Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(async () => await fixture.RedmineManager.GetAsync<User>(userId));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}