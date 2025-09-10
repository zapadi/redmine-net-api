using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class MembershipTests(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    private ProjectMembership CreateTestMembership()
    {
        var roles = fixture.RedmineManager.Get<Role>();
        Assert.NotEmpty(roles);

        var user = new User
        {
            Login              = RandomHelper.GenerateText(10),
            FirstName          = RandomHelper.GenerateText(8),
            LastName           = RandomHelper.GenerateText(9),
            Email              = $"{RandomHelper.GenerateText(5)}@example.com",
            Password           = "password123",
            MustChangePassword = false,
            Status             = UserStatus.StatusActive
        };
        var createdUser = fixture.RedmineManager.Create(user);
        Assert.NotNull(createdUser);

        var membership = new ProjectMembership
        {
            User  = new IdentifiableName { Id = createdUser.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };

        return fixture.RedmineManager.Create(membership, PROJECT_ID);
    }

    [Fact]
    public void GetProjectMemberships_Should_Succeed()
    {
        var memberships = fixture.RedmineManager.GetProjectMemberships(PROJECT_ID);
        Assert.NotNull(memberships);
    }

    [Fact]
    public void CreateMembership_Should_Succeed()
    {
        var roles = fixture.RedmineManager.Get<Role>();
        Assert.NotEmpty(roles);

        var user = new User
        {
            Login              = RandomHelper.GenerateText(10),
            FirstName          = RandomHelper.GenerateText(8),
            LastName           = RandomHelper.GenerateText(9),
            Email              = $"{RandomHelper.GenerateText(5)}@example.com",
            Password           = "password123",
            MustChangePassword = false,
            Status             = UserStatus.StatusActive
        };
        var createdUser = fixture.RedmineManager.Create(user);

        var membership = new ProjectMembership
        {
            User  = new IdentifiableName { Id = createdUser.Id },
            Roles = [new MembershipRole { Id = roles[0].Id }]
        };
        var createdMembership = fixture.RedmineManager.Create(membership, PROJECT_ID);

        Assert.NotNull(createdMembership);
        Assert.True(createdMembership.Id > 0);
        Assert.Equal(membership.User.Id, createdMembership.User.Id);
        Assert.NotEmpty(createdMembership.Roles);
    }

    [Fact]
    public void UpdateMembership_Should_Succeed()
    {
        var membership = CreateTestMembership();

        var roles = fixture.RedmineManager.Get<Role>();
        var newRoleId = roles.First(r => membership.Roles.All(mr => mr.Id != r.Id)).Id;
        membership.Roles = [new MembershipRole { Id = newRoleId }];

        fixture.RedmineManager.Update(membership.Id.ToString(), membership);

        var updatedMemberships = fixture.RedmineManager.GetProjectMemberships(PROJECT_ID);
        var updated = updatedMemberships.Items.First(m => m.Id == membership.Id);

        Assert.Contains(updated.Roles, r => r.Id == newRoleId);
    }

    [Fact]
    public void DeleteMembership_Should_Succeed()
    {
        var membership = CreateTestMembership();
        fixture.RedmineManager.Delete<ProjectMembership>(membership.Id.ToString());

        var afterDelete = fixture.RedmineManager.GetProjectMemberships(PROJECT_ID);
        Assert.DoesNotContain(afterDelete.Items, m => m.Id == membership.Id);
    }
}