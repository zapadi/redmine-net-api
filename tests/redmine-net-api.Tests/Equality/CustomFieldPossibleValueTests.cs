using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class CustomFieldPossibleValueTests : BaseEqualityTests<CustomFieldPossibleValue>
{
    protected override CustomFieldPossibleValue CreateSampleInstance()
    {
        return new CustomFieldPossibleValue
        {
            Value = "test-value",
            Label = "Test Label"
        };
    }

    protected override CustomFieldPossibleValue CreateDifferentInstance()
    {
        return new CustomFieldPossibleValue
        {
            Value = "different-value",
            Label = "Different Label"
        };
    }
}