using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueWatcherTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void AddWatcher_Should_Succeed()
    {
        var (issue, _)  = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);
        const int userId = 1; 

        fixture.RedmineManager.AddWatcherToIssue(issue.Id, userId);

        var updated = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(),
            RequestOptions.Include(RedmineKeys.WATCHERS));

        Assert.Contains(updated.Watchers, w => w.Id == userId);
    }

    [Fact]
    public void RemoveWatcher_Should_Succeed()
    {
        var (issue, _)  = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);
        const int userId = 1;

        fixture.RedmineManager.AddWatcherToIssue(issue.Id, userId);
        fixture.RedmineManager.RemoveWatcherFromIssue(issue.Id, userId);

        var updated = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(),
            RequestOptions.Include(RedmineKeys.WATCHERS));

        Assert.DoesNotContain(updated.Watchers ?? [], w => w.Id == userId);
    }
}