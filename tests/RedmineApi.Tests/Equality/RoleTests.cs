using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class RoleTests : BaseEqualityTests<Role>
{
    protected override Role CreateSampleInstance()
    {
        return new Role
        {
            Id = 1,
            Name = "Developer",
            Permissions = 
            [
                new Permission { Info = "add_issues" },
                new Permission { Info = "edit_issues" }
            ],
            IsAssignable = true
        };
    }

    protected override Role CreateDifferentInstance()
    {
        return new Role
        {
            Id = 2,
            Name = "Manager",
            Permissions = 
            [
                new Permission { Info = "manage_project" }
            ],
            IsAssignable = false
        };
    }
}