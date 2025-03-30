using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

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