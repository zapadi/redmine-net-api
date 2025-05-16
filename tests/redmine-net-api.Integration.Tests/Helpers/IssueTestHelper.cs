using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;

internal static class IssueTestHelper
{
    internal static readonly IdentifiableName ProjectIdName = IdentifiableName.Create<Project>(1);

    internal static Issue CreateIssue(List<IssueCustomField> customFields = null, List<Watcher> watchers = null,
        List<Upload> uploads = null)
        => new()
        {
            Project = ProjectIdName,
            Subject = RandomHelper.GenerateText(9),
            Description = RandomHelper.GenerateText(18),
            Tracker = 1.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 2.ToIdentifier(),
            CustomFields = customFields,
            Watchers = watchers,
            Uploads = uploads
        };

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
}