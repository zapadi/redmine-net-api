using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class CustomFieldRoleTests : BaseEqualityTests<IdentifiableName>
{
    protected override IdentifiableName CreateSampleInstance()
    {
        return new CustomFieldRole
        {
            Id = 1,
            Name = "Test Role"
        };
    }

    protected override IdentifiableName CreateDifferentInstance()
    {
        return new CustomFieldRole
        {
            Id = 2,
            Name = "Different Role"
        };
    }
}
