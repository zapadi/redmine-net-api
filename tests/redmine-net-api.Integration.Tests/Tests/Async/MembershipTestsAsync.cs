using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class MembershipTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    
    private async Task<ProjectMembership> CreateTestMembershipAsync()
    {
        var roles = await fixture.RedmineManager.GetAsync<Role>();
        Assert.NotEmpty(roles);
        
        var user = new User
        {
            Login = ThreadSafeRandom.GenerateText(10),
            FirstName = ThreadSafeRandom.GenerateText(8),
            LastName = ThreadSafeRandom.GenerateText(9),
            Email = $"{ThreadSafeRandom.GenerateText(5)}@example.com",
            Password = "password123",
            MustChangePassword = false,
            Status = UserStatus.StatusActive
        };
        
        var createdUser = await fixture.RedmineManager.CreateAsync(user);
        Assert.NotNull(createdUser);
        
        var membership = new ProjectMembership
        {
            User = new IdentifiableName { Id = createdUser.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };
        
        return await fixture.RedmineManager.CreateAsync(membership, PROJECT_ID);
    }

    [Fact]
    public async Task GetProjectMemberships_Should_Succeed()
    {
        // Act
        var memberships = await fixture.RedmineManager.GetProjectMembershipsAsync(PROJECT_ID);

        // Assert
        Assert.NotNull(memberships);
    }

    [Fact]
    public async Task CreateMembership_Should_Succeed()
    {
        // Arrange
        var roles = await fixture.RedmineManager.GetAsync<Role>();
        Assert.NotEmpty(roles);
        
        var user = new User
        {
            Login = ThreadSafeRandom.GenerateText(10),
            FirstName = ThreadSafeRandom.GenerateText(8),
            LastName = ThreadSafeRandom.GenerateText(9),
            Email = $"{ThreadSafeRandom.GenerateText(5)}@example.com",
            Password = "password123",
            MustChangePassword = false,
            Status = UserStatus.StatusActive
        };
        
        var createdUser = await fixture.RedmineManager.CreateAsync(user);
        Assert.NotNull(createdUser);
        
        var membership = new ProjectMembership
        {
            User = new IdentifiableName { Id = createdUser.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };

        // Act
        var createdMembership = await fixture.RedmineManager.CreateAsync(membership, PROJECT_ID);

        // Assert
        Assert.NotNull(createdMembership);
        Assert.True(createdMembership.Id > 0);
        Assert.Equal(membership.User.Id, createdMembership.User.Id);
        Assert.NotEmpty(createdMembership.Roles);
    }

    [Fact]
    public async Task UpdateMembership_Should_Succeed()
    {
        // Arrange
        var membership = await CreateTestMembershipAsync();
        Assert.NotNull(membership);
        
        var roles = await fixture.RedmineManager.GetAsync<Role>();
        Assert.NotEmpty(roles);
        
        // Change roles
        var newRoleId = roles.FirstOrDefault(r => membership.Roles.All(mr => mr.Id != r.Id))?.Id ?? roles.First().Id;
        membership.Roles = [new MembershipRole { Id = newRoleId }];

        // Act
        await fixture.RedmineManager.UpdateAsync(membership.Id.ToString(), membership);
        
        // Get the updated membership from project memberships
        var updatedMemberships = await fixture.RedmineManager.GetProjectMembershipsAsync(PROJECT_ID);
        var updatedMembership = updatedMemberships.Items.FirstOrDefault(m => m.Id == membership.Id);

        // Assert
        Assert.NotNull(updatedMembership);
        Assert.Contains(updatedMembership.Roles, r => r.Id == newRoleId);
    }

    [Fact]
    public async Task DeleteMembership_Should_Succeed()
    {
        // Arrange
        var membership = await CreateTestMembershipAsync();
        Assert.NotNull(membership);
        
        var membershipId = membership.Id.ToString();

        // Act
        await fixture.RedmineManager.DeleteAsync<ProjectMembership>(membershipId);
        
        // Get project memberships
        var updatedMemberships = await fixture.RedmineManager.GetProjectMembershipsAsync(PROJECT_ID);

        // Assert
        Assert.DoesNotContain(updatedMemberships.Items, m => m.Id == membership.Id);
    }
}