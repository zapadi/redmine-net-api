using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class NullListPropertyEqualityTests
{
    [Fact]
    public void Role_DifferentId_WithNullPermissions_ReturnsFalse()
    {
        var role1 = new Role { Id = 1, Name = "Developer" };
        var role2 = new Role { Id = 2, Name = "Developer" };

        Assert.False(role1.Equals(role2));
    }

    [Fact]
    public void Role_DifferentPermissions_ReturnsFalse()
    {
        var role1 = new Role { Id = 1, Permissions = [new Permission { Info = "add_issues" }] };
        var role2 = new Role { Id = 1, Permissions = null };

        Assert.False(role1.Equals(role2));
        Assert.False(role2.Equals(role1));
    }

    [Fact]
    public void Tracker_WithNullEnabledStandardFields_EqualsItself()
    {
        var tracker1 = new Tracker { Id = 1, Name = "Bug" };
        var tracker2 = new Tracker { Id = 1, Name = "Bug" };

        Assert.True(tracker1.Equals(tracker2));
    }

    [Fact]
    public void Tracker_DifferentId_WithNullEnabledStandardFields_ReturnsFalse()
    {
        var tracker1 = new Tracker { Id = 1, Name = "Bug" };
        var tracker2 = new Tracker { Id = 2, Name = "Bug" };

        Assert.False(tracker1.Equals(tracker2));
    }

    [Fact]
    public void Group_DifferentName_WithNullLists_ReturnsFalse()
    {
        var group1 = new Group { Id = 1, Name = "Group 1" };
        var group2 = new Group { Id = 1, Name = "Group 2" };

        Assert.False(group1.Equals(group2));
    }

    [Fact]
    public void Group_DifferentMemberships_ReturnsFalse()
    {
        var group1 = new Group { Id = 1, Name = "Group", Memberships = [new Membership { Id = 1 }] };
        var group2 = new Group { Id = 1, Name = "Group", Memberships = [new Membership { Id = 2 }] };

        Assert.False(group1.Equals(group2));
    }

    [Fact]
    public void Membership_DifferentId_WithNullRoles_ReturnsFalse()
    {
        var membership1 = new Membership { Id = 1 };
        var membership2 = new Membership { Id = 2 };

        Assert.False(membership1.Equals(membership2));
    }

    [Fact]
    public void ProjectMembership_DifferentId_WithNullRoles_ReturnsFalse()
    {
        var membership1 = new ProjectMembership { Id = 1 };
        var membership2 = new ProjectMembership { Id = 2 };

        Assert.False(membership1.Equals(membership2));
    }

    [Fact]
    public void User_DifferentLogin_WithNullLists_ReturnsFalse()
    {
        var user1 = new User { Id = 1, Login = "user1" };
        var user2 = new User { Id = 1, Login = "user2" };

        Assert.False(user1.Equals(user2));
    }

    [Fact]
    public void User_DifferentGroups_ReturnsFalse()
    {
        var user1 = new User { Id = 1, Login = "user", Groups = [new UserGroup { Id = 1 }] };
        var user2 = new User { Id = 1, Login = "user", Groups = [new UserGroup { Id = 2 }] };

        Assert.False(user1.Equals(user2));
    }
}
