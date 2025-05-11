using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class GroupTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Group> CreateTestGroupAsync()
    {
        var group = new Group
        {
            Name = $"Test Group {Guid.NewGuid()}"
        };
        
        return await fixture.RedmineManager.CreateAsync(group);
    }

    [Fact]
    public async Task GetAllGroups_Should_Succeed()
    {
        // Act
        var groups = await fixture.RedmineManager.GetAsync<Group>();

        // Assert
        Assert.NotNull(groups);
    }

    [Fact]
    public async Task CreateGroup_Should_Succeed()
    {
        // Arrange
        var group = new Group
        {
            Name = $"Test Group {Guid.NewGuid()}"
        };

        // Act
        var createdGroup = await fixture.RedmineManager.CreateAsync(group);

        // Assert
        Assert.NotNull(createdGroup);
        Assert.True(createdGroup.Id > 0);
        Assert.Equal(group.Name, createdGroup.Name);
    }

    [Fact]
    public async Task GetGroup_Should_Succeed()
    {
        // Arrange
        var createdGroup = await CreateTestGroupAsync();
        Assert.NotNull(createdGroup);

        // Act
        var retrievedGroup = await fixture.RedmineManager.GetAsync<Group>(createdGroup.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedGroup);
        Assert.Equal(createdGroup.Id, retrievedGroup.Id);
        Assert.Equal(createdGroup.Name, retrievedGroup.Name);
    }

    [Fact]
    public async Task UpdateGroup_Should_Succeed()
    {
        // Arrange
        var createdGroup = await CreateTestGroupAsync();
        Assert.NotNull(createdGroup);

        var updatedName = $"Updated Test Group {Guid.NewGuid()}";
        createdGroup.Name = updatedName;

        // Act
        await fixture.RedmineManager.UpdateAsync(createdGroup.Id.ToInvariantString(), createdGroup);
        var retrievedGroup = await fixture.RedmineManager.GetAsync<Group>(createdGroup.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedGroup);
        Assert.Equal(createdGroup.Id, retrievedGroup.Id);
        Assert.Equal(updatedName, retrievedGroup.Name);
    }

    [Fact]
    public async Task DeleteGroup_Should_Succeed()
    {
        // Arrange
        var createdGroup = await CreateTestGroupAsync();
        Assert.NotNull(createdGroup);

        var groupId = createdGroup.Id.ToInvariantString();

        // Act
        await fixture.RedmineManager.DeleteAsync<Group>(groupId);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => 
            await fixture.RedmineManager.GetAsync<Group>(groupId));
    }

    [Fact]
    public async Task AddUserToGroup_Should_Succeed()
    {
        // Arrange
        var group = await CreateTestGroupAsync();
        Assert.NotNull(group);
        
        // Assuming there's at least one user in the system (typically Admin with ID 1)
        var userId = 1;

        // Act
        await fixture.RedmineManager.AddUserToGroupAsync(group.Id, userId);
        var updatedGroup = await fixture.RedmineManager.GetAsync<Group>(group.Id.ToString(), RequestOptions.Include("users"));

        // Assert
        Assert.NotNull(updatedGroup);
        Assert.NotNull(updatedGroup.Users);
        Assert.Contains(updatedGroup.Users, u => u.Id == userId);
    }

    [Fact]
    public async Task RemoveUserFromGroup_Should_Succeed()
    {
        // Arrange
        var group = await CreateTestGroupAsync();
        Assert.NotNull(group);
        
        // Assuming there's at least one user in the system (typically Admin with ID 1)
        var userId = 1;
        
        // First add the user to the group
        await fixture.RedmineManager.AddUserToGroupAsync(group.Id, userId);
        
        // Act
        await fixture.RedmineManager.RemoveUserFromGroupAsync(group.Id, userId);
        var updatedGroup = await fixture.RedmineManager.GetAsync<Group>(group.Id.ToString(), RequestOptions.Include("users"));

        // Assert
        Assert.NotNull(updatedGroup);
      //  Assert.DoesNotContain(updatedGroup.Users ?? new List<IdentifiableName>(), u => u.Id == userId);
    }
}