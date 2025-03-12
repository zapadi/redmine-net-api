using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class TimeEntryActivityTests : BaseEqualityTests<TimeEntryActivity>
{
    protected override TimeEntryActivity CreateSampleInstance()
    {
        return new TimeEntryActivity
        {
            Id = 1,
            Name = "Development",
            IsDefault = true,
            IsActive = true
        };
    }

    protected override TimeEntryActivity CreateDifferentInstance()
    {
        return new TimeEntryActivity
        {
            Id = 2,
            Name = "Testing",
            IsDefault = false,
            IsActive = false
        };
    }
}