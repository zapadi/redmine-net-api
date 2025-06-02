using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.ProjectMembership;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectMembershipTests(RedmineTestContainerFixture fixture)
{
    private Redmine.Net.Api.Types.ProjectMembership CreateRandomProjectMembership()
    {
        var roles = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Role>();
        Assert.NotEmpty(roles);

        var user = TestEntityFactory.CreateRandomUserPayload();
        var createdUser = fixture.RedmineManager.Create(user);
        Assert.NotNull(createdUser);

        var membership = new Redmine.Net.Api.Types.ProjectMembership
        {
            User  = new IdentifiableName { Id = createdUser.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };

        return fixture.RedmineManager.Create(membership, TestConstants.Projects.DefaultProjectIdentifier);
    }

    [Fact]
    public void GetProjectMemberships_WithValidProjectId_ShouldReturnMemberships()
    {
        var memberships = fixture.RedmineManager.GetProjectMemberships(TestConstants.Projects.DefaultProjectIdentifier);
        Assert.NotNull(memberships);
    }

    [Fact]
    public void CreateProjectMembership_WithValidData_ShouldSucceed()
    {
        var membership = CreateRandomProjectMembership();

        Assert.NotNull(membership);
        Assert.True(membership.Id > 0);
        Assert.NotNull(membership.User);
        Assert.NotEmpty(membership.Roles);
    }

    [Fact]
    public void UpdateProjectMembership_WithValidData_ShouldSucceed()
    {
        var membership = CreateRandomProjectMembership();
        Assert.NotNull(membership);
        
        var roles = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Role>();
        Assert.NotEmpty(roles);
        
        var newRoleId = roles.First(r => membership.Roles.All(mr => mr.Id != r.Id)).Id;
        membership.Roles = [new MembershipRole { Id = newRoleId }];

        // Act
        fixture.RedmineManager.Update(membership.Id.ToString(), membership);

        var updatedMembership = fixture.RedmineManager.Get<Redmine.Net.Api.Types.ProjectMembership>(membership.Id.ToString());

        // Assert        
        Assert.NotNull(updatedMembership);
        Assert.Contains(updatedMembership.Roles, r => r.Id == newRoleId);
    }

    [Fact]
    public void DeleteProjectMembership_WithValidId_ShouldSucceed()
    {
        // Arrange
        var membership = CreateRandomProjectMembership();
        Assert.NotNull(membership);
        
        // Act
        fixture.RedmineManager.Delete<Redmine.Net.Api.Types.ProjectMembership>(membership.Id.ToString());

        // Assert
        Assert.Throws<RedmineNotFoundException>(() => fixture.RedmineManager.Get<Redmine.Net.Api.Types.ProjectMembership>(membership.Id.ToString()));
    }
}