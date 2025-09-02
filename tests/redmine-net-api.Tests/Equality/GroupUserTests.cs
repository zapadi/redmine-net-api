using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class GroupUserTests : BaseEqualityTests<IdentifiableName>
{
    protected override IdentifiableName CreateSampleInstance()
    {
        return new GroupUser
        {
            Id = 1,
            Name = "Test User"
        };
    }

    protected override IdentifiableName CreateDifferentInstance()
    {
        return new GroupUser
        {
            Id = 2,
            Name = "Different User"
        };
    }
}
