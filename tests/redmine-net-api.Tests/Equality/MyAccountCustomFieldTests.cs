using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public class MyAccountCustomFieldTests : BaseEqualityTests<MyAccountCustomField>
{
    protected override MyAccountCustomField CreateSampleInstance()
    {
        return new MyAccountCustomField
        {
            Id = 1,
            Name = "Test Field",
            Value = "Test Value",
        };
    }

    protected override MyAccountCustomField CreateDifferentInstance()
    {
        return new MyAccountCustomField
        {
            Id = 2,
            Name = "Different Field",
            Value = "Different Value",
        };
    }
}