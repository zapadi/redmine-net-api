using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class ErrorTests : BaseEqualityTests<Error>
{
    protected override Error CreateSampleInstance()
    {
        return new Error( "Test error" );
    }

    protected override Error CreateDifferentInstance()
    {
        return new Error("Different error");
    }
}