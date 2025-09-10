using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineAPI.Integration.Tests.Tests.Types.Base;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueWatcherTestsSync : IssueWatcherTestsBase
{
    public IssueWatcherTestsSync(RedmineTestContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void AddWatcher_Sync_Should_Succeed()
    {
        var issue = CreateTestIssue();
        var userId = 1; // existing user

        Fixture.RedmineManager.AddWatcherToIssue(issue.Id, userId);

        var updated = GetIssueWithWatchers(issue.Id);
        AssertWatcherExists(updated, userId);
    }

    [Fact]
    public void RemoveWatcher_Sync_Should_Succeed()
    {
        var issue = CreateTestIssue();
        var userId = 1;

        Fixture.RedmineManager.AddWatcherToIssue(issue.Id, userId);
        Fixture.RedmineManager.RemoveWatcherFromIssue(issue.Id, userId);

        var updated = GetIssueWithWatchers(issue.Id);
        AssertWatcherNotExists(updated, userId);
    }
}