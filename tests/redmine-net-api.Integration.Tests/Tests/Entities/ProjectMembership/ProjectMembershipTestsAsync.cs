using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.ProjectMembership;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectMembershipTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Redmine.Net.Api.Types.ProjectMembership> CreateRandomMembershipAsync()
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
        
        return await fixture.RedmineManager.CreateAsync(membership, TestConstants.Projects.DefaultProjectIdentifier);
    }

    [Fact]
    public async Task GetProjectMemberships_WithValidProjectId_ShouldReturnMemberships()
    {
        // Act
        var memberships = await fixture.RedmineManager.GetProjectMembershipsAsync(TestConstants.Projects.DefaultProjectIdentifier);

        // Assert
        Assert.NotNull(memberships);
    }

    [Fact]
    public async Task CreateProjectMembership_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var projectMembership = await CreateRandomMembershipAsync();

        // Assert
        Assert.NotNull(projectMembership);
        Assert.True(projectMembership.Id > 0);
        Assert.NotNull(projectMembership.User);
        Assert.NotEmpty(projectMembership.Roles);
    }

    [Fact]
    public async Task UpdateProjectMembership_WithValidData_ShouldSucceed()  
    {
        // Arrange
        var membership = await CreateRandomMembershipAsync();
        Assert.NotNull(membership);
        
        var roles = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Role>();
        Assert.NotEmpty(roles);
        
        var newRoleId = roles.FirstOrDefault(r => membership.Roles.All(mr => mr.Id != r.Id))?.Id ?? roles.First().Id;
        membership.Roles = [new MembershipRole { Id = newRoleId }];

        // Act
        await fixture.RedmineManager.UpdateAsync(membership.Id.ToString(), membership);
        
        var updatedMembership = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.ProjectMembership>(membership.Id.ToString());

        // Assert
        Assert.NotNull(updatedMembership);
        Assert.Contains(updatedMembership.Roles, r => r.Id == newRoleId);
    }

    [Fact]
    public async Task DeleteProjectMembership_WithValidId_ShouldSucceed()
    {
        // Arrange
        var membership = await CreateRandomMembershipAsync();
        Assert.NotNull(membership);
        
        var membershipId = membership.Id.ToString();

        // Act
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.ProjectMembership>(membershipId);
        
        // Assert
        await Assert.ThrowsAsync<NotFoundException>(() => fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.ProjectMembership>(membershipId));
    }
    
    [Fact]
    public async Task GetProjectMemberships_ShouldReturnMemberships()
    {
        // Test implementation
    }

    [Fact]
    public async Task GetProjectMembership_WithValidId_ShouldReturnMembership()
    {
        // Test implementation
    }

   [Fact]
    public async Task CreateProjectMembership_WithInvalidData_ShouldFail()
    {
        // Test implementation
    }

    [Fact]
    public async Task UpdateProjectMembership_WithInvalidData_ShouldFail()
    {
        // Test implementation
    }

    [Fact]
    public async Task DeleteProjectMembership_WithInvalidId_ShouldFail()
    {
        // Test implementation
    }

}