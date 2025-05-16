using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueWatcherTests(RedmineTestContainerFixture fixture)
{
    private Issue CreateTestIssue()
    {
        var issue = IssueTestHelper.CreateIssue();
        return fixture.RedmineManager.Create(issue);
    }

    [Fact]
    public void AddWatcher_Should_Succeed()
    {
        var issue  = CreateTestIssue();
        var userId = 1; // existing user

        fixture.RedmineManager.AddWatcherToIssue(issue.Id, userId);

        var updated = fixture.RedmineManager.Get<Issue>(
            issue.Id.ToString(),
            RequestOptions.Include("watchers"));

        Assert.Contains(updated.Watchers, w => w.Id == userId);
    }

    [Fact]
    public void RemoveWatcher_Should_Succeed()
    {
        var issue  = CreateTestIssue();
        var userId = 1;

        fixture.RedmineManager.AddWatcherToIssue(issue.Id, userId);
        fixture.RedmineManager.RemoveWatcherFromIssue(issue.Id, userId);

        var updated = fixture.RedmineManager.Get<Issue>(
            issue.Id.ToString(),
            RequestOptions.Include("watchers"));

        Assert.DoesNotContain(updated.Watchers ?? [], w => w.Id == userId);
    }
}