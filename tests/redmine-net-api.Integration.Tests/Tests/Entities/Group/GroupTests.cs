using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Group;

[Collection(Constants.RedmineTestContainerCollection)]
public class GroupTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllGroups_Should_Succeed()
    {
        var groups = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Group>();

        Assert.NotNull(groups);
    }

    [Fact]
    public void CreateGroup_Should_Succeed()
    {
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = fixture.RedmineManager.Create(groupPayload);
        Assert.NotNull(group);

        Assert.NotNull(group);
        Assert.True(group.Id > 0);
        Assert.Equal(group.Name, group.Name);
    }

    [Fact]
    public void GetGroup_Should_Succeed()
    {
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = fixture.RedmineManager.Create(groupPayload);
        Assert.NotNull(group);

        var retrievedGroup = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Group>(group.Id.ToInvariantString());

        Assert.NotNull(retrievedGroup);
        Assert.Equal(group.Id, retrievedGroup.Id);
        Assert.Equal(group.Name, retrievedGroup.Name);
    }

    [Fact]
    public void UpdateGroup_Should_Succeed()
    {
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = fixture.RedmineManager.Create(groupPayload);
        Assert.NotNull(group);

        group.Name = RandomHelper.GenerateText(7);

        fixture.RedmineManager.Update(group.Id.ToInvariantString(), group);
        var retrievedGroup = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Group>(group.Id.ToInvariantString());

        Assert.NotNull(retrievedGroup);
        Assert.Equal(group.Id, retrievedGroup.Id);
        Assert.Equal(group.Name, retrievedGroup.Name);
    }

    [Fact]
    public void DeleteGroup_Should_Succeed()
    {
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = fixture.RedmineManager.Create(groupPayload);
        Assert.NotNull(group);

        var groupId = group.Id.ToInvariantString();

        fixture.RedmineManager.Delete<Redmine.Net.Api.Types.Group>(groupId);

        Assert.Throws<NotFoundException>(() =>
            fixture.RedmineManager.Get<Redmine.Net.Api.Types.Group>(groupId));
    }

    [Fact]
    public void AddUserToGroup_Should_Succeed()
    {
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = fixture.RedmineManager.Create(groupPayload);
        Assert.NotNull(group);

        var userId = 1; 

        fixture.RedmineManager.AddUserToGroup(group.Id, userId);
        var updatedGroup = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Group>(group.Id.ToString(), RequestOptions.Include(RedmineKeys.USERS));

        Assert.NotNull(updatedGroup);
        Assert.NotNull(updatedGroup.Users);
        Assert.Contains(updatedGroup.Users, u => u.Id == userId);
    }

    [Fact]
    public void RemoveUserFromGroup_Should_Succeed()
    {
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: null);
        var group = fixture.RedmineManager.Create(groupPayload);
        Assert.NotNull(group);

        fixture.RedmineManager.AddUserToGroup(group.Id, userId: 1);

        fixture.RedmineManager.RemoveUserFromGroup(group.Id, userId: 1);
        var updatedGroup = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Group>(group.Id.ToString(), RequestOptions.Include(RedmineKeys.USERS));

        Assert.NotNull(updatedGroup);
    }
}