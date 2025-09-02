using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class IssueStatusTests : BaseEqualityTests<IssueStatus>
{
    protected override IssueStatus CreateSampleInstance()
    {
        return new IssueStatus
        {
            Id = 1,
            Name = "New",
            IsDefault = true,
            IsClosed = false
        };
    }

    protected override IssueStatus CreateDifferentInstance()
    {
        return new IssueStatus
        {
            Id = 2,
            Name = "Closed",
            IsDefault = false,
            IsClosed = true
        };
    }
}