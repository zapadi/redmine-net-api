using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class MembershipTests : BaseEqualityTests<Membership>
{
    protected override Membership CreateSampleInstance()
    {
        return new Membership
        {
            Id = 1,
            Project = new IdentifiableName { Id = 1, Name = "Project 1" },
            User = new IdentifiableName { Id = 1, Name = "User 1" },
            Roles = [new MembershipRole { Id = 1, Name = "Developer", Inherited = false }]
        };
    }

    protected override Membership CreateDifferentInstance()
    {
        return new Membership
        {
            Id = 2,
            Project = new IdentifiableName { Id = 2, Name = "Project 2" },
            User = new IdentifiableName { Id = 2, Name = "User 2" },
            Roles = [new MembershipRole { Id = 2, Name = "Manager", Inherited = true }]
        };
    }

    [Fact]
    public void GetHashCode_EqualMembershipsWithDifferentUserAndGroup_ReturnsSameValue()
    {
        var membership1 = CreateSampleInstance();
        var membership2 = CreateSampleInstance();
        membership2.User = new IdentifiableName { Id = 5, Name = "User 5" };
        membership2.Group = new IdentifiableName { Id = 6, Name = "Group 6" };

        Assert.True(membership1.Equals(membership2));
        Assert.Equal(membership1.GetHashCode(), membership2.GetHashCode());
    }
}
