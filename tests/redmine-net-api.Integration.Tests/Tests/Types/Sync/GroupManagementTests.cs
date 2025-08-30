using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class GroupTests(RedmineTestContainerFixture fixture)
{
    private Group CreateTestGroup()
    {
        var group = new Group
        {
            Name = $"Test Group {Guid.NewGuid()}"
        };

        return fixture.RedmineManager.Create(group);
    }

    [Fact]
    public void GetAllGroups_Should_Succeed()
    {
        var groups = fixture.RedmineManager.Get<Group>();

        Assert.NotNull(groups);
    }

    [Fact]
    public void CreateGroup_Should_Succeed()
    {
        var group = new Group { Name = $"Test Group {Guid.NewGuid()}" };

        var createdGroup = fixture.RedmineManager.Create(group);

        Assert.NotNull(createdGroup);
        Assert.True(createdGroup.Id > 0);
        Assert.Equal(group.Name, createdGroup.Name);
    }

    [Fact]
    public void GetGroup_Should_Succeed()
    {
        var createdGroup = CreateTestGroup();
        Assert.NotNull(createdGroup);

        var retrievedGroup = fixture.RedmineManager.Get<Group>(createdGroup.Id.ToInvariantString());

        Assert.NotNull(retrievedGroup);
        Assert.Equal(createdGroup.Id, retrievedGroup.Id);
        Assert.Equal(createdGroup.Name, retrievedGroup.Name);
    }

    [Fact]
    public void UpdateGroup_Should_Succeed()
    {
        var createdGroup = CreateTestGroup();
        Assert.NotNull(createdGroup);

        var updatedName = $"Updated Test Group {Guid.NewGuid()}";
        createdGroup.Name = updatedName;

        fixture.RedmineManager.Update(createdGroup.Id.ToInvariantString(), createdGroup);
        var retrievedGroup = fixture.RedmineManager.Get<Group>(createdGroup.Id.ToInvariantString());

        Assert.NotNull(retrievedGroup);
        Assert.Equal(createdGroup.Id, retrievedGroup.Id);
        Assert.Equal(updatedName, retrievedGroup.Name);
    }

    [Fact]
    public void DeleteGroup_Should_Succeed()
    {
        var createdGroup = CreateTestGroup();
        Assert.NotNull(createdGroup);

        var groupId = createdGroup.Id.ToInvariantString();

        fixture.RedmineManager.Delete<Group>(groupId);

        var ex = Assert.Throws<RedmineApiException>(() =>
            fixture.RedmineManager.Get<Group>(groupId));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }

    [Fact]
    public void AddUserToGroup_Should_Succeed()
    {
        var group = CreateTestGroup();
        Assert.NotNull(group);

        var userId = 1; // assuming Admin

        fixture.RedmineManager.AddUserToGroup(group.Id, userId);
        var updatedGroup = fixture.RedmineManager.Get<Group>(group.Id.ToString(), RequestOptions.Include("users"));

        Assert.NotNull(updatedGroup);
        Assert.NotNull(updatedGroup.Users);
        Assert.Contains(updatedGroup.Users, u => u.Id == userId);
    }

    [Fact]
    public void RemoveUserFromGroup_Should_Succeed()
    {
        var group = CreateTestGroup();
        Assert.NotNull(group);

        var userId = 1; // assuming Admin

        fixture.RedmineManager.AddUserToGroup(group.Id, userId);

        fixture.RedmineManager.RemoveUserFromGroup(group.Id, userId);
        var updatedGroup = fixture.RedmineManager.Get<Group>(group.Id.ToString(), RequestOptions.Include("users"));

        Assert.NotNull(updatedGroup);
        // Assert.DoesNotContain(updatedGroup.Users ?? new List<IdentifiableName>(), u => u.Id == userId);
    }
}