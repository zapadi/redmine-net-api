using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class TrackerCustomFieldTests : BaseEqualityTests<IdentifiableName>
{
    protected override IdentifiableName CreateSampleInstance()
    {
        return new TrackerCustomField
        {
            Id = 1,
            Name = "Test Field"
        };
    }

    protected override IdentifiableName CreateDifferentInstance()
    {
        return new TrackerCustomField
        {
            Id = 2,
            Name = "Different Field"
        };
    }
}
