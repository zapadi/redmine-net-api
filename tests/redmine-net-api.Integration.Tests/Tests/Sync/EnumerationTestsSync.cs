using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class EnumerationTests(RedmineTestContainerFixture fixture)
{
    [Fact] public void GetDocumentCategories_Should_Succeed()   => Assert.NotNull(fixture.RedmineManager.Get<DocumentCategory>());
    [Fact] public void GetIssuePriorities_Should_Succeed()      => Assert.NotNull(fixture.RedmineManager.Get<IssuePriority>());
    [Fact] public void GetTimeEntryActivities_Should_Succeed()  => Assert.NotNull(fixture.RedmineManager.Get<TimeEntryActivity>());
}