using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class QueryTests : BaseEqualityTests<Query>
{
    protected override Query CreateSampleInstance()
    {
        return new Query
        {
            Id = 1,
            Name = "Test Query",
            IsPublic = true,
            ProjectId = 1
        };
    }

    protected override Query CreateDifferentInstance()
    {
        return new Query
        {
            Id = 2,
            Name = "Different Query",
            IsPublic = false,
            ProjectId = 2
        };
    }
}