using System.Collections.Specialized;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class MembershipTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    
    private async Task<ProjectMembership> CreateTestMembershipAsync(string? projectId = null)
    {
        var roles = await fixture.RedmineManager.GetAsync<Role>();
        Assert.NotEmpty(roles);
        
        var user = new User
        {
            Login = RandomHelper.GenerateText(10),
            FirstName = RandomHelper.GenerateText(8),
            LastName = RandomHelper.GenerateText(9),
            Email = $"{RandomHelper.GenerateText(5)}@example.com",
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
        
        return await fixture.RedmineManager.CreateAsync(membership, projectId ?? PROJECT_ID);
    }

    [Fact]
    public async Task GetProjectMemberships_Should_Succeed()
    {
        // Act
        var memberships = await fixture.RedmineManager.GetProjectMembershipsAsync(PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(memberships);
    }

    [Fact]
    public async Task CreateMembership_Should_Succeed()
    {
        // Arrange
        var roles = await fixture.RedmineManager.GetAsync<Role>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotEmpty(roles);
        
        var user = new User
        {
            Login = RandomHelper.GenerateText(10),
            FirstName = RandomHelper.GenerateText(8),
            LastName = RandomHelper.GenerateText(9),
            Email = $"{RandomHelper.GenerateText(5)}@example.com",
            Password = "password123",
            MustChangePassword = false,
            Status = UserStatus.StatusActive
        };
        
        var createdUser = await fixture.RedmineManager.CreateAsync(user, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdUser);
        
        var membership = new ProjectMembership
        {
            User = new IdentifiableName { Id = createdUser.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };

        // Act
        var createdMembership = await fixture.RedmineManager.CreateAsync(membership, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

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
        var membership = await CreateTestMembershipAsync(PROJECT_ID);
        Assert.NotNull(membership);
        
        var roles = await fixture.RedmineManager.GetAsync<Role>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotEmpty(roles);
        
        // Change roles
        var newRoleId = roles.FirstOrDefault(r => membership.Roles.All(mr => mr.Id != r.Id))?.Id ?? roles.First().Id;
        membership.Roles = [new MembershipRole { Id = newRoleId }];

        // Act
        await fixture.RedmineManager.UpdateAsync(membership.Id.ToString(), membership, cancellationToken: TestContext.Current.CancellationToken);
        
        // Get the updated membership from project memberships
        var updatedMemberships = await fixture.RedmineManager.GetProjectMembershipsAsync(PROJECT_ID,new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                {RedmineKeys.LIMIT, "100"}
            }
        }, cancellationToken: TestContext.Current.CancellationToken);
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
        await fixture.RedmineManager.DeleteAsync<ProjectMembership>(membershipId, cancellationToken: TestContext.Current.CancellationToken);
        
        // Get project memberships
        var updatedMemberships = await fixture.RedmineManager.GetProjectMembershipsAsync(PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.DoesNotContain(updatedMemberships.Items, m => m.Id == membership.Id);
    }
}