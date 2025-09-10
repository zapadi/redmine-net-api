using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Base;

public abstract class IssueWatcherTestsBase
{
    protected readonly RedmineTestContainerFixture Fixture;

    protected IssueWatcherTestsBase(RedmineTestContainerFixture fixture)
    {
        Fixture = fixture;
    }

    protected Issue CreateTestIssue()
    {
        var issue = IssueTestHelper.CreateIssue();
        return Fixture.RedmineManager.Create(issue);
    }

    protected static void AssertWatcherExists(Issue issue, int userId)
    {
        Assert.Contains(issue.Watchers, w => w.Id == userId);
    }

    protected static void AssertWatcherNotExists(Issue issue, int userId)
    {
        Assert.DoesNotContain(issue.Watchers ?? [], w => w.Id == userId);
    }

    protected Issue GetIssueWithWatchers(int issueId)
    {
        return Fixture.RedmineManager.Get<Issue>(
            issueId.ToString(),
            RequestOptions.Include("watchers"));
    }
}
