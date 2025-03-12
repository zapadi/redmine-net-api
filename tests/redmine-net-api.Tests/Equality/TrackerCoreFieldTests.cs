using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

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
