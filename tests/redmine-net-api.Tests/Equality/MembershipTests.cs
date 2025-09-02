using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

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
}