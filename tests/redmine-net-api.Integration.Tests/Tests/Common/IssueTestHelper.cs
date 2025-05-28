using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;

internal static class IssueTestHelper
{
    internal static void AssertBasic(Issue expected, Issue actual)
    {
        Assert.NotNull(actual);
        Assert.True(actual.Id > 0);
        Assert.Equal(expected.Subject, actual.Subject);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Project.Id, actual.Project.Id);
        Assert.Equal(expected.Tracker.Id, actual.Tracker.Id);
        Assert.Equal(expected.Status.Id, actual.Status.Id);
        Assert.Equal(expected.Priority.Id, actual.Priority.Id);
    }

    internal static (Issue, Issue payload) CreateRandomIssue(RedmineManager redmineManager, int projectId = TestConstants.Projects.DefaultProjectId,
        int trackerId = 1,
        int priorityId = 2,
        int statusId = 1,
        string subject = null,
        List<IssueCustomField> customFields = null,
        List<Watcher> watchers = null,
        List<Upload> uploads = null)
    {
        var issuePayload = TestEntityFactory.CreateRandomIssuePayload(projectId, trackerId, priorityId, statusId,
            subject, customFields, watchers, uploads);
        var issue = redmineManager.Create(issuePayload);
        Assert.NotNull(issue);
        return (issue, issuePayload);
    }

    internal static async Task<(Issue, Issue payload)> CreateRandomIssueAsync(RedmineManager redmineManager, int projectId = TestConstants.Projects.DefaultProjectId,
        int trackerId = 1,
        int priorityId = 2,
        int statusId = 1,
        string subject = null,
        List<IssueCustomField> customFields = null,
        List<Watcher> watchers = null,
        List<Upload> uploads = null)
    {
        var issuePayload = TestEntityFactory.CreateRandomIssuePayload(projectId, trackerId, priorityId, statusId,
            subject, customFields, watchers, uploads);
        var issue = await redmineManager.CreateAsync(issuePayload);
        Assert.NotNull(issue);
        return (issue, issuePayload);
    }
    
    public static (Issue first, Issue second) CreateRandomTwoIssues(RedmineManager redmineManager)
    {
        return (Build(), Build());

        Issue Build() => redmineManager.Create(TestEntityFactory.CreateRandomIssuePayload());
    }
    
    public static (IssueRelation issueRelation, Issue firstIssue, Issue secondIssue) CreateRandomIssueRelation(RedmineManager redmineManager, IssueRelationType issueRelationType = IssueRelationType.Relates)
    {
        var (i1, i2) = CreateRandomTwoIssues(redmineManager);
        var rel      = TestEntityFactory.CreateRandomIssueRelationPayload(i1.Id, i2.Id, issueRelationType);
        var relation = redmineManager.Create(rel, i1.Id.ToString());
        return (relation, i1, i2);
    }
    
    public static async Task<(Issue first, Issue second)> CreateRandomTwoIssuesAsync(RedmineManager redmineManager)
    {
        return (await BuildAsync(), await BuildAsync());

        async Task<Issue> BuildAsync() => await redmineManager.CreateAsync(TestEntityFactory.CreateRandomIssuePayload());
    }

    public static async Task<(IssueRelation issueRelation, Issue firstIssue, Issue secondIssue)> CreateRandomIssueRelationAsync(RedmineManager redmineManager, IssueRelationType issueRelationType = IssueRelationType.Relates)
    {
        var (i1, i2) = await CreateRandomTwoIssuesAsync(redmineManager);
        var rel      = TestEntityFactory.CreateRandomIssueRelationPayload(i1.Id, i2.Id, issueRelationType);
        var relation = redmineManager.Create(rel, i1.Id.ToString());
        return (relation, i1, i2);
    }
    
}