using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public class MyAccountCustomFieldTests : BaseEqualityTests<MyAccountCustomField>
{
    protected override MyAccountCustomField CreateSampleInstance()
    {
        return new MyAccountCustomField
        {
            Id = 1,
            Name = "Test Field",
            Value = "Test Value",
            Multiple = true,
            Values = ["Test Value", "Other"],
        };
    }

    protected override MyAccountCustomField CreateDifferentInstance()
    {
        return new MyAccountCustomField
        {
            Id = 2,
            Name = "Different Field",
            Value = "Different Value",
            Values = ["Different Value"],
        };
    }

    [Fact]
    public void Should_Compare_Values_By_Content()
    {
        var left = new MyAccountCustomField { Id = 1, Values = ["a", null] };
        var right = new MyAccountCustomField { Id = 1, Values = ["a", null] };
        var different = new MyAccountCustomField { Id = 1, Values = ["a", "b"] };

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
        Assert.NotEqual(left, different);
        Assert.NotEqual(left, new MyAccountCustomField { Id = 1 });
    }
}
