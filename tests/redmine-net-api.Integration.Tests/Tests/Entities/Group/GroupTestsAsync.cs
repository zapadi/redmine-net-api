using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Group;

[Collection(Constants.RedmineTestContainerCollection)]
public class GroupTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllGroups_Should_Succeed()
    {
        // Act
        var groups = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Group>();

        // Assert
        Assert.NotNull(groups);
    }

    [Fact]
    public async Task CreateGroup_Should_Succeed()
    {
        // Arrange
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);

        // Act
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);

        // Assert
        Assert.NotNull(group);
        Assert.True(group.Id > 0);
        Assert.Equal(groupPayload.Name, group.Name);
    }

    [Fact]
    public async Task GetGroup_Should_Succeed()
    {
        // Arrange
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);

        // Act
        var retrievedGroup = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Group>(group.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedGroup);
        Assert.Equal(group.Id, retrievedGroup.Id);
        Assert.Equal(group.Name, retrievedGroup.Name);
    }

    [Fact]
    public async Task UpdateGroup_Should_Succeed()
    {
        // Arrange
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);

        group.Name = RandomHelper.GenerateText(7);

        // Act
        await fixture.RedmineManager.UpdateAsync(group.Id.ToInvariantString(), group);
        var retrievedGroup = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Group>(group.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedGroup);
        Assert.Equal(group.Id, retrievedGroup.Id);
        Assert.Equal(group.Name, retrievedGroup.Name);
    }

    [Fact]
    public async Task DeleteGroup_Should_Succeed()
    {
        // Arrange
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);

        var groupId = group.Id.ToInvariantString();

        // Act
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.Group>(groupId);

        // Assert
        await Assert.ThrowsAsync<RedmineNotFoundException>(async () => 
            await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Group>(groupId));
    }

    [Fact]
    public async Task AddUserToGroup_Should_Succeed()
    {
        // Arrange
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);
        
        // Act
        await fixture.RedmineManager.AddUserToGroupAsync(group.Id, userId: 1);
        var updatedGroup = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Group>(group.Id.ToString(), RequestOptions.Include(RedmineKeys.USERS));

        // Assert
        Assert.NotNull(updatedGroup);
        Assert.NotNull(updatedGroup.Users);
        Assert.Contains(updatedGroup.Users, ug => ug.Id == 1);
    }

    [Fact]
    public async Task RemoveUserFromGroup_Should_Succeed()
    {
        // Arrange
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);
        
        await fixture.RedmineManager.AddUserToGroupAsync(group.Id, userId: 1);
        
        // Act
        await fixture.RedmineManager.RemoveUserFromGroupAsync(group.Id, userId: 1);
        var updatedGroup = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Group>(group.Id.ToString(), RequestOptions.Include(RedmineKeys.USERS));

        // Assert
        Assert.NotNull(updatedGroup);
    }
}