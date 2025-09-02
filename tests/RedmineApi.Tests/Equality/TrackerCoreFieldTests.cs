using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class TrackerCoreFieldTests : BaseEqualityTests<TrackerCoreField>
{
    protected override TrackerCoreField CreateSampleInstance()
    {
        return new TrackerCoreField("Developer");
    }

    protected override TrackerCoreField CreateDifferentInstance()
    {
        return new TrackerCoreField("Admin");
    }
}
