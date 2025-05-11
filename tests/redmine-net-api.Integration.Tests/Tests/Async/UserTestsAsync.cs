using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class UserTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<User> CreateTestUserAsync()
    {
        var user = new User
        {
            Login = ThreadSafeRandom.GenerateText(12),
            FirstName = ThreadSafeRandom.GenerateText(8),
            LastName = ThreadSafeRandom.GenerateText(10),
            Email = $"{ThreadSafeRandom.GenerateText(5)}.{ThreadSafeRandom.GenerateText(4)}@gmail.com",
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
            Login = ThreadSafeRandom.GenerateText(5),
            FirstName = ThreadSafeRandom.GenerateText(5),
            LastName = ThreadSafeRandom.GenerateText(5),
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

        var updatedFirstName = ThreadSafeRandom.GenerateText(10);
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
        await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetAsync<User>(userId));
    }
}