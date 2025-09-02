using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class ProjectMembershipTests : BaseEqualityTests<ProjectMembership>
{
    protected override ProjectMembership CreateSampleInstance()
    {
        return new ProjectMembership
        {
            Id = 1,
            Project = new IdentifiableName { Id = 1, Name = "Project 1" },
            User = new IdentifiableName { Id = 1, Name = "User 1" },
            Roles = [new MembershipRole { Id = 1, Name = "Developer" }]
        };
    }

    protected override ProjectMembership CreateDifferentInstance()
    {
        return new ProjectMembership
        {
            Id = 2,
            Project = new IdentifiableName { Id = 2, Name = "Project 2" },
            User = new IdentifiableName { Id = 2, Name = "User 2" },
            Roles = [new MembershipRole { Id = 2, Name = "Manager" }]
        };
    }
}