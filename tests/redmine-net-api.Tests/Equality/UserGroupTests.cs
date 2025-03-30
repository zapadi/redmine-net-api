using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class UserGroupTests : BaseEqualityTests<IdentifiableName>
{
    protected override IdentifiableName CreateSampleInstance()
    {
        return new UserGroup
        {
            Id = 1,
            Name = "Test Group"
        };
    }

    protected override IdentifiableName CreateDifferentInstance()
    {
        return new UserGroup
        {
            Id = 2,
            Name = "Different Group"
        };
    }
}

