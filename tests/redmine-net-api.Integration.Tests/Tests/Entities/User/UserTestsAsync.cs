using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Http.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.User;

[Collection(Constants.RedmineTestContainerCollection)]
public class UserTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task CreateUser_WithValidData_ShouldSucceed()
    {
        //Arrange
        var userPayload = TestEntityFactory.CreateRandomUserPayload(emailNotificationType: EmailNotificationType.OnlyMyEvents);

        //Act
        var createdUser = await fixture.RedmineManager.CreateAsync<Redmine.Net.Api.Types.User>(userPayload);

        //Assert
        Assert.NotNull(createdUser);
        Assert.True(createdUser.Id > 0);
        Assert.Equal(userPayload.Login, createdUser.Login);
        Assert.Equal(userPayload.FirstName, createdUser.FirstName);
        Assert.Equal(userPayload.LastName, createdUser.LastName);
        Assert.Equal(userPayload.Email, createdUser.Email);
    }

    [Fact]
    public async Task GetUser_WithValidId_ShouldReturnUser()
    {
        //Arrange
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);

        //Act
        var retrievedUser = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(user.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Id, retrievedUser.Id);
        Assert.Equal(user.Login, retrievedUser.Login);
        Assert.Equal(user.FirstName, retrievedUser.FirstName);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldSucceed()
    {
        //Arrange
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);

        user.FirstName = RandomHelper.GenerateText(10);

        //Act
        await fixture.RedmineManager.UpdateAsync(user.Id.ToInvariantString(), user);
        var retrievedUser = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(user.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Id, retrievedUser.Id);
        Assert.Equal(user.FirstName, retrievedUser.FirstName);
    }

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldSucceed()
    {
        //Arrange
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var userId = user.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.User>(userId);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(userId));
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnUserDetails()
    {
        var currentUser = await fixture.RedmineManager.GetCurrentUserAsync();
        Assert.NotNull(currentUser);
    }
    
    [Fact]
    public async Task GetUsers_WithActiveStatus_ShouldReturnUsers()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var users = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(new RequestOptions()
        {
            QueryString =  RedmineKeys.STATUS.WithItem(((int)UserStatus.StatusActive).ToString()) 
        });

        Assert.NotNull(users);
        Assert.True(users.Count > 0, "User count == 0");
    }
    
    [Fact]
    public async Task GetUsers_WithLockedStatus_ShouldReturnUsers()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload(status: UserStatus.StatusLocked);
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var users = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(new RequestOptions()
        {
            QueryString = RedmineKeys.STATUS.WithItem(((int)UserStatus.StatusLocked).ToString())
        });

        Assert.NotNull(users);
        Assert.True(users.Count >= 1, "User(Locked) count == 0");
    }

    [Fact]
    public async Task GetUsers_WithRegisteredStatus_ShouldReturnUsers()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload(status: UserStatus.StatusRegistered);
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var users = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(new RequestOptions()
        {
            QueryString = RedmineKeys.STATUS.WithInt((int)UserStatus.StatusRegistered)
        });

        Assert.NotNull(users);
        Assert.True(users.Count >= 1, "User(Registered) count == 0");
    }

    [Fact]
    public async Task GetUser_WithGroupsAndMemberships_ShouldIncludeRelatedData()
    {
        var roles = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Role>();
        Assert.NotEmpty(roles);

        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var membership = new Redmine.Net.Api.Types.ProjectMembership
        {
            User = new IdentifiableName { Id = user.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };

        var groupPayload = new Redmine.Net.Api.Types.Group()
        {
            Name = RandomHelper.GenerateText(3),
            Users = [IdentifiableName.Create<GroupUser>(user.Id)]
        };
        
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);

        // Act
        var projectMembership = await fixture.RedmineManager.CreateAsync(membership, TestConstants.Projects.DefaultProjectIdentifier);
        Assert.NotNull(projectMembership);
        
        user = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(user.Id.ToInvariantString(),
            RequestOptions.Include($"{RedmineKeys.GROUPS},{RedmineKeys.MEMBERSHIPS}"));

        Assert.NotNull(user);
        Assert.NotNull(user.Groups);
        Assert.NotNull(user.Memberships);
        
        Assert.True(user.Groups.Count > 0, "Group count == 0");
        Assert.True(user.Memberships.Count > 0, "Membership count == 0");
    }

    [Fact]
    public async Task GetUsers_ByGroupId_ShouldReturnFilteredUsers()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: [user.Id]);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);
        
        var users = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(new RequestOptions()
        {
            QueryString = RedmineKeys.GROUP_ID.WithInt(group.Id)
        });

        Assert.NotNull(users);
        Assert.True(users.Count > 0, "User count == 0");
    }

    [Fact]
    public async Task AddUserToGroup_WithValidIds_ShouldSucceed()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(name: null, userIds: null);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);
        
        await fixture.RedmineManager.AddUserToGroupAsync(group.Id, user.Id);

        user = fixture.RedmineManager.Get<Redmine.Net.Api.Types.User>(user.Id.ToInvariantString(), RequestOptions.Include(RedmineKeys.GROUPS));

        Assert.NotNull(user);
        Assert.NotNull(user.Groups);
        Assert.NotNull(user.Groups.FirstOrDefault(g => g.Id == group.Id));
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithValidIds_ShouldSucceed()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var user = await fixture.RedmineManager.CreateAsync(userPayload);
        Assert.NotNull(user);
        
        var groupPayload = TestEntityFactory.CreateRandomGroupPayload(userIds: [user.Id]);
        var group = await fixture.RedmineManager.CreateAsync(groupPayload);
        Assert.NotNull(group);
        
        await fixture.RedmineManager.RemoveUserFromGroupAsync(group.Id, user.Id);

        user = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.User>(user.Id.ToInvariantString(), RequestOptions.Include(RedmineKeys.GROUPS));

        Assert.NotNull(user);
        Assert.True(user.Groups == null || user.Groups.FirstOrDefault(g => g.Id == group.Id) == null);
    }
}