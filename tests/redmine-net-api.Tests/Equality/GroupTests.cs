using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class GroupTests : BaseEqualityTests<Group>
{
    protected override Group CreateSampleInstance()
    {
        return new Group
        {
            Id = 1,
            Name = "Test Group",
            Users = [new GroupUser { Id = 1, Name = "User 1" }],
            CustomFields = [new IssueCustomField { Id = 1, Name = "Field 1" }],
            Memberships = [new Membership { Id = 1, Project = new IdentifiableName { Id = 1, Name = "Project 1" } }]
        };
    }

    protected override Group CreateDifferentInstance()
    {
        var group = CreateSampleInstance();
        group.Name = "Different Group";
        group.Users = [new GroupUser { Id = 2, Name = "User 2" }];
        return group;
    }
}