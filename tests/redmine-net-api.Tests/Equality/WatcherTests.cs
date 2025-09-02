using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class WatcherTests : BaseEqualityTests<Watcher>
{
    protected override Watcher CreateSampleInstance()
    {
        return new Watcher
        {
            Id = 1,
        };
    }

    protected override Watcher CreateDifferentInstance()
    {
        return new Watcher
        {
            Id = 2,
        };
    }
}