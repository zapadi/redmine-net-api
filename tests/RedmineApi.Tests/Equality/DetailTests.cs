using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class DetailTests : BaseEqualityTests<Detail>
{
    protected override Detail CreateSampleInstance()
    {
        return new Detail
        {
            Property = "status",
            Name = "Status",
            OldValue = "1",
            NewValue = "2"
        };
    }

    protected override Detail CreateDifferentInstance()
    {
        return new Detail
        {
            Property = "priority",
            Name = "Priority",
            OldValue = "3",
            NewValue = "4"
        };
    }
}